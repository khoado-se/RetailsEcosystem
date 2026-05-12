using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RetailsEcosystem.Customer.Application.Exceptions;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Shared.Enums;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.API.Controllers
{
    [ApiController]
    [Route("api/vnpay")]
    public class VnpayController : ControllerBase
    {
        private readonly IVnpayService           _vnpayService;
        private readonly IOrderService           _orderService;
        private readonly ILogger<VnpayController> _logger;

        public VnpayController(
            IVnpayService vnpayService,
            IOrderService orderService,
            ILogger<VnpayController> logger)
        {
            _vnpayService = vnpayService;
            _orderService = orderService;
            _logger       = logger;
        }

        // POST /api/vnpay/ipn — server-to-server callback from VNPay
        [HttpPost("ipn")]
        [AllowAnonymous]
        public async Task<IActionResult> Ipn()
        {
            var parameters = Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());

            if (!_vnpayService.ValidateSignature(parameters))
            {
                _logger.LogWarning("[IPN] Signature validation failed. Request discarded.");
                return Ok(new { RspCode = "97", Message = "Invalid signature" });
            }

            var txnRef    = parameters.GetValueOrDefault("vnp_TxnRef",           "");
            var rspCode   = parameters.GetValueOrDefault("vnp_ResponseCode",      "");
            var txnStatus = parameters.GetValueOrDefault("vnp_TransactionStatus", "");
            var txnNo     = parameters.GetValueOrDefault("vnp_TransactionNo",     "");
            var vnpAmount = parameters.GetValueOrDefault("vnp_Amount",            "0");

            _logger.LogInformation("[IPN] Received for TxnRef={TxnRef}. RspCode={RspCode}, TxnStatus={TxnStatus}, Amount={Amount}.",
                txnRef, rspCode, txnStatus, vnpAmount);

            // Look up the attempt by TxnRef
            PaymentAttempt? attempt;
            try { attempt = await _orderService.GetPaymentAttemptByTxnRefAsync(txnRef); }
            catch { return Ok(new { RspCode = "01", Message = "Order not found" }); }

            if (attempt == null)
            {
                _logger.LogWarning("[IPN] TxnRef={TxnRef} not found in PaymentAttempts. IPN discarded.", txnRef);
                return Ok(new { RspCode = "01", Message = "Order not found" });
            }

            // Idempotency: already resolved
            if (attempt.Status != PaymentAttemptStatus.Initiated)
            {
                _logger.LogInformation("[IPN] Duplicate IPN received for TxnRef={TxnRef}. AttemptStatus={Status}. Skipped.", txnRef, attempt.Status);
                return Ok(new { RspCode = "00", Message = "Already processed" });
            }

            // Parse orderId from TxnRef (format: "{orderId}-{yyyyMMddHHmmss}")
            var parts = txnRef.Split('-');
            if (parts.Length < 2 || !int.TryParse(parts[0], out int orderId))
                return Ok(new { RspCode = "01", Message = "Order not found" });

            // Amount validation
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId, string.Empty, "Admin");
                if (long.TryParse(vnpAmount, out var received))
                {
                    var expected = (long)Math.Round(order.TotalAmount * 100m, MidpointRounding.AwayFromZero);
                    if (received != expected)
                    {
                        _logger.LogWarning("[IPN] Amount mismatch for TxnRef={TxnRef}. Received={Received}, Expected={Expected}.", txnRef, received, expected);
                        return Ok(new { RspCode = "04", Message = "Invalid amount" });
                    }
                }
            }
            catch
            {
                return Ok(new { RspCode = "01", Message = "Order not found" });
            }

            try
            {
                if (rspCode == "00" && txnStatus == "00")
                {
                    await _orderService.ConfirmVnpayPaymentAsync(orderId, txnNo, txnRef);
                    _logger.LogInformation("[IPN] Payment confirmed. OrderId={OrderId}, TxnRef={TxnRef}, VnpayTxnNo={TxnNo}.", orderId, txnRef, txnNo);
                }
                else
                {
                    var attemptStatus = rspCode == "24"
                        ? PaymentAttemptStatus.Cancelled
                        : PaymentAttemptStatus.Failed;
                    await _orderService.RecordPaymentOutcomeAsync(orderId, txnRef, rspCode, attemptStatus);
                    _logger.LogInformation("[IPN] Payment {Outcome}. OrderId={OrderId}, TxnRef={TxnRef}, RspCode={RspCode}.",
                        rspCode == "24" ? "cancelled by customer" : "failed", orderId, txnRef, rspCode);
                }
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                _logger.LogWarning("[CONCURRENCY] Concurrent IPN processing detected for TxnRef={TxnRef}. Response: already processed.", txnRef);
            }

            // Always return 00 — stops VNPay retry loop
            return Ok(new { RspCode = "00", Message = "Confirm Success" });
        }

        // POST /api/vnpay/confirm-return — called by Web layer on browser ReturnUrl redirect
        [HttpPost("confirm-return")]
        [Authorize]
        public async Task<IActionResult> ConfirmReturn([FromBody] Dictionary<string, string> parameters)
        {
            if (!_vnpayService.ValidateSignature(parameters))
                return BadRequest(new { error = "Invalid signature" });

            var txnRef    = parameters.GetValueOrDefault("vnp_TxnRef",            "");
            var rspCode   = parameters.GetValueOrDefault("vnp_ResponseCode",       "");
            var txnStatus = parameters.GetValueOrDefault("vnp_TransactionStatus",  "");
            var txnNo     = parameters.GetValueOrDefault("vnp_TransactionNo",      "");

            var parts = txnRef.Split('-');
            if (parts.Length < 2 || !int.TryParse(parts[0], out int orderId))
                return BadRequest(new { error = "Invalid TxnRef" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            try
            {
                await _orderService.GetOrderByIdAsync(orderId, userId, "Customer");
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (NotFoundException)        { return NotFound(); }

            try
            {
                if (rspCode == "00" && txnStatus == "00")
                {
                    await _orderService.ConfirmVnpayPaymentAsync(orderId, txnNo, txnRef);
                }
                else
                {
                    var attemptStatus = rspCode == "24"
                        ? PaymentAttemptStatus.Cancelled
                        : PaymentAttemptStatus.Failed;
                    await _orderService.RecordPaymentOutcomeAsync(orderId, txnRef, rspCode, attemptStatus);
                }
            }
            catch (NotFoundException) { return NotFound(); }

            var updated = await _orderService.GetOrderByIdAsync(orderId, userId, "Customer");
            return Ok(new { orderId, paymentStatus = (int)updated.PaymentStatus });
        }
    }
}

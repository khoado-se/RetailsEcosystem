using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Web.Interfaces;
using RetailsEcosystem.Customer.Web.Models.Vnpay;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.Web.Controllers
{
    [Route("vnpay")]
    public class VnpayController : Controller
    {
        private readonly IVnpayWebService         _vnpayWebService;
        private readonly IOrderService            _orderService;
        private readonly ILogger<VnpayController> _logger;

        public VnpayController(
            IVnpayWebService vnpayWebService,
            IOrderService orderService,
            ILogger<VnpayController> logger)
        {
            _vnpayWebService = vnpayWebService;
            _orderService    = orderService;
            _logger          = logger;
        }

        // GET /vnpay/return — browser redirect from VNPay after payment
        [HttpGet("return")]
        public async Task<IActionResult> Return()
        {
            var parameters = Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString());

            if (!_vnpayWebService.ValidateSignature(parameters))
            {
                return View(new VnpayReturnViewModel
                {
                    Success   = false,
                    Retryable = false,
                    Message   = "Invalid signature. Please contact support if this issue persists."
                });
            }

            var responseCode = parameters.GetValueOrDefault("vnp_ResponseCode", "");
            var txnRef       = parameters.GetValueOrDefault("vnp_TxnRef", "");
            var vnpAmount    = parameters.GetValueOrDefault("vnp_Amount", "0");

            int.TryParse(txnRef.Split('-')[0], out int orderId);
            long.TryParse(vnpAmount, out long rawAmount);

            var success   = responseCode == "00";
            var cancelled = responseCode == "24";

            // Confirm payment via API — complementary to IPN, idempotent
            int attemptsLeft = -1; // -1 = unknown (unauthenticated or fetch failed)
            if (User.Identity?.IsAuthenticated == true)
            {
                var token = User.FindFirstValue("access_token");
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        await _orderService.ConfirmReturnAsync(token, parameters);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex,
                            "[ReturnUrl] confirm-return API call failed for TxnRef={TxnRef}. IPN will handle it.", txnRef);
                    }

                    if (orderId > 0)
                    {
                        try
                        {
                            var order = await _orderService.GetOrderByIdAsync(token, orderId);
                            attemptsLeft = Math.Max(0, 3 - order.PaymentAttemptCount);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex,
                                "[ReturnUrl] Failed to fetch order {OrderId} for attempt count.", orderId);
                        }
                    }
                }
            }

            string message = responseCode switch
            {
                "00" => "Payment completed successfully.",
                "24" => "Payment was cancelled.",
                "07" => "Transaction declined — suspected fraud.",
                "09" => "Transaction declined — card not registered for online payments.",
                "10" => "Transaction declined — card authentication failed three times.",
                "11" => "Payment session expired. Please try again.",
                "12" => "Your card or account has been locked.",
                "13" => "Incorrect OTP entered.",
                "51" => "Insufficient balance.",
                "65" => "Daily transaction limit reached.",
                "75" => "Payment bank is under maintenance.",
                "79" => "Incorrect payment password entered too many times.",
                _    => $"Payment unsuccessful (error code: {responseCode})."
            };

            // Retryable: not success, has orderId, and either unknown attempts or attempts remain
            var retryable = !success && orderId > 0 && (attemptsLeft == -1 || attemptsLeft > 0);

            return View(new VnpayReturnViewModel
            {
                Success      = success,
                Cancelled    = cancelled,
                Retryable    = retryable,
                AttemptsLeft = attemptsLeft,
                OrderId      = orderId,
                Amount       = rawAmount / 100,
                ResponseCode = responseCode,
                Message      = message,
            });
        }
    }
}

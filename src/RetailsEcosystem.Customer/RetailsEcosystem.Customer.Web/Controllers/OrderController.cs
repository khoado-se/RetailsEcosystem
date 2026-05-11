using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Enums;
using RetailsEcosystem.Customer.Web.Interfaces;
using RetailsEcosystem.Customer.Web.Models.Order;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.Web.Controllers
{
    [Route("orders")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService  = cartService;
        }

        // GET /orders/checkout
        [HttpGet("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var token = User.FindFirstValue("access_token")!;
            var cart = await _cartService.GetCartAsync(token);

            if (!cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            var model = new CheckoutViewModel { Cart = cart };
            return View(model);
        }

        // POST /orders/checkout
        [HttpPost("checkout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var token = User.FindFirstValue("access_token")!;

            if (!ModelState.IsValid)
            {
                model.Cart = await _cartService.GetCartAsync(token);
                return View(model);
            }

            try
            {
                var order = await _orderService.CreateOrderAsync(token, new CreateOrderDto
                {
                    ShippingAddress = model.ShippingAddress,
                    PaymentMethod   = model.PaymentMethod,
                });

                if (model.PaymentMethod == PaymentMethod.VNPay)
                {
                    var result = await _orderService.InitiatePaymentAsync(token, order.Id);
                    return Redirect(result.PaymentUrl);
                }

                return RedirectToAction(nameof(Confirmation), new { id = order.Id });
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                model.Cart = await _cartService.GetCartAsync(token);
                ModelState.AddModelError(string.Empty, "Order could not be placed. Please check stock availability.");
                return View(model);
            }
        }

        // POST /orders/{id}/retry-payment — re-initiate VNPay for an AwaitingPayment order
        [HttpPost("{id}/retry-payment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RetryPayment(int id)
        {
            var token = User.FindFirstValue("access_token")!;
            try
            {
                var result = await _orderService.InitiatePaymentAsync(token, id);
                return Redirect(result.PaymentUrl);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                TempData["ErrorMessage"] = "Payment could not be initiated. Please try again or contact support.";
                return RedirectToAction(nameof(Detail), new { id });
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Unable to initiate payment. Please try again.";
                return RedirectToAction(nameof(Detail), new { id });
            }
        }

        // GET /orders/confirmation/{id}
        [HttpGet("confirmation/{id}")]
        public async Task<IActionResult> Confirmation(int id)
        {
            var token = User.FindFirstValue("access_token")!;
            try
            {
                var order = await _orderService.GetOrderByIdAsync(token, id);
                return View(order);
            }
            catch (HttpRequestException)
            {
                return NotFound();
            }
        }

        // GET /orders
        [HttpGet("")]
        public async Task<IActionResult> History(int pageNumber = 1)
        {
            var token = User.FindFirstValue("access_token")!;
            var result = await _orderService.GetOrdersAsync(token, pageNumber, 10);
            return View(result);
        }

        // GET /orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var token = User.FindFirstValue("access_token")!;
            try
            {
                var order = await _orderService.GetOrderByIdAsync(token, id);
                return View(order);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                return Forbid();
            }
            catch (HttpRequestException)
            {
                return NotFound();
            }
        }

        // POST /orders/{id}/cancel
        [HttpPost("{id}/cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var token = User.FindFirstValue("access_token")!;
            try
            {
                await _orderService.CancelOrderAsync(token, id);
                TempData["SuccessMessage"] = "Order cancelled successfully.";
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Unable to cancel this order.";
            }
            return RedirectToAction(nameof(Detail), new { id });
        }
    }
}

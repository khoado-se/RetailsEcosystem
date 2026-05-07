using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Web.Interfaces;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.Web.Controllers
{
    [Route("cart")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        // GET /cart
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var token = User.FindFirstValue("access_token")!;
            var cart = await _cartService.GetCartAsync(token);

            var stockChecks = cart.Items.Select(async item =>
            {
                try { return await _productService.GetByIdAsync(item.ProductId); }
                catch { return null; }
            });
            var productResults = await Task.WhenAll(stockChecks);

            var warnings = cart.Items
                .Zip(productResults, (item, product) => (item, product))
                .Where(r => r.product != null && r.product.StockQuantity < r.item.Quantity)
                .Select(r => $"\"{r.item.ProductName}\" now has only {r.product!.StockQuantity} unit(s) in stock (you have {r.item.Quantity} in your cart).")
                .ToList();

            if (warnings.Count > 0)
                ViewBag.StockWarnings = warnings;

            return View(cart);
        }

        // POST /cart/items  — called via AJAX from product pages
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddItemRequest req)
        {
            var token = User.FindFirstValue("access_token")!;
            try
            {
                var cart = await _cartService.AddItemAsync(token, req.ProductId, req.Quantity);
                return Ok(new { itemCount = cart.ItemCount });
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PUT /cart/items/{id}  — called via AJAX from cart page
        [HttpPut("items/{id:int}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] UpdateItemRequest req)
        {
            var token = User.FindFirstValue("access_token")!;
            try
            {
                var cart = await _cartService.UpdateItemAsync(token, id, req.Quantity);
                return Ok(cart);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /cart/items/{id}  — called via AJAX from cart page
        [HttpDelete("items/{id:int}")]
        public async Task<IActionResult> RemoveItem(int id)
        {
            var token = User.FindFirstValue("access_token")!;
            var cart = await _cartService.RemoveItemAsync(token, id);
            return Ok(cart);
        }

        // DELETE /cart  — called via form POST from cart page
        [HttpPost("clear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var token = User.FindFirstValue("access_token")!;
            await _cartService.ClearCartAsync(token);
            return RedirectToAction(nameof(Index));
        }

        // GET /cart/count  — polled by header mini-cart
        [HttpGet("count")]
        [AllowAnonymous]
        public async Task<IActionResult> Count()
        {
            var token = User.FindFirstValue("access_token");
            if (string.IsNullOrEmpty(token))
                return Ok(new { itemCount = 0 });

            var cart = await _cartService.GetCartAsync(token);
            return Ok(new { itemCount = cart.ItemCount });
        }

        // GET /cart/summary — populates the header dropdown
        [HttpGet("summary")]
        [AllowAnonymous]
        public async Task<IActionResult> Summary()
        {
            var token = User.FindFirstValue("access_token");
            if (string.IsNullOrEmpty(token))
                return Ok(new { itemCount = 0, total = 0, items = Array.Empty<object>() });

            var cart = await _cartService.GetCartAsync(token);
            return Ok(new
            {
                itemCount = cart.ItemCount,
                total = cart.Total,
                items = cart.Items.Select(i => new
                {
                    id = i.Id,
                    productId = i.ProductId,
                    productName = i.ProductName,
                    productImageUrl = i.ProductImageUrl,
                    quantity = i.Quantity,
                    unitPrice = i.UnitPrice,
                    lineTotal = i.LineTotal
                })
            });
        }

        public record AddItemRequest(int ProductId, int Quantity);
        public record UpdateItemRequest(int Quantity);
    }
}

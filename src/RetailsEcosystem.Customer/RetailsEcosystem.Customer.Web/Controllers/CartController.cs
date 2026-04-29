using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Web.Interfaces;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.Web.Controllers
{
    [Route("cart")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET /cart
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var token = User.FindFirstValue("access_token")!;
            var cart = await _cartService.GetCartAsync(token);
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

        public record AddItemRequest(int ProductId, int Quantity);
        public record UpdateItemRequest(int Quantity);
    }
}

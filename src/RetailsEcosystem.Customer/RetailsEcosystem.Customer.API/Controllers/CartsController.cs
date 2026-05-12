using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Application.Exceptions;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Shared.DTOs.Cart;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET /api/carts
        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Ok(await _cartService.GetCartAsync(userId));
        }

        // POST /api/carts/items
        [HttpPost("items")]
        public async Task<ActionResult<CartDto>> AddItem([FromBody] AddCartItemDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            try
            {
                var cart = await _cartService.AddItemAsync(userId, dto);
                return Ok(cart);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/carts/items/{itemId}
        [HttpPut("items/{itemId:int}")]
        public async Task<ActionResult<CartDto>> UpdateItem(int itemId, [FromBody] UpdateCartItemDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            try
            {
                var cart = await _cartService.UpdateItemAsync(userId, itemId, dto);
                return Ok(cart);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE /api/carts/items/{itemId}
        [HttpDelete("items/{itemId:int}")]
        public async Task<ActionResult<CartDto>> RemoveItem(int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            try
            {
                var cart = await _cartService.RemoveItemAsync(userId, itemId);
                return Ok(cart);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // DELETE /api/carts
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }
    }
}

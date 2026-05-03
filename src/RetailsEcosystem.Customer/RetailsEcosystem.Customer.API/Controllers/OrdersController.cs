using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Order;
using RetailsEcosystem.Customer.Shared.Enums;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET /api/orders/stats
        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderStatsDto>> GetStats()
        {
            var stats = await _orderService.GetStatsAsync();
            return Ok(stats);
        }

        // POST /api/orders
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            try
            {
                var order = await _orderService.CreateOrderAsync(userId, dto);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/orders
        [HttpGet]
        public async Task<ActionResult<PagedResult<OrderDto>>> GetOrders(
            int pageNumber = 1, int pageSize = 10, OrderStatus? status = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var role = User.IsInRole("Admin") ? "Admin" : "Customer";
            var result = await _orderService.GetOrdersAsync(userId, role, new PagedRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            }, status);
            return Ok(result);
        }

        // GET /api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var role = User.IsInRole("Admin") ? "Admin" : "Customer";
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id, userId, role);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // PUT /api/orders/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderDto>> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, dto);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE /api/orders/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<OrderDto>> CancelOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            try
            {
                var order = await _orderService.CancelOrderAsync(id, userId);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
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
    }
}

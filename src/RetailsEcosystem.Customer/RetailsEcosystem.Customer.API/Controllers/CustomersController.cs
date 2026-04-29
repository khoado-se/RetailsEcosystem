using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Customer;
using System.Security.Claims;

namespace RetailsEcosystem.Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET /api/customers/me
        [HttpGet("me")]
        public async Task<ActionResult<CustomerDto>> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Ok(await _customerService.GetByIdAsync(userId));
        }

        // PUT /api/customers/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _customerService.UpdateProfileAsync(userId, dto);
            return NoContent();
        }

        // PUT /api/customers/me/password
        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _customerService.ChangePasswordAsync(userId, dto);
            return NoContent();
        }

        // GET /api/customers?pageNumber=1&pageSize=10&search=
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetAll(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null)
        {
            var result = await _customerService.GetAllAsync(
                new PagedRequest { PageNumber = pageNumber, PageSize = pageSize },
                search);
            return Ok(result);
        }

        // PUT /api/customers/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateCustomerStatusDto dto)
        {
            await _customerService.UpdateStatusAsync(id, dto.IsActive);
            return NoContent();
        }
    }
}

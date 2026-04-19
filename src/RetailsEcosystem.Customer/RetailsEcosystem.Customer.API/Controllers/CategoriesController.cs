using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;

namespace RetailsEcosystem.Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<ActionResult<PagedResult<CategoryDto>>> GetCategories(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _categoryService.GetAllAsync(new PagedRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

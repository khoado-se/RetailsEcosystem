using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Product;

namespace RetailsEcosystem.Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
            int pageNumber = 1, int pageSize = 8, int? categoryId = null,
            string? search = null, bool? isFeatured = null,
            string? sortBy = null, bool sortDesc = true)
        {
            var result = await _productService.GetAllProductAsync(new PagedRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Search = search,
                IsFeatured = isFeatured,
                SortBy = sortBy,
                SortDesc = sortDesc
            }, categoryId);

            return Ok(result);
        }

        [HttpGet("{productId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDto>> GetProduct(int productId)
        {
            var productDto = await _productService.FindProductByIdAsync(productId);

            if (productDto == null)
            {
                return NotFound();
            }

            return Ok(productDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProduct(int id, UpdateProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return BadRequest();
            }

            try
            {
                await _productService.UpdateProductAsync(productDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> PostProduct(CreateProductDto product)
        {
            var createdProductId = await _productService.CreateProductAsync(product);

            return CreatedAtAction("GetProduct", new { productId = createdProductId }, product);
        }

        [HttpDelete("{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            try
            {
                await _productService.DeleteProductAsync(productId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("featured")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetFeatureProducts(int pageNumber = 1, int pageSize = 4)
        {
            var result = await _productService.GetFeaturedProductsAsync(new PagedRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            });

            return Ok(result);
        }

    }
}

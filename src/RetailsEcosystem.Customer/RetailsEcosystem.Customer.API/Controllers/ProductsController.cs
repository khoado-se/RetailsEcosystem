using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Domain.Entities;
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
        public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(int pageNumber = 1, int pageSize = 8, int? categoryId = null)
        {
            try
            {
                var result = await _productService.GetAllProductAsync(new PagedRequest
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                }, categoryId);

                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{productId}")]
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
        public async Task<IActionResult> PutProduct(int id, UpdateProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return BadRequest();
            }

            await _productService.UpdateProductAsync(productDto);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(CreateProductDto product)
        {
            var createdProductId = await _productService.CreateProductAsync(product);

            return CreatedAtAction("GetProduct", new { productId = createdProductId }, product);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            await _productService.DeleteProductAsync(productId);

            return NoContent();
        }

        [HttpGet("featured")]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetFeatureProducts(int pageNumber = 1, int pageSize = 4)
        {
            try
            {
                var result = await _productService.GetFeaturedProductsAsync(new PagedRequest
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

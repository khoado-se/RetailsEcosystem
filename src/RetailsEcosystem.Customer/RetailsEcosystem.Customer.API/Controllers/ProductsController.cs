using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Application.Exceptions;
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
        private readonly IValidator<CreateProductDto> _createValidator;
        private readonly IValidator<UpdateProductDto> _updateValidator;

        public ProductsController(
            IProductService productService,
            IValidator<CreateProductDto> createValidator,
            IValidator<UpdateProductDto> updateValidator)
        {
            _productService = productService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
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

            var validation = await _updateValidator.ValidateAsync(productDto);
            if (!validation.IsValid)
                return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

            try
            {
                await _productService.UpdateProductAsync(productDto);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> PostProduct(CreateProductDto product)
        {
            var validation = await _createValidator.ValidateAsync(product);
            if (!validation.IsValid)
                return BadRequest(new { errors = validation.Errors.Select(e => e.ErrorMessage) });

            var createdProductId = await _productService.CreateProductAsync(product);

            return CreatedAtAction("GetProduct", new { productId = createdProductId }, new { id = createdProductId });
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
            catch (NotFoundException)
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

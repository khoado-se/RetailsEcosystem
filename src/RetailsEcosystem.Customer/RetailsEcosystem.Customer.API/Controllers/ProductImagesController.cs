using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.API.Services;
using RetailsEcosystem.Customer.Application.Exceptions;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Shared.DTOs.ProductImage;

namespace RetailsEcosystem.Customer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImagesController : ControllerBase
    {
        private readonly IFileStorageService _fileService;
        private readonly IProductImageService _productImageService;

        public ProductImagesController(IFileStorageService fileService, IProductImageService productImageService)
        {
            _fileService = fileService;
            _productImageService = productImageService;
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OnPostUploadAsync(int productId, List<IFormFile> files)
        {
            var urls = await _fileService.SaveFilesAsync(files);
            await _productImageService.AddImageRangeAsync(productId, urls);
            return Ok(new { urls });
        }

        [HttpGet("by-product/{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductImageItemDto>>> GetByProduct(int productId)
        {
            var images = await _productImageService.GetByProductIdAsync(productId);
            return Ok(images);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                var publicId = await _productImageService.DeleteImageAsync(id);
                await _fileService.DeleteFileAsync(publicId);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}


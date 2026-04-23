using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.API.Services;
using RetailsEcosystem.Customer.Application.Interfaces;

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
        public async Task<IActionResult> OnPostUploadAsync(int productId, List<IFormFile> files)
        {
            try
            {
                var urls = await _fileService.SaveFilesAsync(files);
                await _productImageService.AddImageRangeAsync(productId, urls);    

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}


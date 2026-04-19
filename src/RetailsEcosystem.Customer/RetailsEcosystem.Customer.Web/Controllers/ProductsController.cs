using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Web.Interfaces;
using RetailsEcosystem.Customer.Web.Services;

namespace RetailsEcosystem.Customer.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _productService.GetAllAsync(pageNumber, pageSize);
            return View(result);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Web.Attributes;
using RetailsEcosystem.Customer.Web.Interfaces;
using RetailsEcosystem.Customer.Web.Services;
using System.Text.Json;

namespace RetailsEcosystem.Customer.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        public ProductsController(
            IProductService productService
        )
        {
            _productService = productService;
        }
        [Breadcrumb("Shop")]
        public async Task<IActionResult> Index([FromQuery] PagedRequest pagedRequest, [FromQuery] int? categoryId = null)
        {
            var productsDto = await _productService.GetAllAsync(pagedRequest, categoryId);

            return View(viewName: "ProductIndex", model: productsDto);
        }

        [Breadcrumb("Product Details","Shop","/products")]
        public async Task<IActionResult> Details(int productId)
        {
            var productDto = await _productService.GetByIdAsync(productId);
            if (productDto == null)
            {
                return NotFound();
            }

            // Set the dynamic title for the view using ViewData
            ViewData["DynamicTitle"] = productDto.Name;

            return View(viewName: "ProductDetails", model: productDto);
        }
    }
}

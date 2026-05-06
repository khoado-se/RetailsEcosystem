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
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [Breadcrumb("Shop")]
        public async Task<IActionResult> ProductIndex([FromQuery] PagedRequest pagedRequest, [FromQuery] int? categoryId = null, [FromQuery] string? search = null)
        {
            var productsDto = await _productService.GetAllAsync(pagedRequest, categoryId, search);
            var categories  = await _categoryService.GetAllAsync(new PagedRequest { PageNumber = 1, PageSize = 200 });

            ViewBag.CategoryId  = categoryId;
            ViewBag.Search      = search;
            ViewBag.Categories  = categories.Items;

            return View(model: productsDto);
        }

        [Breadcrumb("Product Details",parentName: "Shop", parentAction: nameof(ProductIndex))]
        public async Task<IActionResult> ProductDetails(int id)
        {
            var productDto = await _productService.GetByIdAsync(id);
            if (productDto == null)
            {
                return NotFound();
            }

            // Set the dynamic title for the view using ViewData
            ViewData["DynamicTitle"] = productDto.Name;

            return View(model: productDto);
        }
    }
}

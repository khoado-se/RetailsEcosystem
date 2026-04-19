using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Web.Attributes;
using RetailsEcosystem.Customer.Web.Interfaces;
using RetailsEcosystem.Customer.Web.Models;
using System.Diagnostics;

namespace RetailsEcosystem.Customer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        public HomeController(IProductService productService)
        {
            _productService = productService;
        }
        [Breadcrumb]
        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _productService
                .GetFeaturedProductsAsync(new PagedRequest
                {
                    PageNumber = 1,
                    PageSize = 4
                });

            ViewBag.FeatureProducts = featuredProducts.Items;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

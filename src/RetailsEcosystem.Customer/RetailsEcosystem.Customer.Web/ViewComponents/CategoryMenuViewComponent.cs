using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Web.Interfaces;
using RetailsEcosystem.Customer.Web.Options;

namespace RetailsEcosystem.Customer.Web.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;
        private readonly string _apiCategoriesUrl;

        public CategoryMenuViewComponent(ICategoryService categoryService, IOptions<ApiSettings> apiSettings)
        {
            _categoryService = categoryService;
            _apiCategoriesUrl = $"{apiSettings.Value.BaseUrl.TrimEnd('/')}/api/categories";
        }

        public async Task<IViewComponentResult> InvokeAsync(string viewName = "Default")
        {
            ViewData["ApiCategoriesUrl"] = _apiCategoriesUrl;

            if (viewName == "Default")
            {
                return View(viewName);
            }

            // CategorySelect: fetch first page so Razor can resolve selectedName for the trigger label
            var items = await _categoryService.GetAllAsync(new PagedRequest { PageNumber = 1, PageSize = 50 });
            return View(viewName, items.Items);
        }
    }
}

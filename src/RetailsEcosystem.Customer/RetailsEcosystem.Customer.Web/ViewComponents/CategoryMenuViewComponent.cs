using Microsoft.AspNetCore.Mvc;
using RetailsEcosystem.Customer.Web.Interfaces;

namespace RetailsEcosystem.Customer.Web.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public CategoryMenuViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService; 
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _categoryService.GetAllAsync();
            return View(items.Items);
        }
    }
}

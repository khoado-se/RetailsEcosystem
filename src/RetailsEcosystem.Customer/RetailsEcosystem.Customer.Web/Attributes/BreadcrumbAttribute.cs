using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RetailsEcosystem.Customer.Web.Models;

namespace RetailsEcosystem.Customer.Web.Attributes
{
    public class BreadcrumbAttribute : ActionFilterAttribute
    {
        private readonly string _title; 
        private readonly string _parentName;
        private readonly string _parentUrl;
        public BreadcrumbAttribute(
            string title = null, 
            string parentName = null, 
            string parentUrl = null)
        {
            _title = title;
            _parentName = parentName;
            _parentUrl = parentUrl;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var controllerName = context.RouteData.Values["controller"]?.ToString(); // Get Name of controller
            var actionName = context.RouteData.Values["action"]?.ToString(); // Get Action was called

            if (controllerName == "Home" && actionName == "Index")
            {
                return; // Home has no breadcrumb
            }
            if (context.Controller is Controller controller)
            {
                // priority: 1. DynamicTitle > 2. Attribute Title
                var displayTitle = controller.ViewData["DynamicTitle"]?.ToString() ?? _title;

                // Initalize breadcrumb list with Home as the root
                var breadcrumbs = new List<BreadcrumbNode>
                {
                    new BreadcrumbNode { Name = "Home", Url = "/", IsActive = false }
                };
                // Add parent page to breadcrumb if specified
                if (!string.IsNullOrEmpty(_parentName))
                {
                    breadcrumbs.Add(new BreadcrumbNode
                    {
                        Name = _parentName,
                        Url = _parentUrl ?? "#",
                        IsActive = false
                    });
                }
                // Add current page to breadcrumb
                breadcrumbs.Add(new BreadcrumbNode
                {
                    Name = displayTitle,
                    Url = context.HttpContext.Request.Path, // Current path
                    IsActive = true
                });

                // Pass breadcrumb data to the view
                controller.ViewData["BreadcrumbNodes"] = breadcrumbs;
                controller.ViewData["PageTitle"] = displayTitle;
            }
            base.OnActionExecuted(context);
        }
    }
}

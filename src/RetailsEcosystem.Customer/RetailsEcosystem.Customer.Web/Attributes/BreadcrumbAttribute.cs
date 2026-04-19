using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using RetailsEcosystem.Customer.Web.Models;

namespace RetailsEcosystem.Customer.Web.Attributes
{
    /// <summary>
    /// An action filter attribute that adds breadcrumb navigation data to the view for the decorated action or
    /// controller.
    /// </summary>
    /// <remarks>Use this attribute to provide breadcrumb navigation in ASP.NET MVC applications. The
    /// attribute sets breadcrumb nodes in the controller's ViewData, allowing views to render a breadcrumb trail
    /// reflecting the current page and its parent hierarchy. Specify the title and optional parent information to
    /// customize the breadcrumb display. The attribute does not add a breadcrumb for the Home/Index action.</remarks>
    public class BreadcrumbAttribute : ActionFilterAttribute
    {
        private readonly string _title; 
        private readonly string _parentName;
        private readonly string _parentAction; // store action name of parent page for URL generation
        private readonly string _parentController; // store controller name of parent page for URL generation
        public BreadcrumbAttribute(string title = null, string parentName = null, string parentAction = null, string parentController = null)
        {
            _title = title;
            _parentName = parentName;
            _parentAction = parentAction;
            _parentController = parentController;
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
                var urlHelperFactory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
                var urlHelper = urlHelperFactory.GetUrlHelper(context);

                // Priority: DynamicTitle > Attribute Title > Action Name
                var displayTitle = controller.ViewData["DynamicTitle"]?.ToString() ?? _title ?? "Home";

                var breadcrumbs = new List<BreadcrumbNode>
                {
                    new BreadcrumbNode { Name = "Home", Url = "/", IsActive = false }
                };

                // Handle parent breadcrumb if specified
                if (!string.IsNullOrEmpty(_parentName) && !string.IsNullOrEmpty(_parentAction))
                {
                    // Generate URL for parent breadcrumb using provided action and controller
                    var targetController = _parentController ?? context.RouteData.Values["controller"]?.ToString();

                    string? pUrl = urlHelper.Action(_parentAction, targetController);

                    breadcrumbs.Add(new BreadcrumbNode
                    {
                        Name = _parentName,
                        Url = pUrl ?? "#",
                        IsActive = false
                    });
                }

                // Add current page as active breadcrumb
                breadcrumbs.Add(new BreadcrumbNode
                {
                    Name = displayTitle,
                    Url = context.HttpContext.Request.Path,
                    IsActive = true
                });

                controller.ViewData["BreadcrumbNodes"] = breadcrumbs;
                controller.ViewData["PageTitle"] = displayTitle;
            }
            base.OnActionExecuted(context);
        }
    }
}

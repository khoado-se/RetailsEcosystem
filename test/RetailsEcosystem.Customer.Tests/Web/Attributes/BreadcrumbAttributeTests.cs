using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RetailsEcosystem.Customer.Web.Attributes;
using RetailsEcosystem.Customer.Web.Models;

namespace RetailsEcosystem.Customer.Tests.Web.Attributes;

public class BreadcrumbAttributeTests
{
    private static ActionExecutedContext BuildExecutedContext(
        string controllerName,
        string actionName,
        Controller? controller = null,
        string? urlHelperResult = "/products")
    {
        var urlHelperMock = new Mock<IUrlHelper>();
        urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
            .Returns(urlHelperResult);

        var urlHelperFactoryMock = new Mock<IUrlHelperFactory>();
        urlHelperFactoryMock
            .Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
            .Returns(urlHelperMock.Object);

        var services = new ServiceCollection();
        services.AddSingleton(urlHelperFactoryMock.Object);
        var sp = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext { RequestServices = sp };
        httpContext.Request.Path = $"/{controllerName.ToLower()}/{actionName.ToLower()}";

        var routeData = new RouteData();
        routeData.Values["controller"] = controllerName;
        routeData.Values["action"] = actionName;

        var actionDescriptor = new ControllerActionDescriptor
        {
            ControllerName = controllerName,
            ActionName = actionName
        };

        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

        controller ??= new TestController();
        controller.ControllerContext = new ControllerContext(actionContext);

        return new ActionExecutedContext(actionContext, [], controller);
    }

    private class TestController : Controller { }

    // ── Tests ─────────────────────────────────────────────────────────────────

    [Fact]
    public void OnActionExecuted_HomeIndex_NoBreadcrumbSet()
    {
        var attr = new BreadcrumbAttribute("Home");
        var ctx = BuildExecutedContext("Home", "Index");

        attr.OnActionExecuted(ctx);

        var controller = (Controller)ctx.Controller;
        controller.ViewData["BreadcrumbNodes"].Should().BeNull();
    }

    [Fact]
    public void OnActionExecuted_NoParent_TwoNodeBreadcrumb()
    {
        var attr = new BreadcrumbAttribute(title: "Products");
        var ctx = BuildExecutedContext("Products", "Index");

        attr.OnActionExecuted(ctx);

        var controller = (Controller)ctx.Controller;
        var breadcrumbs = controller.ViewData["BreadcrumbNodes"] as List<BreadcrumbNode>;
        breadcrumbs.Should().NotBeNull();
        breadcrumbs!.Should().HaveCount(2);
        breadcrumbs[0].Name.Should().Be("Home");
        breadcrumbs[1].Name.Should().Be("Products");
        breadcrumbs[1].IsActive.Should().BeTrue();
    }

    [Fact]
    public void OnActionExecuted_WithParent_ThreeNodeBreadcrumb()
    {
        var attr = new BreadcrumbAttribute(
            title: "Product Detail",
            parentName: "Products",
            parentAction: "Index",
            parentController: "Products");
        var ctx = BuildExecutedContext("Products", "Details");

        attr.OnActionExecuted(ctx);

        var controller = (Controller)ctx.Controller;
        var breadcrumbs = controller.ViewData["BreadcrumbNodes"] as List<BreadcrumbNode>;
        breadcrumbs.Should().NotBeNull();
        breadcrumbs!.Should().HaveCount(3);
        breadcrumbs[0].Name.Should().Be("Home");
        breadcrumbs[1].Name.Should().Be("Products");
        breadcrumbs[2].Name.Should().Be("Product Detail");
        breadcrumbs[2].IsActive.Should().BeTrue();
    }

    [Fact]
    public void OnActionExecuted_DynamicTitle_TakesPriority()
    {
        var attr = new BreadcrumbAttribute(title: "Static Title");
        var testController = new TestController();
        // ControllerContext must be set before accessing ViewData
        var ctx = BuildExecutedContext("Products", "Details", testController);
        testController.ViewData["DynamicTitle"] = "Dynamic Product Name";

        attr.OnActionExecuted(ctx);

        var breadcrumbs = testController.ViewData["BreadcrumbNodes"] as List<BreadcrumbNode>;
        breadcrumbs.Should().NotBeNull();
        var activeCrumb = breadcrumbs!.Last();
        activeCrumb.Name.Should().Be("Dynamic Product Name");
    }

    [Fact]
    public void OnActionExecuted_SetsPageTitleInViewData()
    {
        var attr = new BreadcrumbAttribute(title: "My Page");
        var ctx = BuildExecutedContext("Cart", "Index");

        attr.OnActionExecuted(ctx);

        var controller = (Controller)ctx.Controller;
        controller.ViewData["PageTitle"].Should().Be("My Page");
    }

    [Fact]
    public void OnActionExecuted_NullTitle_FallsBackToDefaultHome()
    {
        var attr = new BreadcrumbAttribute();
        var ctx = BuildExecutedContext("Cart", "Index");

        attr.OnActionExecuted(ctx);

        var controller = (Controller)ctx.Controller;
        var breadcrumbs = controller.ViewData["BreadcrumbNodes"] as List<BreadcrumbNode>;
        breadcrumbs.Should().NotBeNull();
        breadcrumbs!.Last().Name.Should().Be("Home");
    }

    [Fact]
    public void OnActionExecuted_ParentWithNullUrl_FallsBackToHash()
    {
        var attr = new BreadcrumbAttribute(
            title: "Detail",
            parentName: "Products",
            parentAction: "Index",
            parentController: "Products");
        var ctx = BuildExecutedContext("Products", "Details", urlHelperResult: null);

        attr.OnActionExecuted(ctx);

        var controller = (Controller)ctx.Controller;
        var breadcrumbs = controller.ViewData["BreadcrumbNodes"] as List<BreadcrumbNode>;
        var parentCrumb = breadcrumbs!.FirstOrDefault(b => b.Name == "Products");
        parentCrumb.Should().NotBeNull();
        parentCrumb!.Url.Should().Be("#");
    }

    [Fact]
    public void OnActionExecuted_NonControllerContext_NoBreadcrumbSet()
    {
        var attr = new BreadcrumbAttribute(title: "Page");

        // Use a plain object as controller (not Controller)
        var httpContext = new DefaultHttpContext();
        var routeData = new RouteData();
        routeData.Values["controller"] = "Products";
        routeData.Values["action"] = "Index";

        var actionDescriptor = new ControllerActionDescriptor
        {
            ControllerName = "Products",
            ActionName = "Index"
        };
        var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

        // ctx.Controller is a plain object (not Controller)
        var ctx = new ActionExecutedContext(actionContext, [], new object());

        attr.OnActionExecuted(ctx);

        // No ViewData available — simply should not throw
    }

    [Fact]
    public void OnActionExecuted_ParentWithNullParentController_UsesCurrentController()
    {
        // parentController is null → uses context's controller name
        var attr = new BreadcrumbAttribute(
            title: "Detail",
            parentName: "Products",
            parentAction: "Index",
            parentController: null);
        var ctx = BuildExecutedContext("Products", "Details");

        attr.OnActionExecuted(ctx);

        var controller = (Controller)ctx.Controller;
        var breadcrumbs = controller.ViewData["BreadcrumbNodes"] as List<BreadcrumbNode>;
        breadcrumbs.Should().NotBeNull();
        breadcrumbs!.Should().HaveCount(3);
        breadcrumbs[1].Name.Should().Be("Products");
    }
}

using Microsoft.AspNetCore.Mvc;

namespace RetailsEcosystem.Customer.Web.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }
    }
}

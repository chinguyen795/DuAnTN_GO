using Microsoft.AspNetCore.Mvc;

namespace DuAnTN.Areas.Seller.Controllers
{
    [Area("Saller")]
    public class QL_ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace DuAnTN.Areas.Seller.Controllers
{
    [Area("Saller")]
    public class QL_DonHangController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

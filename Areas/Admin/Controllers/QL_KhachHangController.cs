using Microsoft.AspNetCore.Mvc;

namespace DuAnTN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QL_KhachHangController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace DuAnTN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuanLyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

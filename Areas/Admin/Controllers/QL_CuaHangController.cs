using Microsoft.AspNetCore.Mvc;

namespace DuAnTN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QL_CuaHangController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

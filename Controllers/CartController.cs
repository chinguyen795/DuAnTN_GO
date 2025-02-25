using Microsoft.AspNetCore.Mvc;

namespace DuAnTN.Controllers
{
    public class CartController : Controller
    {
        public async Task<IActionResult> Cart()
        {
            return View();
        }

        public async Task<IActionResult> Pay()
        {
            return View();
        }

    }
}

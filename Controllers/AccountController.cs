using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace DuAnTN.Controllers
{
    public class AccountController : Controller
    {
        public async Task<IActionResult> Profile()
        {
            return View();
        }

        public async Task<IActionResult> Address()
        {
            return View();
        }

        public async Task<IActionResult> RegisterSeller()
        {
            return View();
        }
    }
}

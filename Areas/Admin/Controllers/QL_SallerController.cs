using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NuGet.Packaging.Signing;

namespace DuAnTN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QL_SallerController : Controller
    {
        private readonly UserInfoServices _userInfoService;
        private readonly UserService _userService;
        private readonly DinerService _dinerService;
        public QL_SallerController(UserInfoServices userinfoService, UserService userService, DinerService dinerService)
        {
            _userInfoService = userinfoService;
            _userService = userService;
            _dinerService = dinerService;
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var diners = await _dinerService.GetDiningsAsync();

            foreach (var diner in diners)
            {
                diner.User = await _userService.GetUserByIdAsync(diner.UserId);  // Liên kết thông tin danh mục
            }
            if (!string.IsNullOrEmpty(search))
            {
                diners = diners.Where(u => u.DinerName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var hiddenCustomers = Request.Cookies["HiddenCustomers"]?.Split(',')
                                     .Where(s => !string.IsNullOrEmpty(s))
                                     .Select(int.Parse)
                                     .ToList() ?? new List<int>();

            ViewBag.HiddenCustomers = hiddenCustomers; // Lưu vào ViewBag để dùng trong View
            diners = diners.Where(u => !hiddenCustomers.Contains(u.Id)).ToList();

            var pagedFoods = diners.Skip((page - 1) * 10).Take(10).ToList();
            var totalFoods = diners.Count();
            var totalPages = (int)Math.Ceiling(totalFoods / (double)10);

            ViewBag.TotalPages = totalPages;
            ViewBag.Page = page;
            ViewBag.Search = search;

            return View(pagedFoods);
        }

        [HttpGet]
        [Route("Dines/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var diner = await _dinerService.GetDinerByIdAsync(id);
            if (diner == null) return NotFound();

            var result = await _dinerService.DeleteDinerAsync(id);
            if (result)
            {
                TempData["ToastMessage"] = $"✅ Saler '{diner.DinerName}' đã được xóa!";
                TempData["ToastType"] = "success";
            }
            else
            {
                TempData["ToastMessage"] = "❌ Không thể xóa saler!";
                TempData["ToastType"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Hide(int id)
        {
            var hiddenCustomers = Request.Cookies["HiddenCustomers"]?.Split(',')
                                     .Where(s => !string.IsNullOrEmpty(s))
                                     .Select(int.Parse)
                                     .ToList() ?? new List<int>();

            if (hiddenCustomers.Contains(id))
            {
                hiddenCustomers.Remove(id);
            }
            else
            {
                hiddenCustomers.Add(id);
            }

            Response.Cookies.Append("HiddenCustomers", string.Join(",", hiddenCustomers), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(1), 
                HttpOnly = true
            });

            return RedirectToAction(nameof(Index));
        }
    }
}

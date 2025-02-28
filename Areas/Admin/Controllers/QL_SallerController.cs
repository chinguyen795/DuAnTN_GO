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
            //Lấy danh sách UserInfo
            var diners = await _dinerService.GetDiningsAsync();

            foreach (var diner in diners)
            {
                diner.User = await _userService.GetUserByIdAsync(diner.Id);  // Liên kết thông tin danh mục
            }
            // Tìm kiếm nếu có
            if (!string.IsNullOrEmpty(search))
            {
                diners = diners.Where(u => u.DinerName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Lấy danh sách khách hàng bị ẩn từ Cookies
            var hiddenCustomers = Request.Cookies["HiddenCustomers"]?.Split(',')
                                     .Where(s => !string.IsNullOrEmpty(s))
                                     .Select(int.Parse)
                                     .ToList() ?? new List<int>();

            // Loại bỏ khách hàng bị ẩn khỏi danh sách hiển thị
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

            await _dinerService.DeleteDinerAsync(id);
            TempData["SuccessMessage"] = $"Danh mục '{diner.DinerName}' đã được xoá thành công.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult Hide(int id)
        {
            // Lấy danh sách ID khách hàng bị ẩn từ Cookies
            var hiddenCustomers = Request.Cookies["HiddenCustomers"]?.Split(',')
                                     .Where(s => !string.IsNullOrEmpty(s))
                                     .Select(int.Parse)
                                     .ToList() ?? new List<int>();

            // Nếu khách hàng đã bị ẩn, xóa khỏi danh sách để hiện lên lại
            if (hiddenCustomers.Contains(id))
            {
                hiddenCustomers.Remove(id);
            }
            else
            {
                hiddenCustomers.Add(id); // Nếu chưa bị ẩn, thêm vào danh sách
            }

            // Cập nhật lại Cookies với danh sách mới
            Response.Cookies.Append("HiddenCustomers", string.Join(",", hiddenCustomers), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(7), // Lưu trong 7 ngày
                HttpOnly = true
            });

            return RedirectToAction(nameof(Index));
        }
    }
}

using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using NuGet.Packaging.Signing;
using X.PagedList.Extensions;

namespace DuAnTN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QL_KhachHangController : Controller
    {
        private readonly UserInfoServices _userInfoService;
        private readonly UserService _userService;
        private readonly AddressService _AddressService;

        public QL_KhachHangController(UserInfoServices userinfoService, UserService userService, AddressService addressService)
        {
            _userInfoService = userinfoService;
            _userService = userService;
            _AddressService = addressService;
        }

        public async Task<IActionResult> Index(int? id, string search, int page = 1)
        {
            var userinfo = await _userInfoService.GetUserInfosAsync();

            foreach (var user in userinfo)
            {
                user.User = await _userService.GetUserByIdAsync(user.UserId);
            }

            if (!string.IsNullOrEmpty(search))
            {
/*                userinfo = userinfo.Where(f => f.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
*/            }

            // Lấy danh sách khách hàng bị ẩn từ Cookies
            var hiddenCustomers = Request.Cookies["HiddenCustomers"]?.Split(',')
                                     .Where(s => !string.IsNullOrEmpty(s))
                                     .Select(int.Parse)
                                     .ToList() ?? new List<int>();

            // Loại bỏ khách hàng bị ẩn khỏi danh sách hiển thị
            ViewBag.HiddenCustomers = hiddenCustomers; // Lưu vào ViewBag để dùng trong View
            userinfo = userinfo.Where(u => !hiddenCustomers.Contains(u.Id)).ToList();

            var pagedFoods = userinfo.Skip((page - 1) * 10).Take(10).ToList();
            var totalFoods = userinfo.Count();
            var totalPages = (int)Math.Ceiling(totalFoods / (double)10);

            ViewBag.TotalPages = totalPages;
            ViewBag.Page = page;
            ViewBag.Search = search;

            return View(pagedFoods);
        }
        [HttpGet]
        [Route("UserInfors/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var kh = await _userInfoService.GetUserInfoByIdAsync(id);
            if (kh == null) return NotFound();

            var result = await _userInfoService.DeleteUserInfoAsync(id);
            if (result)
            {
                TempData["ToastMessage"] = $"✅ Người dùng '{kh.User?.FullName}' đã được xóa!";
                TempData["ToastType"] = "success";
            }
            else
            {
                TempData["ToastMessage"] = "❌ Không thể xóa người dùng!";
                TempData["ToastType"] = "danger";
            }
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

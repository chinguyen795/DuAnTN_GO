using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using X.PagedList.Extensions;

namespace DuAnTN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuanLyController : Controller
    {
        private readonly UserInfoServices _userInfoService;
        private readonly UserService _userService;


        public QuanLyController(UserInfoServices userinfoService, UserService userService)
        {
            _userInfoService = userinfoService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(int? id, string search, int page = 1)
        {
            // Lấy danh sách các món ăn
            var user = await _userService.GetUsersAsync();

            // Lấy danh sách Role từ API thông qua UserService
            var roles = await _userService.GetRoleAsync();

            // Đẩy danh sách Role vào ViewBag để hiển thị trong dropdown
            ViewBag.Roles = roles;

            //Lọc theo tìm kiếm nếu có
            //foreach (var user in users)
            //{
            //    user = await _userService.GetUserByIdAsync(user.Id);  // Liên kết thông tin danh mục
            //}

            // Tìm theo thuộc tính
            if (!string.IsNullOrEmpty(search))
            {
                user = user
                    .Where(f =>
                        ((f.role != null && f.role.RoleName != null && f.role.RoleName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) ||
                         (f.Email != null && f.Email.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) ||
                         (f.Phone != null && f.Phone.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) ||
                         (f.FullName != null && f.FullName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)))

                    .ToList();
            }
            // Lấy danh sách khách hàng bị ẩn từ Cookies
            var hiddenCustomers = Request.Cookies["HiddenCustomers"]?.Split(',')
                                     .Where(s => !string.IsNullOrEmpty(s))
                                     .Select(int.Parse)
                                     .ToList() ?? new List<int>();

            // Loại bỏ khách hàng bị ẩn khỏi danh sách hiển thị
            ViewBag.HiddenCustomers = hiddenCustomers; // Lưu vào ViewBag để dùng trong View
            user = user.Where(u => !hiddenCustomers.Contains(u.Id)).ToList();

            // Nếu có id, lấy danh mục cần chỉnh sửa
            if (id.HasValue)
            {
                ViewBag.CategoryToEdit = await _userService.GetUserByIdAsync(id.Value);
            }

            var pagedFoods = user.Skip((page - 1) * 10).Take(10).ToList(); var totalFoods = user.Count();
            var totalPages = (int)Math.Ceiling(totalFoods / (double)10);

            ViewBag.TotalPages = totalPages;
            ViewBag.Page = page;
            ViewBag.Search = search;

            return View(pagedFoods);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _userService.GetRoleAsync();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            // Gọi API tạo user
            bool success = await _userService.CreateUserAsync(user);

            if (!success)
            {
                // Nếu API lỗi, hiển thị danh sách Roles để tránh lỗi View
                ViewBag.Roles = await _userService.GetRoleAsync();
                ModelState.AddModelError("", "❌ Không thể thêm người dùng. Kiểm tra lại API!");
                return View(user);
            }

            // Hiển thị thông báo thành công
            TempData["ToastMessage"] = $"✅ Người dùng '{user.FullName}' đã được thêm thành công!";
            TempData["ToastType"] = "success";

            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var us = await _userService.GetUserByIdAsync(id);
            if (us == null)
            {
                return NotFound();
            }
            return View(us); // Trả về View có chứa Modal
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User us)
        {

            await _userService.UpdateUserAsync(us.Id, us);
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        [Route("Users/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userService.DeleteUserAsync(id);
            if (result)
            {
                TempData["ToastMessage"] = $"✅ Người dùng '{user.FullName}' đã được xóa!";
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
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
            var user = await _userService.GetUsersAsync();

            var roles = await _userService.GetRoleAsync();

            ViewBag.Roles = roles;


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
            var hiddenCustomers = Request.Cookies["HiddenCustomers"]?.Split(',')
                                     .Where(s => !string.IsNullOrEmpty(s))
                                     .Select(int.Parse)
                                     .ToList() ?? new List<int>();

            ViewBag.HiddenCustomers = hiddenCustomers;
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
            bool success = await _userService.CreateUserAsync(user);

            if (!success)
            {
                ViewBag.Roles = await _userService.GetRoleAsync();
                ModelState.AddModelError("", "❌ Không thể thêm người dùng. Kiểm tra lại API!");
                return View(user);
            }

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
            return View(us);
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

            // Cập nhật lại Cookies với danh sách mới
            Response.Cookies.Append("HiddenCustomers", string.Join(",", hiddenCustomers), new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(1), 
                HttpOnly = true
            });

            return RedirectToAction(nameof(Index));
        }
    }
}
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

        public QL_KhachHangController(UserInfoServices userinfoService, UserService userService)
        {
            _userInfoService = userinfoService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(int? id,string search, int page = 1)
        {
           
            // Lấy danh sách các món ăn
            var userinfo = await _userInfoService.GetUserInfosAsync();
            // Lọc theo tìm kiếm nếu có
            if (!string.IsNullOrEmpty(search))
            {
                userinfo = userinfo.Where(c => c.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            foreach (var user in userinfo)
            {
                user.User = await _userService.GetUserByIdAsync(user.UserId);  // Liên kết thông tin danh mục
            }
            if (!string.IsNullOrEmpty(search))
            {
                userinfo = userinfo.Where(f => f.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Phân trang: lấy các sản phẩm cho trang hiện tại
            var pagedFoods = userinfo.Skip((page - 1) * 10).Take(10).ToList(); // Số lượng sản phẩm mỗi trang là 10

            // Lấy tổng số trang
            var totalFoods = userinfo.Count();
            var totalPages = (int)Math.Ceiling(totalFoods / (double)10);

            // Truyền dữ liệu vào ViewBag
            ViewBag.TotalPages = totalPages;
            ViewBag.Page = page;
            ViewBag.Search = search; // Truyền lại giá trị tìm kiếm vào ViewBag

            return View(pagedFoods);
            return View(userinfo);  // Trả lại View với danh sách món ăn đã liên kết thông tin danh mục
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var food = await _userInfoService.DeleteUserInfoAsync(id);
            if (food != null)
            {
                await _userInfoService.DeleteUserInfoAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}


using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;

namespace DuAnTN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QL_CuaHangController : Controller
    {
        private readonly DinerService _dinerService;
        private readonly UserService _userService;
        private const int PageSize = 10; // Số sản phẩm mỗi trang

        public QL_CuaHangController(DinerService dinerService, UserService userService)
        {
            _dinerService = dinerService;
            _userService = userService;
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var diners = await _dinerService.GetDiningsAsync();

            // Lấy thông tin danh mục cho mỗi món ăn
            foreach (var diner in diners)
            {
                diner.User = await _userService.GetUserByIdAsync(diner.Id);  // Liên kết thông tin danh mục
            }
            if (!string.IsNullOrEmpty(search))
            {
                diners = diners.Where(d => d.DinerName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Phân trang: lấy các sản phẩm cho trang hiện tại
            var pagedFoods = diners.Skip((page - 1) * 10).Take(10).ToList(); // Số lượng sản phẩm mỗi trang là 10

            // Lấy tổng số trang
            var totalFoods = diners.Count();
            var totalPages = (int)Math.Ceiling(totalFoods / (double)10);

            // Truyền dữ liệu vào ViewBag
            ViewBag.TotalPages = totalPages;
            ViewBag.Page = page;
            ViewBag.Search = search; // Truyền lại giá trị tìm kiếm vào ViewBag

            return View(pagedFoods);
            return View(diners);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var diner = await _dinerService.DeleteDinerAsync(id);
            if (diner != null)
            {
                await _dinerService.DeleteDinerAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

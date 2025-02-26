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
            // Lấy danh sách UserInfo
            var userinfo = await _dinerService.GetDiningsAsync();

            //foreach (var user in userinfo)
            //{
            //    user.User = await _userService.GetUserByIdAsync(user.UserId);  // Liên kết UserInfo với User
            //}

            //// Lấy danh sách Diner
            //var diners = await _dinerService.GetDiningsAsync();

            //foreach (var diner in diners)
            //{
            //    diner.User = await _userService.GetUserByIdAsync(diner.UserId);  // Liên kết Diner với User
            //}

            //// Tìm kiếm nếu có
            //if (!string.IsNullOrEmpty(search))
            //{
            //    userinfo = userinfo.Where(u => u..Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            //}

            // Gộp dữ liệu UserInfo và Diner vào ViewModel (có thể tạo ViewModel riêng nếu cần)
            //var viewModel = userinfo.Select(user => new
            //{
            //    User = user.User,
            //    UserInfo = user,
            //    Diner = diners.FirstOrDefault(d => d.UserId == user.UserId) // Tìm Diner tương ứng với UserInfo
            //}).ToList();

            // Phân trang
            //int pageSize = 10;
            //int totalItems = viewModel.Count();
            //int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            //var pagedData = viewModel.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Truyền dữ liệu vào ViewBag
            //ViewBag.TotalPages = totalPages;
            //ViewBag.Page = page;
            //ViewBag.Search = search;

            //return View(pagedData); // Trả về danh sách đã xử lý
            return View(userinfo);
        }




        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var food = await _dinerService.DeleteDinerAsync(id);
            if (food != null)
            {
                await _dinerService.DeleteDinerAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

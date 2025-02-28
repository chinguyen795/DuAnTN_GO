using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace DuAnTN.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        private readonly AuthService _authSevice;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, UserService userService, AuthService authSevice)
        {
            _logger = logger;
            _userService = userService;
            _authSevice = authSevice;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Index() 
        { 
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> Index(User user)
        {
            if (ModelState.IsValid)
            {
                user.RoleId = 3;
                await _userService.CreateUserAsync(user);

                // Sau khi lưu thành công, chuyển hướng về trang danh sách danh mục
                return RedirectToAction(nameof(Index));
            }
            return View(user); // Nếu có lỗi, trả lại view và hiển thị thông báo lỗi
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var token = await _authSevice.Login(Email, Password);

            if (string.IsNullOrEmpty(token))
            {
                TempData["ToastMessage"] = "❌ Email hoặc mật khẩu không đúng!";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var userId = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;  // Kiểm tra đúng key của token
            var fullName = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastMessage"] = "❌ Không lấy được thông tin người dùng!";
                return RedirectToAction("Index");
            }

            // Thêm log kiểm tra ID lưu vào Session
            Console.WriteLine($"🟢 User ID lưu vào session: {userId}");

            HttpContext.Session.SetString("JwtToken", token);
            HttpContext.Session.SetString("Id", userId);  // Kiểm tra có đúng key không
            HttpContext.Session.SetString("FullName", fullName ?? "");

            TempData["ToastMessage"] = $"✅ Chào mừng {fullName}, bạn đã đăng nhập thành công!";
            TempData["ToastType"] = "success";

            return RedirectToAction("Index");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JwtToken");
            HttpContext.Session.Remove("Id");
            HttpContext.Session.Remove("FullName");

            return RedirectToAction("Index");
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

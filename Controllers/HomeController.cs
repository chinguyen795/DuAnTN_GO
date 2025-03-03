using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace DuAnTN.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        private readonly AuthService _authSevice;
        private readonly FoodService _foodService;
        private readonly DinerService _dinerService;
        private readonly CategoryService _categoryService;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, CategoryService categoryService , UserService userService, AuthService authSevice, FoodService foodService, DinerService dinerService)
        {
            _logger = logger;
            _userService = userService;
            _authSevice = authSevice;
            _foodService = foodService;
            _dinerService = dinerService;
            _categoryService = categoryService;
        }

        public IActionResult Privacy()
        {
            return View();
        }
     


        public async Task<IActionResult> Index(int? categoryId)
        {
            var diners = await _dinerService.GetDiningsAsync();
            var foods = await _foodService.GetFoodsAsync();
            var categories = await _categoryService.GetCategoriesAsync();

            if (categoryId.HasValue && categoryId > 0)
            {
                foods = foods.Where(f => f.CategoryId == categoryId).ToList();   //Lọc theo CategoryId 
            }
            foreach (var food in foods)
            {
                food.Diner = diners.FirstOrDefault(d => d.Id == food.DinerId);
            }
            ViewBag.SelectedCategory = categoryId;
            ViewBag.Diners = diners;
            ViewBag.Foods = foods;
            ViewBag.Categories = categories;
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> CheckEmail(string email)
        {
            if (await _userService.IsEmailExistsAsync(email))
            {
                TempData["ToastMessage"] = "❌ Email đã tồn tại!";
                return RedirectToAction("Index"); // Quay lại trang chính nếu email tồn tại
            }

            // Gửi mã xác thực
            var isSent = await _userService.SendVerificationCodeAsync(email);
            if (isSent)
            {
                return RedirectToAction("VerifyCode", new { email });
            }

            TempData["ToastMessage"] = "❌ Gửi mã xác thực thất bại!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult VerifyCode(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index");
            }

            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCode(string email, string[] otp)
        {
            // Gộp 6 số nhập vào thành chuỗi duy nhất
            string code = string.Join("", otp);

            // Gọi API để kiểm tra mã xác thực
            if (await _userService.VerifyCodeAsync(email, code))
            {
                return RedirectToAction("Register", new { email });
            }

            TempData["ToastMessage"] = "❌ Mã xác thực không đúng!";
            return RedirectToAction("VerifyCode", new { email });
        }


        [HttpGet]
        public IActionResult Register(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index");
            }

            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "❌ Dữ liệu không hợp lệ!";
                TempData["ToastType"] = "danger";
                return View(user); // Nếu model không hợp lệ, trả lại view với các lỗi
            }


            // Kiểm tra mật khẩu và xác nhận mật khẩu
            if (user.Password != user.RePassword)
            {
                TempData["ToastMessage"] = "❌ Mật khẩu và xác nhận mật khẩu không khớp!";
                TempData["ToastType"] = "danger";
                return View(user);  // Nếu mật khẩu không khớp, trả lại view với lỗi
            }

            // Đảm bảo gán vai trò mặc định cho người dùng
            user.RoleId = 3;

            if (user.UserInfo == null)
            {
                user.UserInfo = new UserInfo
                {
                    Gender = "Trong", // Bạn có thể thay bằng giá trị mặc định hoặc yêu cầu người dùng nhập
                    BirthDay = DateTime.Now, // Có thể là giá trị mặc định nếu không có dữ liệu
                    IdentityCard = "111111111", // Cũng có thể để giá trị mặc định
                    CreateAt = DateTime.Now,
                    UserId = user.Id
                };
            }
            // Lưu mật khẩu vào cơ sở dữ liệu (chú ý là không lưu RePassword)
            var isCreated = await _userService.CreateUserAsync(user);
            if (isCreated)
            {
                TempData["ToastMessage"] = "✅ Đăng ký thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction(nameof(Index)); // Sau khi tạo thành công, chuyển hướng về trang Index
            }

            TempData["ToastMessage"] = "❌ Đăng ký thất bại!";
            TempData["ToastType"] = "danger";
            return View(user); // Trả lại view và hiển thị lỗi
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

            try
            {
                //Kiểm tra token trước khi giải mã
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);

                var userId = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                var fullName = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "fullName")?.Value;
                var role = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
                var roleIDString = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "roleID")?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(fullName))
                {
                    TempData["ToastMessage"] = "❌ Không lấy được thông tin người dùng!";
                    return RedirectToAction("Index");
                }

                //Chuyển roleID từ string sang int (nếu lỗi thì mặc định là 3)
                int roleID = int.TryParse(roleIDString, out int parsedRoleID) ? parsedRoleID : 3;

                //Lưu thông tin vào session
                HttpContext.Session.SetString("JwtToken", token);
                HttpContext.Session.SetString("Id", userId);
                HttpContext.Session.SetString("FullName", fullName);
                HttpContext.Session.SetString("Role", role ?? "User");
                HttpContext.Session.SetString("RoleID", roleID.ToString());

                TempData["ToastMessage"] = $"✅ Chào mừng {fullName}, bạn đã đăng nhập thành công!";
                TempData["ToastType"] = "success";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = "❌ Lỗi đăng nhập, vui lòng thử lại!";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["ToastMessage"] = "✅ Bạn đã đăng xuất thành công!";
            TempData["ToastType"] = "success";

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

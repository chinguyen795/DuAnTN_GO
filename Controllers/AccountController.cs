using DuAnTN.Models;
using Microsoft.AspNetCore.Mvc;
using DuAnTN.Services;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace DuAnTN.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly AddressService _addressService;
        private readonly UserService _userService;
        private readonly UserInfoServices _userInfoService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _apiUrl = "https://localhost:7248/api/Users";
        private static Dictionary<string, string> _verificationCodes = new Dictionary<string, string>();


        public AccountController(HttpClient httpClient, AddressService addressService, UserService userService, UserInfoServices userInfoService, IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = httpClient;
            _addressService = addressService;
            _userService = userService;
            _userInfoService = userInfoService;
            _webHostEnvironment = webHostEnvironment;
        }

        // Action to display the address list
        public async Task<IActionResult> Address()
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastMessage"] = "❌ Vui lòng đăng nhập để xem địa chỉ!";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index", "Home");
            }

            var addresses = await _addressService.GetAddressesAsync();
            var userAddresses = addresses.Where(a => a.UserId == int.Parse(userId)).ToList();
            return View(userAddresses);
        }

        private readonly string _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "provinces.json");

        // GET: Show Add Address Form
        public IActionResult AddAddress()
        {
            var json = System.IO.File.ReadAllText(_jsonFilePath);
            var data = JsonConvert.DeserializeObject<List<dynamic>>(json);
            ViewBag.Provinces = data;
            return View();
        }

        // POST: Handle Add Address Submission
        [HttpPost]
        public async Task<IActionResult> AddAddress(Address address, string Province, string District, string Ward)
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastMessage"] = "❌ Vui lòng đăng nhập để thêm địa chỉ!";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index", "Home");
            }

            // Combine the address fields into one
            address.Description = $"{Province}, {District}, {Ward}";
            address.UserId = int.Parse(userId);

            // Save the address using the service
            var result = await _addressService.AddAddressAsync(address);
            if (result)
            {
                TempData["ToastMessage"] = "✅ Địa chỉ đã được lưu!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Address");
            }

            TempData["ToastMessage"] = "❌ Đã có lỗi xảy ra!";
            TempData["ToastType"] = "danger";
            return View(address); // If error, return to the form
        }

        // POST: Handle Delete Address
        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var result = await _addressService.DeleteAddressAsync(id);
            if (result)
            {
                TempData["ToastMessage"] = "✅ Địa chỉ đã được xóa!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Address");
            }

            TempData["ToastMessage"] = "❌ Không thể xóa địa chỉ!";
            TempData["ToastType"] = "danger";
            return RedirectToAction("Address");
        }

        [HttpGet]
        public async Task<IActionResult> EditAddress(int id)
        {
            // Get the address to edit
            var address = await _addressService.GetAddressByIdAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            // Pass provinces data to the view
            var json = System.IO.File.ReadAllText(_jsonFilePath);
            var data = JsonConvert.DeserializeObject<List<dynamic>>(json);
            ViewBag.Provinces = data;

            return View(address); // Return the edit view with the existing address
        }

        // Xử lý cập nhật địa chỉ
        [HttpPost]
        public async Task<IActionResult> EditAddress(Address model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "❌ Dữ liệu không hợp lệ!";
                TempData["ToastType"] = "danger";
                return View(model);
            }

            bool result = await _addressService.UpdateAddressAsync(model);
            if (!result)
            {
                TempData["ToastMessage"] = "❌ Cập nhật địa chỉ thất bại!";
                TempData["ToastType"] = "danger";
                return View(model);
            }

            TempData["ToastMessage"] = "✅ Địa chỉ đã được cập nhật thành công!";
            TempData["ToastType"] = "success";

            return RedirectToAction("Address");
        }
        public async Task<IActionResult> UserInfo()
        {
            var userId = HttpContext.Session.GetString("Id");

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastMessage"] = "❌ Vui lòng đăng nhập để xem thông tin!";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index", "Home");
            }

            var userInfo = await _userInfoService.GetUserInfoByIdAsync(int.Parse(userId));

            if (userInfo == null)
            {
                TempData["ToastMessage"] = "⚠ Bạn chưa có thông tin, vui lòng thêm!";
                return View();
            }

            // Lấy thông tin FullName từ bảng User
            var user = await _userService.GetUserByIdAsync(int.Parse(userId));
            ViewBag.FullName = user?.FullName ?? "Chưa có thông tin";
            ViewBag.Phone = user?.Phone;

            return View(userInfo);
        }


        public IActionResult AddUserInfo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUserInfo(UserInfo userInfo)
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastMessage"] = "❌ Bạn cần đăng nhập trước khi thêm thông tin!";
                return RedirectToAction("Index", "Home");
            }

            userInfo.UserId = int.Parse(userId);
            var result = await _userInfoService.CreateUserInfoAsync(userInfo);

            if (result)
            {
                TempData["ToastMessage"] = "✅ Thêm thông tin thành công!";
                return RedirectToAction("UserInfo");
            }

            TempData["ToastMessage"] = "❌ Thêm thông tin thất bại!";
            return View(userInfo);
        }


        [HttpGet]
        public async Task<IActionResult> EditUserInfo()
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastMessage"] = "❌ Bạn cần đăng nhập để chỉnh sửa thông tin!";
                return RedirectToAction("Index", "Home");
            }

            var userInfo = await _userInfoService.GetUserInfoByIdAsync(int.Parse(userId));
            if (userInfo == null)
            {
                TempData["ToastMessage"] = "⚠ Bạn chưa có hồ sơ, vui lòng thêm thông tin!";
                return RedirectToAction("AddUserInfo");
            }

            var user = await _userService.GetUserByIdAsync(int.Parse(userId));
            ViewBag.FullName = user?.FullName ?? "Chưa có thông tin";

            return View(userInfo);
        }

        [HttpPost]
        public async Task<IActionResult> EditUserInfo(UserInfo userInfo, string FullName, IFormFile AvatarFile)
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastMessage"] = "❌ Bạn cần đăng nhập để chỉnh sửa thông tin!";
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "❌ Dữ liệu không hợp lệ!";
                return View(userInfo);
            }

            // Xử lý Upload Avatar nếu có file mới
            if (AvatarFile != null && AvatarFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "avatars");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = $"{Guid.NewGuid()}_{AvatarFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await AvatarFile.CopyToAsync(stream);
                }

                // Lưu tên file vào database
                userInfo.Avatar = fileName;
            }

            // Cập nhật thông tin người dùng
            var result = await _userInfoService.UpdateUserInfoAsync(userInfo, FullName);
            if (result)
            {
                TempData["ToastMessage"] = "✅ Hồ sơ đã được cập nhật thành công!";
                return RedirectToAction("UserInfo");
            }

            TempData["ToastMessage"] = "❌ Cập nhật không thành công!";
            return View(userInfo);
        }

        // GET: Hiển thị form đổi mật khẩu
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastMessage"] = "❌ Bạn cần đăng nhập để thay đổi mật khẩu!";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Login", "Home");
            }

            if (newPassword != confirmPassword)
            {
                TempData["ToastMessage"] = "❌ Mật khẩu mới không khớp!";
                TempData["ToastType"] = "danger";
                return RedirectToAction("ChangePassword");
            }

            // Gọi API hoặc Service để thay đổi mật khẩu
            var result = await _userService.ChangePasswordAsync(int.Parse(userId), currentPassword, newPassword);

            if (result)
            {
                TempData["ToastMessage"] = "✅ Mật khẩu đã được cập nhật thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("UserInfo");
            }

            TempData["ToastMessage"] = "❌ Đổi mật khẩu thất bại. Mật khẩu cũ không đúng!";
            TempData["ToastType"] = "danger";
            return RedirectToAction("ChangePassword");
        }
        // GET: Hiển thị form nhập email
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Gửi mã xác nhận đến email
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                TempData["ToastMessage"] = "❌ Vui lòng nhập email!";
                TempData["ToastType"] = "danger";
                return View();
            }

            // Kiểm tra email có tồn tại không
            var emailExists = await _userService.IsEmailExistsAsync(Email);
            if (!emailExists)
            {
                TempData["ToastMessage"] = "❌ Email không tồn tại trong hệ thống!";
                TempData["ToastType"] = "danger";
                return View();
            }

            // Gửi OTP sử dụng API mới dành riêng cho reset password
            var otpSent = await _userService.SendResetPasswordOTPAsync(Email);
            if (!otpSent)
            {
                TempData["ToastMessage"] = "❌ Gửi mã xác thực thất bại!";
                TempData["ToastType"] = "danger";
                return View();
            }

            return RedirectToAction("VerifyResetPasswordOTP", new { Email });
        }


        // GET: Hiển thị form nhập mã xác nhận và mật khẩu mới
        [HttpGet]
        public IActionResult ResetPassword(string Email)
        {
            ViewBag.Email = Email;
            return View();
        }

        // Xác thực mã và đặt lại mật khẩu
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string Email, string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword)
            {
                TempData["ToastMessage"] = "❌ Mật khẩu xác nhận không khớp!";
                return View();
            }

            var resetRequest = new { Email, NewPassword };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(resetRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7248/api/Users/ChangePassword", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync(); // Lấy nội dung lỗi từ API
                TempData["ToastMessage"] = $"❌ Không thể đặt lại mật khẩu! {errorMessage}";
                return View();
            }

            TempData["ToastMessage"] = "✅ Mật khẩu đã được đặt lại thành công!";
            return RedirectToAction("Login", "Home");
        }


        [HttpGet]
        public IActionResult VerifyOTP(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                TempData["ToastMessage"] = "❌ Email không hợp lệ!";
                return RedirectToAction("ForgotPassword");
            }

            ViewBag.Email = Email;
            return View();
        }
        [HttpPost]
        public IActionResult VerifyOTP(string Email, string OTPCode)
        {
            if (string.IsNullOrEmpty(OTPCode) || OTPCode.Length != 6)
            {
                TempData["ToastMessage"] = "Mã xác nhận không hợp lệ!";
                return RedirectToAction("VerifyOTP", new { Email = Email });
            }

            // Kiểm tra mã OTP có tồn tại và đúng không
            if (!_verificationCodes.ContainsKey(Email) || _verificationCodes[Email] != OTPCode)
            {
                TempData["ToastMessage"] = "Mã xác nhận không đúng!";
                return RedirectToAction("VerifyOTP", new { Email = Email });
            }

            // Nếu OTP đúng, xóa mã OTP để tránh dùng lại
            _verificationCodes.Remove(Email);

            return RedirectToAction("ResetPassword", new { Email = Email }); // Chuyển đến trang đặt lại mật khẩu
        }


        [HttpPost]
        public async Task<IActionResult> ResetUserPassword(string Email, string NewPassword, string ConfirmPassword)
        {
            if (NewPassword != ConfirmPassword)
            {
                TempData["ToastMessage"] = "❌ Mật khẩu xác nhận không khớp!";
                return View();
            }

            var resetRequest = new { Email, NewPassword };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(resetRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7248/api/Users/ResetPassword", jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["ToastMessage"] = $"❌ Không thể đặt lại mật khẩu! {errorMessage}";
                return View();
            }

            TempData["ToastMessage"] = "✅ Mật khẩu đã được đặt lại thành công!";

            return RedirectToAction("Index", "Home");
        }
        // Gửi mã xác thực cho quên mật khẩu
        [HttpPost]
        public async Task<IActionResult> SendResetPasswordOTP(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                TempData["ToastMessage"] = "❌ Vui lòng nhập email!";
                return View("ForgotPassword");
            }

            var emailExists = await _userService.IsEmailExistsAsync(Email);
            if (!emailExists)
            {
                TempData["ToastMessage"] = "❌ Email không tồn tại!";
                return View("ForgotPassword");
            }

            var otpSent = await _userService.SendResetPasswordOTPAsync(Email);
            if (!otpSent)
            {
                TempData["ToastMessage"] = "❌ Gửi mã xác thực thất bại!";
                return View("ForgotPassword");
            }

            return RedirectToAction("VerifyResetPasswordOTP", new { Email });
        }

        // Hiển thị form nhập mã OTP
        [HttpGet]
        public IActionResult VerifyResetPasswordOTP(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                TempData["ToastMessage"] = "❌ Email không hợp lệ!";
                return RedirectToAction("ForgotPassword");
            }

            ViewBag.Email = Email;
            return View();
        }

        // Xác minh mã OTP cho đặt lại mật khẩu
        [HttpPost]
        public async Task<IActionResult> VerifyResetPasswordOTP(string Email, string OTPCode)
        {
            if (string.IsNullOrEmpty(OTPCode) || OTPCode.Length != 6)
            {
                TempData["ToastMessage"] = "❌ Mã xác nhận không hợp lệ!";
                return View();
            }

            var otpVerified = await _userService.VerifyResetPasswordOTPAsync(Email, OTPCode);
            if (!otpVerified)
            {
                TempData["ToastMessage"] = "❌ Mã xác thực không đúng!";
                return View();
            }

            return RedirectToAction("ResetPassword", new { Email });
        }


    }
}

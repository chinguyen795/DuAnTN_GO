using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DuAnTN.Controllers
{
    public class RegisterSellerController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly AddressService _addressService;
        private readonly UserService _userService;
        private readonly UserInfoServices _userInfoService;
        private readonly DinerService _dinerService;

        public RegisterSellerController(HttpClient httpClient, AddressService addressService, UserService userService, UserInfoServices userInfoService, DinerService dinerService)
        {
            _httpClient = httpClient;
            _addressService = addressService;
            _userService = userService;
            _userInfoService = userInfoService;
            _dinerService = dinerService;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterSeller()
        {
            var userId = GetCurrentUserId();
            User? user = null;

            if (userId != null) 
            {
                user = await _userService.GetUserByIdAsync(userId.Value);
            }

            return View(user);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterSeller(string dinerName, string dinerAddress, string taxCode, IFormFile identityImage, IFormFile mainImage)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null) return NotFound();

            if (string.IsNullOrWhiteSpace(dinerName) || string.IsNullOrWhiteSpace(dinerAddress))
            {
                return RedirectToAction("RegisterSeller");
            }

            taxCode = string.IsNullOrWhiteSpace(taxCode) ? "0000000000" : taxCode;

            var userInfo = await _userInfoService.GetUserInfo(userId.Value);
            if (userInfo == null) return RedirectToAction("RegisterSeller");

            string identityImagePath = identityImage != null ? await UploadFile(identityImage) : userInfo.IdentityImageCard ?? "/images/default-cccd.jpg";
            string mainImagePath = mainImage != null ? await UploadFile(mainImage) : "/images/default-shop.jpg";

            userInfo.IdentityCard = taxCode;
            userInfo.IdentityImageCard = identityImagePath;
            await _userInfoService.UpdateUserInfoAsync(userId.Value, userInfo);

            var diner = new Diner
            {
                DinerName = dinerName,
                DinerAddress = dinerAddress,
                PhoneNumber = user.Phone,
                TaxCode = "0000000000",
                MainImage = "default-shop.jpg",
                UserId = userId.Value
            };

            var result = await _dinerService.CreateDinerAsync(diner);
            if (result)
            {
                user.RoleId = 2; // Chuyển role thành seller
                await _userService.UpdateUserAsync(userId.Value, user);

                return RedirectToAction("Index", "QL_CuaHang", new { area = "Seller" });
            }

            return RedirectToAction("RegisterSeller");
        }


        // Hàm Upload File wwwroot images và trả về đường dẫn
        private async Task<string> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/images/" + fileName;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.TryParse(userIdClaim.Value, out int userId) ? userId : (int?)null : null;
        }
    }
}

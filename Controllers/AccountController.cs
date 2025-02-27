using DuAnTN.Models;
using Microsoft.AspNetCore.Mvc;
using DuAnTN.Services;
using System.Threading.Tasks;
using System.Net.Http;
using DuAnTN.Services;

namespace DuAnTN.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly AddressService _addressService;
        private readonly UserService _userService;  // Add the UserService as a private field

        public AccountController(HttpClient httpClient, AddressService addressService, UserService userService)
        {
            _httpClient = httpClient;
            _addressService = addressService;
            _userService = userService;
        }
        // Action để hiển thị thông tin profile
       
        // Lấy danh sách địa chỉ từ API
        public async Task<IActionResult> Address()
        {
            var addresses = await _addressService.GetAddressesAsync(); // Gọi service lấy dữ liệu
            return View(addresses);
        }
        // Hiển thị form thêm địa chỉ mới
        public IActionResult AddAddress()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddAddress(AddressModel address)
        {
            
                var result = await _addressService.AddAddressAsync(address);
                if (result)
                {
                    return RedirectToAction("Address");  // Điều hướng về trang danh sách địa chỉ
                }
            
            return View(address);  // Nếu có lỗi, quay lại form nhập
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var result = await _addressService.DeleteAddressAsync(id);
            if (result)
            {
                return RedirectToAction("Address");  // Quay lại danh sách địa chỉ sau khi xóa
            }

            ModelState.AddModelError("", "Không thể xóa địa chỉ.");
            return RedirectToAction("Address");
        }
        // GET: Account/EditAddress/{id}
        [HttpGet]
        public async Task<IActionResult> EditAddress(int id)
        {
            var address = await _addressService.GetAddressByIdAsync(id); // Lấy địa chỉ theo id
            if (address == null)
            {
                return NotFound();  // Trả về lỗi 404 nếu không tìm thấy địa chỉ
            }
            return View(address);  // Trả về view chỉnh sửa với thông tin địa chỉ
        }

        // POST: Account/EditAddress
        [HttpPost]
        public async Task<IActionResult> EditAddress(AddressModel address)
        {
            if (ModelState.IsValid)
            {
                var result = await _addressService.UpdateAddressAsync(address); // Cập nhật địa chỉ thông qua service
                if (result)
                {
                    return RedirectToAction("Address"); // Quay lại danh sách địa chỉ sau khi cập nhật
                }
                ModelState.AddModelError("", "Không thể cập nhật địa chỉ.");
            }
            return View(address);  // Trả về form nếu có lỗi
        }
       /* public async Task<IActionResult> Profile()
        {
            // Call the service to get user info for UserId = 1
            var userInfo = await _userService.GetUserInfoByIdAsync(1);

            if (userInfo == null)
            {
                return NotFound();  // Return 404 if no user info is found
            }

            return View(userInfo);  // Pass the userInfo to the view
        }*/

    }
}

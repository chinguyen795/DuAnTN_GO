using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DuAnTN.Areas.Saller.Controllers
{
    [Area("Saller")]
    public class QL_CuaHangController : Controller
    {
        private readonly DinerService _dinerService;

        public QL_CuaHangController(DinerService dinerService)
        {
            _dinerService = dinerService;
        }

        // Hiển thị thông tin cửa hàng của seller
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastMessage"] = "❌ Vui lòng đăng nhập để xem thông tin!";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index", "Home");
            }

            var diners = await _dinerService.GetDinersByUserIdAsync(int.Parse(userId));

            if (diners == null || diners.Count == 0)  // Sửa điều kiện kiểm tra
            {
                TempData["ToastMessage"] = "⚠ Không tìm thấy cửa hàng nào!";
                return View(new List<Diner>());  // Trả về danh sách rỗng, tránh lỗi View
            }

            return View(diners);  // Truyền danh sách quán ăn vào View
        }

        public async Task<IActionResult> Edit(int id)
        {
            var diner = await _dinerService.GetDinerByIdAsync(id);

            if (diner == null)
            {
                TempData["ToastMessage"] = "⚠ Không tìm thấy cửa hàng!";
                return RedirectToAction("Index");
            }

            return View(diner);
        }


        // Xử lý cập nhật thông tin cửa hàng
        [HttpPost]
        public async Task<IActionResult> Update(Diner diner, IFormFile mainImageFile, IFormFile image1File, IFormFile image2File)
        {
            if (diner.Id == 0)
            {
                TempData["ToastMessage"] = "❌ ID không hợp lệ!";
                return View("Edit", diner);
            }

            var existingDiner = await _dinerService.GetDinerByIdAsync(diner.Id);
            if (existingDiner == null)
            {
                TempData["ToastMessage"] = "⚠ Không tìm thấy cửa hàng!";
                return RedirectToAction(nameof(Index));
            }

            existingDiner.DinerName = diner.DinerName;
            existingDiner.DinerAddress = diner.DinerAddress;
            existingDiner.PhoneNumber = diner.PhoneNumber;

            if (mainImageFile != null) existingDiner.MainImage = await UploadFile(mainImageFile);
            if (image1File != null) existingDiner.Image1 = await UploadFile(image1File);
            if (image2File != null) existingDiner.Image2 = await UploadFile(image2File);

            var success = await _dinerService.UpdateDinerAsync(existingDiner.Id, existingDiner);

            if (!success)
            {
                TempData["ToastMessage"] = "❌ Cập nhật thất bại!";
                return View("Edit", diner);
            }

            TempData["ToastMessage"] = "✅ Cập nhật thành công!";
            return RedirectToAction(nameof(Index));
        }


        // Hàm xử lý upload file, lưu vào thư mục wwwroot/images và trả về đường dẫn tương đối
        private async Task<string> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var fileName = Path.GetFileName(file.FileName);
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/images/" + fileName;
        }

        // Hàm lấy UserId của seller từ Claims (trong Identity)
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}

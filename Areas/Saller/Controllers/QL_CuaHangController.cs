using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DuAnTN.Areas.Seller.Controllers
{
    [Area("Saller")]
    public class QL_CuaHangController : Controller
    {
        private readonly DinerService _dinerService;

        public QL_CuaHangController(DinerService dinerService)
        {
            _dinerService = dinerService;
        }

        // Hiển thị trang chỉnh sửa cửa hàng của seller
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastMessage"] = "❌ Vui lòng đăng nhập để tiếp tục!";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index", "Home");
            }

            // Lấy duy nhất một cửa hàng của user thay vì danh sách
            var diner = await _dinerService.GetDinerByUserIdAsync(int.Parse(userId));

            if (diner == null)
            {
                TempData["ToastMessage"] = "⚠ Bạn chưa có cửa hàng nào!";
                return RedirectToAction("Index", "Home");
            }

            return View(diner); // ✅ Trả về một đối tượng duy nhất
        }


        // Xử lý cập nhật thông tin cửa hàng
        [HttpPost]
        public async Task<IActionResult> Update(Diner diner, IFormFile mainImageFile, IFormFile image1File, IFormFile image2File)
        {
            if (diner.Id == 0)
            {
                TempData["ToastMessage"] = "❌ ID không hợp lệ!";
                return RedirectToAction(nameof(Index));
            }

            var existingDiner = await _dinerService.GetDinerByIdAsync(diner.Id);
            if (existingDiner == null)
            {
                TempData["ToastMessage"] = "⚠ Không tìm thấy cửa hàng!";
                return RedirectToAction(nameof(Index));
            }

            // Cập nhật thông tin
            existingDiner.DinerName = diner.DinerName;
            existingDiner.DinerAddress = diner.DinerAddress;
            existingDiner.PhoneNumber = diner.PhoneNumber;
            existingDiner.TaxCode = diner.TaxCode;

            // Cập nhật ảnh nếu có
            if (mainImageFile != null) existingDiner.MainImage = await UploadFile(mainImageFile);
            if (image1File != null) existingDiner.Image1 = await UploadFile(image1File);
            if (image2File != null) existingDiner.Image2 = await UploadFile(image2File);

            var success = await _dinerService.UpdateDinerAsync(existingDiner.Id, existingDiner);

            if (!success)
            {
                TempData["ToastMessage"] = "❌ Cập nhật thất bại!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ToastMessage"] = "✅ Cập nhật thành công!";
            return RedirectToAction(nameof(Index));
        }

        // Hàm upload file ảnh
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
    }
}

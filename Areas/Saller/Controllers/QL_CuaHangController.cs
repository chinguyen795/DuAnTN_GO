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
            int userId = 1; // Tạm thời set cứng userId là 1
            var diner = await _dinerService.GetDinerByUserIdAsync(userId);

            if (diner == null)
            {
                diner = new Diner(); // Hiển thị form trống nếu chưa có dữ liệu
            }

            return View(diner);
        }


        // Xử lý cập nhật thông tin cửa hàng
        [HttpPost]
        public async Task<IActionResult> Update(Diner diner, IFormFile mainImageFile, IFormFile image1File, IFormFile image2File)
        {
            if (ModelState.IsValid)
            {
                int userId = GetCurrentUserId();
                var existingDiner = await _dinerService.GetDinerByUserIdAsync(userId);

                if (existingDiner == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin
                existingDiner.DinerName = diner.DinerName;
                existingDiner.DinerAddress = diner.DinerAddress;
                existingDiner.PhoneNumber = diner.PhoneNumber;

                // Cập nhật ảnh nếu có file mới
                if (mainImageFile != null)
                {
                    existingDiner.MainImage = await UploadFile(mainImageFile);
                }
                if (image1File != null)
                {
                    existingDiner.Image1 = await UploadFile(image1File);
                }
                if (image2File != null)
                {
                    existingDiner.Image2 = await UploadFile(image2File);
                }

                await _dinerService.UpdateDinerAsync(existingDiner.Id, existingDiner);
                TempData["SuccessMessage"] = "Cập nhật thông tin cửa hàng thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View("Index", diner);
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

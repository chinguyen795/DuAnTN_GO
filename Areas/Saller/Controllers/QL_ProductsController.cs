using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DuAnTN.Controllers
{
    [Area("Saller")]
    
    public class QL_ProductsController : Controller
    {
        private readonly FoodService _foodService;
        private readonly CategoryService _categoryService;
        private readonly DinerService _dinerService;
        private const int FakeDinerId = 1; // Giả lập cửa hàng có Id = 1

        public QL_ProductsController(FoodService foodService, CategoryService categoryService, DinerService dinerService)
        {
            _foodService = foodService;
            _categoryService = categoryService;
            _dinerService = dinerService;
        }

        // Hiển thị danh sách món ăn của cửa hàng (Index)
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            // Lấy danh sách món ăn của cửa hàng có ID = 1
            var foods = await _foodService.GetFoodsByDinerIdAsync(FakeDinerId);

            if (!string.IsNullOrEmpty(search))
            {
                foods = foods.Where(f => f.FoodName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Phân trang
            int pageSize = 10;
            var pagedFoods = foods.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(foods.Count / (double)pageSize);
            ViewBag.Page = page;
            ViewBag.Search = search;

            return View(pagedFoods);
        }

        // GET: Hiển thị form tạo món ăn mới
       /* public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetCategoriesAsync() ?? new List<Category>();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            return View(new Food { DinerId = FakeDinerId });
        }*/


        // POST: Xử lý tạo món ăn mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Food food, IFormFile MainImage, IFormFile Image1, IFormFile Image2)
        {
             
                food.DinerId = FakeDinerId;
                // Lưu hình ảnh và gán đường dẫn
                food.MainImage = await SaveImageAsync(MainImage);
                food.Image1 = await SaveImageAsync(Image1);
                food.Image2 = await SaveImageAsync(Image2);

                bool result = await _foodService.CreateFoodAsync(food);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo món ăn.");
             
            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "CategoryName");
            return View(food);
        }

        // GET: Hiển thị form chỉnh sửa món ăn
        public async Task<IActionResult> Edit(int id)
        {
            var food = await _foodService.GetFoodByIdAsync(id);
            if (food == null || food.DinerId != FakeDinerId)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "CategoryName");
            return View(food);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Food food, IFormFile MainImage, IFormFile Image1, IFormFile Image2)
        {
            var existingFood = await _foodService.GetFoodByIdAsync(id);
            if (existingFood == null || existingFood.DinerId != FakeDinerId)
            {
                return NotFound();
            }
            
                existingFood.FoodName = food.FoodName;
                existingFood.Price = food.Price;
                existingFood.Description = food.Description;
                existingFood.CategoryId = food.CategoryId;

                // Nếu có ảnh mới, cập nhật lại
                if (MainImage != null)
                {
                    existingFood.MainImage = await SaveImageAsync(MainImage);
                }
                if (Image1 != null)
                {
                    existingFood.Image1 = await SaveImageAsync(Image1);
                }
                if (Image2 != null)
                {
                    existingFood.Image2 = await SaveImageAsync(Image2);
                }

                // Nếu API yêu cầu các trường khác (ví dụ Status), bạn có thể gán mặc định:
                if (string.IsNullOrEmpty(existingFood.Status))
                {
                    existingFood.Status = "Active"; // hoặc giá trị mặc định khác
                }

                bool result = await _foodService.UpdateFoodAsync(id, existingFood);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật món ăn.");
            
            // Đồng nhất tên thuộc tính cho danh mục
            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "CategoryName");
            return View(food);
        }


        // GET: Hiển thị trang xác nhận xóa món ăn
        public async Task<IActionResult> Delete(int id)
        {
            var food = await _foodService.GetFoodByIdAsync(id);
            if (food == null || food.DinerId != FakeDinerId)
            {
                return NotFound();
            }
            return View(food);
        }

        // POST: Xác nhận xóa món ăn
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool result = await _foodService.DeleteFoodAsync(id);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // Hàm lưu ảnh vào thư mục wwwroot/images và trả về đường dẫn
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            // Xác định đường dẫn thư mục lưu ảnh
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            // Nếu thư mục chưa tồn tại, tạo mới
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Tạo tên file độc nhất
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                // Bạn có thể log ex.Message hoặc xử lý lỗi tùy ý
                throw new Exception("Lỗi khi lưu file ảnh: " + ex.Message);
            }

            // Trả về đường dẫn ảnh tương đối dùng cho client (web)
            return "/images/" + fileName;
        }

    }
}

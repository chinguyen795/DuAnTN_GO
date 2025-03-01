using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DuAnTN.Controllers
{
    [Area("Saller")]
    public class QL_ProductsController : Controller
    {
        private readonly FoodService _foodService;
        private readonly CategoryService _categoryService;
        private readonly DinerService _dinerService;

        public QL_ProductsController(FoodService foodService, CategoryService categoryService, DinerService dinerService)
        {
            _foodService = foodService;
            _categoryService = categoryService;
            _dinerService = dinerService;
        }

        // 🔹 Lấy danh sách cửa hàng của User đăng nhập
        private async Task<List<Diner>> GetUserDinersAsync()
        {
            var userId = HttpContext.Session.GetString("Id");

            if (string.IsNullOrEmpty(userId))
            {
                return new List<Diner>(); // Trả về danh sách rỗng nếu không có userId
            }

            return await _dinerService.GetDinersByUserIdAsync(int.Parse(userId));
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var userId = HttpContext.Session.GetString("Id");

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastMessage"] = "❌ Vui lòng đăng nhập để xem thông tin!";
                return RedirectToAction("Index", "Home");
            }

            var diners = await GetUserDinersAsync();
            ViewBag.Diners = diners;

            if (diners == null || diners.Count == 0)
            {
                TempData["ToastMessage"] = "⚠ Bạn chưa có cửa hàng nào!";
                return View(new List<Food>()); // Trả về danh sách rỗng để tránh lỗi View
            }

            var dinerIds = diners.Select(d => d.Id).ToList();
            var foods = await _foodService.GetFoodsByDinerIdsAsync(dinerIds);

            if (!string.IsNullOrEmpty(search))
            {
                foods = foods.Where(f => f.FoodName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            int pageSize = 10;
            var pagedFoods = foods.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(foods.Count / (double)pageSize);
            ViewBag.Page = page;
            ViewBag.Search = search;

            return View(pagedFoods); // 🔹 Truyền danh sách món ăn sang View
        }


        // 🔹 GET: Hiển thị form tạo món ăn mới
        public async Task<IActionResult> Create()
        {
            var userId = HttpContext.Session.GetString("Id");

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastMessage"] = "❌ Vui lòng đăng nhập trước khi thêm món ăn!";
                return RedirectToAction("Index", "Home");
            }

            var diners = await GetUserDinersAsync();
            if (!diners.Any())
            {
                TempData["ToastMessage"] = "⚠ Bạn chưa có cửa hàng nào để thêm món ăn!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Diners = new SelectList(diners, "Id", "DinerName");
            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "CategoryName");
            return View();
        }
        // 🔹 GET: Hiển thị trang xác nhận xóa món ăn
        public async Task<IActionResult> Delete(int id)
        {
            var food = await _foodService.GetFoodByIdAsync(id);
            if (food == null)
            {
                return NotFound();
            }
            return View(food); // 🔹 Trả về View xác nhận xóa
        }

        // 🔹 POST: Xác nhận xóa món ăn
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool result = await _foodService.DeleteFoodAsync(id);
            if (result)
            {
                return RedirectToAction(nameof(Index)); // 🔹 Xóa thành công, quay lại danh sách
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Food food, IFormFile MainImage, IFormFile Image1, IFormFile Image2)
        {
            var userId = HttpContext.Session.GetString("Id");

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ToastMessage"] = "❌ Vui lòng đăng nhập trước khi thêm món ăn!";
                return RedirectToAction("Index", "Home");
            }

            var diners = await GetUserDinersAsync();
            if (!diners.Any(d => d.Id == food.DinerId))
            {
                ModelState.AddModelError("", "Bạn không có quyền tạo món ăn cho cửa hàng này.");
                return View(food);
            }

            // 🔹 Lưu ảnh trước khi gửi dữ liệu lên API
            food.MainImage = await SaveImageAsync(MainImage);
            food.Image1 = await SaveImageAsync(Image1);
            food.Image2 = await SaveImageAsync(Image2);

            bool result = await _foodService.CreateFoodAsync(food);
            if (result)
            {
                TempData["ToastMessage"] = "✅ Tạo món ăn thành công!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "❌ Có lỗi xảy ra khi tạo món ăn.");
            ViewBag.Diners = new SelectList(diners, "Id", "DinerName");
            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "CategoryName");
            return View(food);
        }


        // 🔹 GET: Hiển thị form chỉnh sửa món ăn
        public async Task<IActionResult> Edit(int id)
        {
            var food = await _foodService.GetFoodByIdAsync(id);
            var diners = await GetUserDinersAsync();
            if (food == null || !diners.Any(d => d.Id == food.DinerId))
            {
                return NotFound();
            }

            ViewBag.Diners = new SelectList(diners, "Id", "DinerName", food.DinerId);
            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "CategoryName", food.CategoryId);
            return View(food);
        }

        // 🔹 POST: Xử lý chỉnh sửa món ăn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Food food, IFormFile MainImage, IFormFile Image1, IFormFile Image2)
        {
            var existingFood = await _foodService.GetFoodByIdAsync(id);
            var diners = await GetUserDinersAsync();

            if (existingFood == null || !diners.Any(d => d.Id == existingFood.DinerId))
            {
                return NotFound();
            }

            // 🔹 Cập nhật thông tin món ăn
            existingFood.FoodName = food.FoodName;
            existingFood.Price = food.Price;
            existingFood.Description = food.Description;
            existingFood.CategoryId = food.CategoryId;// 🔹 Cập nhật ảnh mới nếu có
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

            bool result = await _foodService.UpdateFoodAsync(id, existingFood);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật món ăn.");
            ViewBag.Diners = new SelectList(diners, "Id", "DinerName", food.DinerId);
            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "CategoryName", food.CategoryId);
            return View(food);
        }
        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

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
                throw new Exception("Lỗi khi lưu file ảnh: " + ex.Message);
            }

            return "/images/" + fileName;
        }
    }
}
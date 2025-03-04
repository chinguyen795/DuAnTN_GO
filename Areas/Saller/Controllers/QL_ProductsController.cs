using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
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

        private async Task<Diner?> GetUserDinerAsync()
        {
            var userId = HttpContext.Session.GetString("Id");

            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            return await _dinerService.GetDinerByUserIdAsync(int.Parse(userId));
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var diner = await GetUserDinerAsync();

            if (diner == null)
            {
                TempData["ToastMessage"] = "⚠ Bạn chưa có cửa hàng!";
                return RedirectToAction("Index", "Home");
            }

            var foods = await _foodService.GetFoodsByDinerIdAsync(diner.Id);

            if (!string.IsNullOrEmpty(search))
            {
                foods = foods.Where(f => f.FoodName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            int pageSize = 10;
            var pagedFoods = foods.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(foods.Count / (double)pageSize);
            ViewBag.Page = page;
            ViewBag.Search = search;

            return View(pagedFoods);
        }

        public async Task<IActionResult> Create()
        {
            var diner = await GetUserDinerAsync();
            if (diner == null)
            {
                TempData["ToastMessage"] = "⚠ Bạn chưa có cửa hàng!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "CategoryName");
            return View(new Food
            {
                DinerId = diner.Id,
                FoodName = string.Empty,
                MainImage = "/images/default.png",
                Status = "Active"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Food food, IFormFile MainImage, IFormFile Image1, IFormFile Image2)
        {
            var diner = await GetUserDinerAsync();
            if (diner == null)
            {
                TempData["ToastMessage"] = "⚠ Bạn chưa có cửa hàng!";
                return RedirectToAction(nameof(Index));
            }

            food.DinerId = diner.Id;

            food.MainImage = MainImage != null ? await SaveImageAsync(MainImage) : "/images/default.png";
            food.Image1 = Image1 != null ? await SaveImageAsync(Image1) : "/images/default.png";
            food.Image2 = Image2 != null ? await SaveImageAsync(Image2) : "/images/default.png";

            food.Status = string.IsNullOrEmpty(food.Status) ? "Active" : food.Status;

            bool result = await _foodService.CreateFoodAsync(food);
            if (result)
            {
                TempData["ToastMessage"] = "✅ Tạo món ăn thành công!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "❌ Có lỗi xảy ra khi tạo món ăn.");
            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "CategoryName");
            return View(food);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var food = await _foodService.GetFoodByIdAsync(id);
            var diner = await GetUserDinerAsync();

            if (food == null || diner == null || food.DinerId != diner.Id)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "CategoryName", food.CategoryId);
            return View(food);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Food food, IFormFile MainImage, IFormFile Image1, IFormFile Image2)
        {
            var existingFood = await _foodService.GetFoodByIdAsync(id);
            var diner = await GetUserDinerAsync();

            if (existingFood == null || diner == null || existingFood.DinerId != diner.Id)
            {
                return NotFound();
            }

            // ✅ Cập nhật thông tin món ăn
            existingFood.FoodName = food.FoodName;
            existingFood.Price = food.Price;
            existingFood.Description = food.Description;
            existingFood.CategoryId = food.CategoryId;
            existingFood.Status = string.IsNullOrEmpty(food.Status) ? "Active" : food.Status;

            // ✅ Cập nhật ảnh nếu có
            if (MainImage != null) existingFood.MainImage = await SaveImageAsync(MainImage);
            if (Image1 != null) existingFood.Image1 = await SaveImageAsync(Image1);
            if (Image2 != null) existingFood.Image2 = await SaveImageAsync(Image2);

            bool result = await _foodService.UpdateFoodAsync(id, existingFood);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật món ăn.");
            ViewBag.Categories = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "CategoryName", food.CategoryId);
            return View(food);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var food = await _foodService.GetFoodByIdAsync(id);
            if (food == null)
            {
                return NotFound();
            }
            return View(food); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool result = await _foodService.DeleteFoodAsync(id);
            if (result)
            {
                TempData["ToastMessage"] = "✅ Xóa món ăn thành công!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ToastMessage"] = "❌ Xóa thất bại!";
            return RedirectToAction(nameof(Index));
        }


        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return "/images/default.png";

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return "/images/" + fileName;
        }
    }
}

using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Mvc;

namespace DuAnTN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly FoodService _foodService;
        private readonly CategoryService _categoryService;
        private readonly DinerService _dinerService;
        private const int PageSize = 10;

        public ProductsController(FoodService foodService, CategoryService categoryService, DinerService dinerService)
        {
            _foodService = foodService;
            _categoryService = categoryService;
            _dinerService = dinerService;
        }

        public async Task<IActionResult> Index(string search, int page = 1)
        {
            var foods = await _foodService.GetFoodsAsync();

            foreach (var food in foods)
            {
                food.Category = await _categoryService.GetCategoryByIdAsync(food.CategoryId);  // Liên kết thông tin danh mục
            }
            foreach (var food in foods)
            {
                food.Diner = await _dinerService.GetDinerByIdAsync(food.DinerId);  // Liên kết thông tin danh mục
            }
            if (!string.IsNullOrEmpty(search))
            {
                foods = foods.Where(f => f.FoodName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var pagedFoods = foods.Skip((page - 1) * 10).Take(10).ToList(); // Số lượng sản phẩm mỗi trang là 10

            var totalFoods = foods.Count();
            var totalPages = (int)Math.Ceiling(totalFoods / (double)10);

            ViewBag.TotalPages = totalPages;
            ViewBag.Page = page;
            ViewBag.Search = search; 

            return View(pagedFoods);
            return View(foods); 
        }


        public async Task<IActionResult> Details(int id)
        {
            var food = await _foodService.GetFoodByIdAsync(id);
            return View(food);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Food food)
        {
            if (ModelState.IsValid)
            {
                await _foodService.CreateFoodAsync(food);
                return RedirectToAction("Index");
            }
            return View(food);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Food food)
        {
            if (ModelState.IsValid)
            {
                await _foodService.UpdateFoodAsync(id, food);
                return RedirectToAction("Index");
            }
            return View(food);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var food = await _foodService.DeleteFoodAsync(id);
            if (food != null)
            {
                await _foodService.DeleteFoodAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

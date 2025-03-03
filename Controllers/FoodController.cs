using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using DuAnTN.Extensions;
namespace DuAnTN.Controllers
{
    public class FoodController : Controller
    {
        private readonly FoodService _foodService;
        private readonly DinerService _dinerService;

        public FoodController(FoodService foodService, DinerService dinerService)
        {
            _foodService = foodService;
            _dinerService = dinerService;
        }

        public async Task<IActionResult> Food(string sortOrder, decimal? minPrice, decimal? maxPrice)
        {
            var foods = await _foodService.GetFoodsAsync();

            foreach (var food in foods)
            {
                food.Diner = await _dinerService.GetDinerByIdAsync(food.DinerId);
            }

            // Lọc theo khoảng giá
            if (minPrice.HasValue)
            {
                foods = foods.Where(f => f.Price >= minPrice.Value).ToList();
            }
            if (maxPrice.HasValue)
            {
                foods = foods.Where(f => f.Price <= maxPrice.Value).ToList();
            }

            // Sắp xếp theo giá
            switch (sortOrder)
            {
                case "price_asc":
                    foods = foods.OrderBy(f => f.Price).ToList();
                    break;
                case "price_desc":
                    foods = foods.OrderByDescending(f => f.Price).ToList();
                    break;
                default:
                    foods = foods.OrderBy(f => f.FoodName).ToList();
                    break;
            }

            ViewBag.SortOrder = sortOrder;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

            return View(foods);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var food = await _foodService.GetFoodByIdAsync(id); // Trả về một Food duy nhất

            if (food == null)
            {
                return View("NotFound"); // Nếu không tìm thấy sản phẩm, hiển thị trang lỗi
            }

            food.Diner = await _dinerService.GetDinerByIdAsync(food.DinerId);

            return View(food); // Truyền Food duy nhất cho View, bao gồm cả thông tin Diner
        }

        [HttpGet]
        public async Task<IActionResult> UpdateQuantity(int foodId, int quantity)
        {
            var food = await _foodService.GetFoodByIdAsync(foodId);
            if (food == null) return NotFound();

            if (quantity < 1) quantity = 1;

            return Json(new { quantity, totalPrice = quantity * food.Price });
        }


    }
}

using DuAnTN.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DuAnTN.Controllers
{
    public class FoodController : Controller
    {
        private readonly FoodServices _foodService;
        private readonly DinerServices _dinerServices;

        public FoodController(FoodServices foodService, DinerServices dinerServices)
        {
            _foodService = foodService;
            _dinerServices = dinerServices;
        }

        public async Task<IActionResult> Food(string sortOrder)
        {
            var foods = await _foodService.GetFoodsAsync();

            foreach (var food in foods)
            {
                food.Diner = await _dinerServices.GetDiner(food.DinerId);  // Liên kết thông tin danh mục
            }

            
            switch (sortOrder)
            {
                case "price_asc":
                    foods = foods.OrderBy(f => f.Price).ToList(); // Sắp xếp theo giá tăng dần
                    break;
                case "price_desc":
                    foods = foods.OrderByDescending(f => f.Price).ToList(); // Sắp xếp theo giá giảm dần
                    break;
                default:
                    foods = foods.OrderBy(f => f.FoodName).ToList(); // Sắp xếp theo tên mặc định
                    break;
            }

            ViewBag.SortOrder = sortOrder; // Truyền lại giá trị sắp xếp vào ViewBag

            return View(foods);  // Trả lại View với danh sách món ăn đã sắp xếp
        }

        public async Task<IActionResult> Detail(int id)
        {
            var food = await _foodService.GetFoodByIdAsync(id); // Trả về một Food duy nhất

            if (food == null)
            {
                return View("NotFound"); // Nếu không tìm thấy sản phẩm, hiển thị trang lỗi
            }

            food.Diner = await _dinerServices.GetDiner(food.DinerId);

            return View(food); // Truyền Food duy nhất cho View, bao gồm cả thông tin Diner
        }

    }
}

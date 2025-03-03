using DuAnTN.Models;
using DuAnTN.Services;
using Microsoft.AspNetCore.Mvc;
using DuAnTN.Extensions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace DuAnTN.Controllers
{
    public class CartController : Controller
    {
        private readonly FoodService _foodService; // Sử dụng FoodService
        private readonly OrderDetailsService _orderDetailsService;
        public CartController(FoodService foodService , OrderDetailsService orderDetailsService)
        {
            _foodService = foodService;
            _orderDetailsService = orderDetailsService;
        }

        public List<Food> Foods
        {
            get
            {
                var data = HttpContext.Session.Get<List<Food>>("GioHang");
                return data ?? new List<Food>();
            }
        }

        public async Task<IActionResult> Addtocart(int id, int quantity = 1)
        {
            var myCart = Foods;
            var item = myCart.SingleOrDefault(p => p.Id == id);

            if (item == null)
            {
                var food = await _foodService.GetFoodByIdAsync(id);

                if (food != null)
                {
                    item = new Food
                    {
                        Id = food.Id,
                        FoodName = food.FoodName,
                        Price = food.Price,
                        MainImage = food.MainImage,
                        Status = food.Status
                    };

                    // Lưu số lượng vào Session
                    var cartItems = HttpContext.Session.Get<Dictionary<int, int>>("CartQuantity") ?? new Dictionary<int, int>();
                    cartItems[item.Id] = quantity; // Lưu số lượng được chọn từ View
                    HttpContext.Session.Set("CartQuantity", cartItems);

                    myCart.Add(item);
                }
            }
            else
            {
                // Nếu sản phẩm đã có trong giỏ, tăng số lượng
                var cartItems = HttpContext.Session.Get<Dictionary<int, int>>("CartQuantity") ?? new Dictionary<int, int>();
                if (cartItems.ContainsKey(item.Id))
                {
                    cartItems[item.Id] += quantity; // Cộng thêm số lượng được chọn
                }
                else
                {
                    cartItems[item.Id] = quantity;
                }
                HttpContext.Session.Set("CartQuantity", cartItems);
            }

            HttpContext.Session.Set("GioHang", myCart);
            return RedirectToAction("Index");
        }


        public IActionResult Index()
        {
            // Lấy danh sách sản phẩm từ Session
            var myCart = HttpContext.Session.Get<List<Food>>("GioHang") ?? new List<Food>();

            return View(myCart); // Truyền danh sách sản phẩm vào View
        }

        public IActionResult Pay()
        {
            return View();
        }
    }
}

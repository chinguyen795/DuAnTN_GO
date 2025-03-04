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
        private readonly FoodService _foodService;
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
            var myCart = HttpContext.Session.Get<List<Food>>("GioHang") ?? new List<Food>();
            var cartItems = HttpContext.Session.Get<Dictionary<int, int>>("CartQuantity") ?? new Dictionary<int, int>();

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

                    myCart.Add(item);
                }
            }

            // Cập nhật số lượng vào Session
            if (cartItems.ContainsKey(id))
            {
                cartItems[id] += quantity; 
            }
            else
            {
                cartItems[id] = quantity;
            }

            HttpContext.Session.Set("GioHang", myCart);
            HttpContext.Session.Set("CartQuantity", cartItems);

            return RedirectToAction("Index");
        }


        public IActionResult Index()
        {
            // Lấy danh sách sản phẩm từ Session
            var myCart = HttpContext.Session.Get<List<Food>>("GioHang") ?? new List<Food>();

            return View(myCart);
        }

        public IActionResult Pay()
        {
            return View();
        }
    }
}

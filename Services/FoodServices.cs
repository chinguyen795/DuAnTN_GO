using DuAnTN.Models;
using Newtonsoft.Json;
using System.Text;

namespace DuAnTN.Services
{
    public class FoodServices
    {
        private readonly HttpClient _httpClient; // Phương thức GET/POST/PUT/DELETE
        private readonly string _apiUrl = "https://localhost:7248/api/Foods"; // biến chứa URL (API)

        // Hàm tạo
        public FoodServices()
        {
            _httpClient = new HttpClient();
        }
        // Lấy tất cả thực phẩm
        public async Task<List<Food>> GetFoodsAsync()
        {
            var response = await _httpClient.GetStringAsync(_apiUrl);
            return JsonConvert.DeserializeObject<List<Food>>(response);
        }

        // Lấy thực phẩm theo Id
        public async Task<Food> GetFoodByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            return JsonConvert.DeserializeObject<Food>(response);
        }

        public async Task<bool> PostFood(Food food)
        {
            var json = JsonConvert.SerializeObject(food);
            Console.WriteLine("Dữ liệu gửi lên API: " + json);  // Kiểm tra JSON gửi đi
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);

            Console.WriteLine("Trạng thái phản hồi: " + response.StatusCode); // Kiểm tra response API

            return response.IsSuccessStatusCode;
        }
    }
}

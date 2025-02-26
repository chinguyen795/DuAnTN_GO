using DuAnTN.Models;
using Newtonsoft.Json;
using System.Text;

namespace DuAnTN.Services
{
    public class FoodService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7248/api/Foods"; // API của bạn

        public FoodService()
        {
            _httpClient = new HttpClient();
        }

        // Lấy danh sách tất cả món ăn
        public async Task<List<Food>> GetFoodsAsync()
        {
            var response = await _httpClient.GetStringAsync(_apiUrl);
            return JsonConvert.DeserializeObject<List<Food>>(response);
        }

        // Lấy món ăn theo ID
        public async Task<Food> GetFoodByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            return JsonConvert.DeserializeObject<Food>(response);
        }

        public async Task<List<Food>> GetFoodsByDinerIdAsync(int dinerId)
        {
            // Ép luôn dinerId = 1
            dinerId = 1;
            var response = await _httpClient.GetStringAsync($"{_apiUrl}?dinerId={dinerId}");
            return JsonConvert.DeserializeObject<List<Food>>(response);
        }



        // Tạo món ăn mới
        public async Task<bool> PostFood(Food food)
        {
            var json = JsonConvert.SerializeObject(food);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);
            return response.IsSuccessStatusCode;
        }

        // Cập nhật món ăn
        public async Task<bool> UpdateFoodAsync(int id, Food food)
        {
            var json = JsonConvert.SerializeObject(food);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
            return response.IsSuccessStatusCode;
        }

        // Xóa món ăn
        public async Task<bool> DeleteFoodAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}

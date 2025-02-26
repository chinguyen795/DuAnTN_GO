using DuAnTN.Models;
using Newtonsoft.Json;
using System.Text;

namespace DuAnTN.Services
{
    public class FoodService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7248/api/Foods";  // API cho Food

        public FoodService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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

        // Tạo thực phẩm mới
        public async Task<bool> CreateFoodAsync(Food food)
        {
            var json = JsonConvert.SerializeObject(food);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);
            return response.IsSuccessStatusCode;
        }

        // Cập nhật thông tin thực phẩm
        public async Task<bool> UpdateFoodAsync(int id, Food food)
        {
            var json = JsonConvert.SerializeObject(food);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
            return response.IsSuccessStatusCode;
        }
        // Xóa thực phẩm
        public async Task<bool> DeleteFoodAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        // Lấy danh sách thực phẩm theo CategoryId
        public async Task<List<Food>> GetFoodsByCategoryAsync(int categoryId)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/Category/{categoryId}");
            return JsonConvert.DeserializeObject<List<Food>>(response);
        }
    }
}

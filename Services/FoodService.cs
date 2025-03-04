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

        public async Task<List<Food>> GetFoodsByDinerIdAsync(int dinerId)
        {
            // Ép luôn dinerId = 1
            dinerId = 1;
            var response = await _httpClient.GetStringAsync($"{_apiUrl}?dinerId={dinerId}");
            return JsonConvert.DeserializeObject<List<Food>>(response);
        }

        // Lấy danh sách món ăn của nhiều cửa hàng (DinerIds)
        public async Task<List<Food>> GetFoodsByDinerIdsAsync(List<int> dinerIds)
        {
            if (dinerIds == null || !dinerIds.Any()) return new List<Food>();

            try
            {
                // Chuyển danh sách `DinerId` thành query string dạng `?dinerIds=1&dinerIds=2`
                var query = string.Join("&dinerIds=", dinerIds);
                var requestUrl = $"{_apiUrl}/ByDiners?dinerIds={query}";

                var response = await _httpClient.GetAsync(requestUrl);
                if (!response.IsSuccessStatusCode) return new List<Food>();

                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Food>>(json) ?? new List<Food>();
            }
            catch
            {
                return new List<Food>();
            }
        }
    }
}

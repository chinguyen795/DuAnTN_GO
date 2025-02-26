using DuAnTN.Models;
using Newtonsoft.Json;
using System.Text;

namespace DuAnTN.Services
{
    public class CategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7248/api/Categories";  // API cho Category

        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetStringAsync(_apiUrl);
            return JsonConvert.DeserializeObject<List<Category>>(response);
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            return JsonConvert.DeserializeObject<Category>(response);
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            var json = JsonConvert.SerializeObject(category);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateCategoryAsync(int id, Category category)
        {
            var json = JsonConvert.SerializeObject(category);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
        // Lấy danh sách Foods theo CategoryId
        public async Task<List<Food>> GetFoodsByCategoryAsync(int categoryId)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{categoryId}/Foods");
            return JsonConvert.DeserializeObject<List<Food>>(response);
        }
    }

}

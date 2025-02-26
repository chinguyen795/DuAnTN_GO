using DuAnTN.Models;
using Newtonsoft.Json;
using System.Text;

namespace DuAnTN.Services
{
    public class CategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7248/api/Categories";

        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Lấy danh sách tất cả danh mục
        public async Task<List<Category>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetStringAsync(_apiUrl);
            return JsonConvert.DeserializeObject<List<Category>>(response);
        }

        // Lấy danh mục theo ID
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            return JsonConvert.DeserializeObject<Category>(response);
        }
    }
}

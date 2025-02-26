using DuAnTN.Models;
using Newtonsoft.Json;
using System.Text;

namespace DuAnTN.Services
{
    public class DinerService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7248/api/Diners";  // API cho Diner

        public DinerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Lấy tất cả diner's
        public async Task<List<Diner>> GetDiningsAsync()
        {
            var response = await _httpClient.GetStringAsync(_apiUrl);
            return JsonConvert.DeserializeObject<List<Diner>>(response);
        }

        // Lấy Diner theo Id
        public async Task<Diner> GetDinerByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            return JsonConvert.DeserializeObject<Diner>(response);
        }

        // Tạo Diner mới
        public async Task<bool> CreateDinerAsync(Diner diner)
        {
            var json = JsonConvert.SerializeObject(diner);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);
            return response.IsSuccessStatusCode;
        }

        // Cập nhật thông tin Diner
        public async Task<bool> UpdateDinerAsync(int id, Diner diner)
        {
            var json = JsonConvert.SerializeObject(diner);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
            return response.IsSuccessStatusCode;
        }

        // Xóa Diner
        public async Task<bool> DeleteDinerAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
using DuAnTN.Models;
using Newtonsoft.Json;
using System.Text;

namespace DuAnTN.Services
{
    public class DinerService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7248/api/Diners";

        public DinerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
         
        // Lấy danh sách tất cả Diner
        public async Task<List<Diner>> GetDinersAsync()
        {
            var response = await _httpClient.GetStringAsync(_apiUrl);
            return JsonConvert.DeserializeObject<List<Diner>>(response);
        }

        // Lấy thông tin Diner theo UserId (Seller)
        public async Task<Diner> GetDinerByUserIdAsync(int userId)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/GetByUserId/{userId}");

            return JsonConvert.DeserializeObject<Diner>(response);
        }

        // Lấy Diner theo Id
        public async Task<Diner> GetDinerByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            return JsonConvert.DeserializeObject<Diner>(response);
        }

        // Cập nhật Diner
        public async Task<bool> UpdateDinerAsync(int id, Diner diner)
        {
            var json = JsonConvert.SerializeObject(diner);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
            return response.IsSuccessStatusCode;
        }
    }
}

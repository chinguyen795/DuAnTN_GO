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
        public async Task<Diner?> GetDinerByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return null;  // Trả về null nếu không tìm thấy
            }

            return await response.Content.ReadFromJsonAsync<Diner>();
            }
        public async Task<bool> UpdateDinerAsync(int id, Diner diner)
        {
            if (id == 0)
            {
                Console.WriteLine("Lỗi: ID không hợp lệ!");
                return false;
            }

            var response = await _httpClient.PutAsJsonAsync($"{_apiUrl}/{id}", diner);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Lỗi cập nhật Diner {id}: {response.StatusCode} - {error}");
                return false;
            }

            return true;
        }


        public async Task<List<Diner>> GetDinersByUserIdAsync(int userId)
        {
            var response = await _httpClient.GetFromJsonAsync<List<Diner>>($"{_apiUrl}/user/{userId}");
            return response ?? new List<Diner>();
        }

        // Tạo Diner mới
        public async Task<bool> CreateDinerAsync(Diner diner)
        {
            var json = JsonConvert.SerializeObject(diner);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<Diner?> GetDinerByUserIdAsync(int userId)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/user/{userId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;  // Trả về null nếu không tìm thấy
            }

            return await response.Content.ReadFromJsonAsync<Diner>();
        }


        // Xóa Diner
        public async Task<bool> DeleteDinerAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}

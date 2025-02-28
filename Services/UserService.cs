using DuAnTN.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace DuAnTN.Models
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7248/api/Users";  // API cho User

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Lấy tất cả User's
        public async Task<List<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetStringAsync(_apiUrl);
            return JsonConvert.DeserializeObject<List<User>>(response);
        }

        // Lấy User theo Id
        public async Task<User> GetUserByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            return JsonConvert.DeserializeObject<User>(response);
        }

        // Tạo User mới
        public async Task<bool> CreateUserAsync(User User)
        {
            var json = JsonConvert.SerializeObject(User);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);
            return response.IsSuccessStatusCode;
        }

        // Cập nhật thông tin User
        public async Task<bool> UpdateUserAsync(int id, User User)
        {
            var json = JsonConvert.SerializeObject(User);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
            return response.IsSuccessStatusCode;
        }

        // Xóa User
        public async Task<bool> DeleteUserAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/CheckEmail?email={email}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return bool.Parse(result); // API trả về true nếu email tồn tại
            }

            return false; // Mặc định trả về false nếu có lỗi
        }


    }
}
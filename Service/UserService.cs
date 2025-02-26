using DuAnTN.Models;
using Newtonsoft.Json;
using System.Text;

namespace DuAnTN.Models
{
    public class UserService
    {
        private readonly HttpClient _httpClient; // Phương thức GET/POST/PUT/DELETE
        private readonly string _apiUrl = "https://localhost:7248/api/Users"; // URL API

        // Hàm tạo
        public UserService()
        {
            _httpClient = new HttpClient();
        }

        // Lấy danh sách User
        public async Task<List<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetStringAsync(_apiUrl);
            var users = JsonConvert.DeserializeObject<List<User>>(response);

            // Kiểm tra và hiển thị thông tin UserInfo của mỗi User
            foreach (var user in users)
            {
                if (user.UserInfo != null)
                {
                    Console.WriteLine($"Thông tin UserInfo của user {user.Email}: {user.UserInfo.Address}");
                }
            }

            return users;
        }

        // Lấy User theo ID
        public async Task<User> GetUserByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            var user = JsonConvert.DeserializeObject<User>(response);

            // Kiểm tra và hiển thị thông tin UserInfo của User
            if (user.UserInfo != null)
            {
                Console.WriteLine($"Thông tin UserInfo của user {user.Email}: {user.UserInfo.Address}");
            }

            return user;
        }

        // Tạo mới User
        public async Task CreateUserAsync(User user)
        {
            var json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await _httpClient.PostAsync(_apiUrl, content);
        }

        // Cập nhật User
        public async Task UpdateUserAsync(User user)
        {
            var json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await _httpClient.PutAsync($"{_apiUrl}/{user.Id}", content);
        }

        // Xóa User
        public async Task DeleteUserAsync(int id)
        {
            await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
        }
    }
}
using DuAnTN.Models;
using Newtonsoft.Json;
using System.Text;

namespace DuAnTN.Services
{
    public class UserServices
    {
        private readonly HttpClient _httpClient; // Phương thức GET/POST/PUT/DELETE
        private readonly string _apiUrl = "https://localhost:7248/api/Users"; // biến chứa URL (API)

        // Hàm tạo
        public UserServices()
        {
            _httpClient = new HttpClient();
        }
        // Lấy tất cả thực phẩm
        public async Task<List<User>> GetUsers()
        {
            var response = await _httpClient.GetStringAsync(_apiUrl);
            return JsonConvert.DeserializeObject<List<User>>(response);
        }

        // Lấy thực phẩm theo Id
        public async Task<User> GetUser(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            return JsonConvert.DeserializeObject<User>(response);
        }

        public async Task<bool> PostUser(User user)
        {
            var json = JsonConvert.SerializeObject(user);
            Console.WriteLine("Dữ liệu gửi lên API: " + json);  // Kiểm tra JSON gửi đi
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);

            Console.WriteLine("Trạng thái phản hồi: " + response.StatusCode); // Kiểm tra response API

            return response.IsSuccessStatusCode;
        }
    }
}

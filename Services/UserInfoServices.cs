using DuAnTN.Models;
using Newtonsoft.Json;
using System.Text;

namespace DuAnTN.Services
{
    public class UserInfoServices
    {
        private readonly HttpClient _httpClient; // Phương thức GET/POST/PUT/DELETE
        private readonly string _apiUrl = "https://localhost:7248/api/UserInfos"; // biến chứa URL (API)

        // Hàm tạo
        public UserInfoServices()
        {
            _httpClient = new HttpClient();
        }
        // Lấy tất cả thực phẩm
        public async Task<List<UserInfo>> GetUserInfosAsync()
        {
            var response = await _httpClient.GetStringAsync(_apiUrl);
            return JsonConvert.DeserializeObject<List<UserInfo>>(response);
        }

        // Lấy thực phẩm theo Id
        public async Task<UserInfo> GetUserInfo(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            return JsonConvert.DeserializeObject<UserInfo>(response);
        }

        public async Task<bool> PostUserInfo(UserInfo userinfor)
        {
            var json = JsonConvert.SerializeObject(userinfor);
            Console.WriteLine("Dữ liệu gửi lên API: " + json);  // Kiểm tra JSON gửi đi
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);

            Console.WriteLine("Trạng thái phản hồi: " + response.StatusCode); // Kiểm tra response API

            return response.IsSuccessStatusCode;
        }
    }
}

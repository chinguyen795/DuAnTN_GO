using DuAnTN.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DuAnTN.Services
{
    public class UserInfoServices
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7248/api/UserInfos"; // Đảm bảo URL API đúng

        // Constructor nhận HttpClient từ DI
        public UserInfoServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Lấy danh sách UserInfo
        public async Task<List<UserInfo>> GetUserInfosAsync()
        {
            var response = await _httpClient.GetStringAsync(_userInfoapiUrl);
            return JsonConvert.DeserializeObject<List<UserInfo>>(response);
        }

        public async Task<UserInfo> GetUserInfo(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_userInfoapiUrl}/{id}");
            return JsonConvert.DeserializeObject<UserInfo>(response);
        }

        public async Task<bool> PostUserInfo(UserInfo userinfor)
        {
            var json = JsonConvert.SerializeObject(userinfor);
            Console.WriteLine("Dữ liệu gửi lên API: " + json);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);

            Console.WriteLine("Trạng thái phản hồi: " + response.StatusCode);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserInfoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}

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
            var response = await _httpClient.GetStringAsync(_apiUrl);
            return JsonConvert.DeserializeObject<List<UserInfo>>(response);
        }

        // Lấy UserInfo theo ID
        public async Task<UserInfo> GetUserInfo(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            return JsonConvert.DeserializeObject<UserInfo>(response);
        }

        // DELETE: Xóa UserInfo theo ID
        public async Task<bool> DeleteUserInfoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        // Cập nhật UserInfo qua API
        public async Task<bool> UpdateUserInfoAsync(UserInfo userInfo)
        {
            try
            {
                // Serialize object UserInfo thành JSON
                var json = JsonConvert.SerializeObject(userInfo);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi PUT request đến API để cập nhật thông tin người dùng
                var response = await _httpClient.PutAsync($"{_apiUrl}/{userInfo.Id}", content);

                return response.IsSuccessStatusCode; // Kiểm tra nếu yêu cầu thành công
            }
            catch (Exception ex)
            {
                // Log lỗi (nếu có) và trả về false nếu gặp lỗi
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}

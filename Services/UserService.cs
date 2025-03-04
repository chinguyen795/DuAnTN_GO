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
 private readonly ILogger<UserService> _logger; // Thêm đối tượng ILogger
        private readonly string _roleApiUrl = "https://localhost:7248/api/Roles";
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

       

        // Cập nhật constructor để nhận ILogger
        public UserService(HttpClient httpClient, ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;  // Khởi tạo _logger
        }
        public async Task<List<User>> GetUsersAsync()
        {
            var response = await _httpClient.GetStringAsync(_apiUrl);
            var users = JsonConvert.DeserializeObject<List<User>>(response) ?? new List<User>();

            var roleResponse = await _httpClient.GetStringAsync(_roleApiUrl);
            var roles = JsonConvert.DeserializeObject<List<Role>>(roleResponse) ?? new List<Role>();

            foreach (var user in users)
            {
                var role = roles.FirstOrDefault(c => c.Id == user.RoleId);
                user.role = role ?? new Role { RoleName = "Không xác định" };
            }
            return users;
        }

        // Lấy User theo Id
        public async Task<User> GetUserByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            return JsonConvert.DeserializeObject<User>(response);
        }

        // Tạo User mới
        public async Task<bool> CreateUserAsync(User category)
        {
            var json = JsonConvert.SerializeObject(category);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUserAsync(int id, User user)
        {
            var json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Lỗi cập nhật User (ID: {id}): {responseContent}");
                return false;
            }

            _logger.LogInformation($"User (ID: {id}) cập nhật thành công: {responseContent}");
            return true;
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
                return bool.Parse(result);
            }

            return false; 
        }

        // Phương thức đổi mật khẩu
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var payload = new
            {
                UserId = userId,
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7248/api/Users/ChangePassword", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SendVerificationCodeAsync(string email)
        {
            var content = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiUrl}/SendVerificationCode", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> VerifyCodeAsync(string email, string code)
        {
            var request = new { Email = email, Code = code };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiUrl}/VerifyCode", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RegisterUserAsync(User user)
        {
            var json = JsonConvert.SerializeObject(user);  // Serialize đối tượng User
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiUrl}/Register", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();  // Đọc nội dung lỗi từ API
                _logger.LogError($"Error during registration: {errorContent}");
                return false;
            }

            return response.IsSuccessStatusCode;
        }
        // Gửi mã xác thực quên mật khẩu
        public async Task<bool> SendResetPasswordOTPAsync(string email)
        {
            var content = new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiUrl}/SendResetPasswordOTP", content);
            return response.IsSuccessStatusCode;
        }

        // Xác minh mã OTP quên mật khẩu
        public async Task<bool> VerifyResetPasswordOTPAsync(string email, string code)
        {
            var request = new { Email = email, Code = code };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_apiUrl}/VerifyResetPasswordOTP", content);
            return response.IsSuccessStatusCode;
        }
        public async Task<List<Role>> GetRoleAsync()
        {
            var response = await _httpClient.GetStringAsync(_roleApiUrl);
            return JsonConvert.DeserializeObject<List<Role>>(response) ?? new List<Role>();
        }
        public async Task<bool> ChangeUserRoleAsync(int userId, int newRoleId)
        {
            var payload = new { UserId = userId, NewRoleId = newRoleId };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiUrl}/ChangeRole", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"❌ Lỗi khi thay đổi RoleId của User (ID: {userId}): {responseContent}");
                return false;
            }

            _logger.LogInformation($"✅ RoleId của User (ID: {userId}) đã được cập nhật thành {newRoleId}");
            return true;
        }

    }
}
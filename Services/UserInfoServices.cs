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
        private readonly string _userInfoapiUrl = "https://localhost:7248/api/UserInfos";

        // Constructor nhận HttpClient từ DI
        public UserInfoServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UserInfo>> GetUserInfosAsync()
        {
            var response = await _httpClient.GetStringAsync(_userInfoapiUrl);
            return JsonConvert.DeserializeObject<List<UserInfo>>(response);
        }

        public async Task<UserInfo> GetUserInfoByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_userInfoapiUrl}/{id}");
            return JsonConvert.DeserializeObject<UserInfo>(response);
        }

        public async Task<bool> CreateUserInfoAsync(UserInfo usif)
        {
            var json = JsonConvert.SerializeObject(usif);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_userInfoapiUrl, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateUserInfoAsync(int id, UserInfo usif)
        {
            var json = JsonConvert.SerializeObject(usif);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_userInfoapiUrl}/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserInfoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_userInfoapiUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}

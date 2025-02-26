using System.Net.Http;
using System.Threading.Tasks;
using DuAnTN.Models;
using Newtonsoft.Json;

namespace DuAnTN.Service
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserInfo> GetUserInfoByIdAsync(int userId)
        {
            // Call API to get user info
            var response = await _httpClient.GetAsync($"https://yourapi.com/api/userinfo/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var userInfo = JsonConvert.DeserializeObject<UserInfo>(content);
                return userInfo;
            }
            else
            {
                return null;  // Handle error if API call fails
            }
        }
    }
}

using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace DuAnTN.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> Login(string email, string password)
        {
            var loginRequest = new { Email = email, Password = password };
            var jsonRequest = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7248/api/Users/login", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"✅ API trả về: {responseContent}");

            try
            {
                var result = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                return result?.Token;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public class LoginResponse
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
            public string Token { get; set; }
        }


        public void SetAuthHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}

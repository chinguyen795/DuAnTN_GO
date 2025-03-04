using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace DuAnTN.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string?> Login(string email, string password)
        {
            var loginRequest = new { Email = email, Password = password };
            var jsonRequest = JsonConvert.SerializeObject(loginRequest);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://localhost:7248/api/Users/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<LoginResponse>(responseContent);

                if (result == null || string.IsNullOrEmpty(result.Token))
                {
                    return null;
                }

                // Lưu token vào session
                var session = _httpContextAccessor.HttpContext.Session;
                session.SetString("Token", result.Token);
                session.SetString("Id", result.Id.ToString());
                session.SetString("FullName", result.FullName ?? "");
                session.SetString("Role", result.Role ?? "User");
                session.SetString("RoleID", result.RoleID.ToString());

                SetAuthHeader(result.Token);
                return result.Token;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void SetAuthHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public class LoginResponse
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
            public int RoleID { get; set; }
            public string Token { get; set; }
        }
    }
}

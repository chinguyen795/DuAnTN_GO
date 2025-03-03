using DuAnTN.Models;
using Newtonsoft.Json;
using System.Text;

namespace DuAnTN.Services
{
    public class OrderDetailsService
    {

            private readonly HttpClient _httpClient;
            private readonly string _apiUrl = "https://localhost:7248/api/OrderDetails"; 

            public OrderDetailsService(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            public async Task<List<OrderDetails>> GetOrderDetailsAsync()
            {
                var response = await _httpClient.GetStringAsync(_apiUrl);
                return JsonConvert.DeserializeObject<List<OrderDetails>>(response);
            }

            public async Task<OrderDetails> GetOrderDetailsByIdAsync(int id)
            {
                var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
                return JsonConvert.DeserializeObject<OrderDetails>(response);
            }

            public async Task<bool> CreateOrderDetailsAsync(OrderDetails OrderDetails)
            {
                var json = JsonConvert.SerializeObject(OrderDetails);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_apiUrl, content);
                return response.IsSuccessStatusCode;
            }

            public async Task<bool> UpdateOrderDetailsAsync(int id, OrderDetails OrderDetails)
            {
                var json = JsonConvert.SerializeObject(OrderDetails);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
                return response.IsSuccessStatusCode;
            }

            public async Task<bool> DeleteOrderDetailsAsync(int id)
            {
                var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
                return response.IsSuccessStatusCode;
            }

        public async Task<List<OrderDetails>> GetOrderDetailsByFoodIdAsync(int foodId)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/food/{foodId}");
            return JsonConvert.DeserializeObject<List<OrderDetails>>(response) ?? new List<OrderDetails>();
        }

    }
}

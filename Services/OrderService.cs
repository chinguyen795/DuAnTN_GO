using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using DuAnTN.Models;

public class OrderService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl = "https://localhost:7248/api/orders";

    public OrderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // 1. Lấy danh sách đơn hàng
    public async Task<List<Order>> GetOrdersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Order>>(_apiUrl) ?? new List<Order>();
    }

    public class OrderCountResponse
    {
        public int Pending { get; set; }
        public int Processed { get; set; }
        public int Canceled { get; set; }
    }

    public async Task<OrderCountResponse> GetOrderCountsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<OrderCountResponse>($"{_apiUrl}/count");
        if (response == null)
        {
            throw new Exception("API trả về dữ liệu rỗng.");
        }
        return response;
    }


    // 3. Lấy thống kê doanh số, truy cập, đơn hàng
    public async Task<dynamic> GetOrderStatisticsAsync()
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/statistics");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"API Error: {response.StatusCode} - {error}");
        }

        return await response.Content.ReadFromJsonAsync<dynamic>();
    }

}

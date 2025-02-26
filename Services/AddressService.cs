using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DuAnTN.Models;
using System.Collections.Generic;
using System.Text;

namespace DuAnTN.Services
{
    public class AddressService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7248/api/address"; // API Address

        public AddressService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Lấy danh sách địa chỉ từ API
        public async Task<List<AddressModel>> GetAddressesAsync()
        {
            var response = await _httpClient.GetAsync(_apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<AddressModel>>(content);
            }

            return new List<AddressModel>(); // Trả về danh sách trống nếu có lỗi
        }

        // Thêm địa chỉ mới vào API
        public async Task<bool> AddAddressAsync(AddressModel address)
        {
            var json = JsonConvert.SerializeObject(address);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiUrl, content);
            return response.IsSuccessStatusCode;
        }

        // Xóa địa chỉ
        public async Task<bool> DeleteAddressAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return response.IsSuccessStatusCode;  // Trả về true nếu xóa thành công
        }

        // Lấy địa chỉ theo ID từ API
        public async Task<AddressModel> GetAddressByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AddressModel>(content); // Trả về địa chỉ tìm thấy
            }

            return null; // Nếu không tìm thấy, trả về null
        }

        // Cập nhật địa chỉ
        public async Task<bool> UpdateAddressAsync(AddressModel address)
        {
            var json = JsonConvert.SerializeObject(address);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_apiUrl}/{address.Id}", content);
            return response.IsSuccessStatusCode; // Trả về true nếu cập nhật thành công
        }
    }
}

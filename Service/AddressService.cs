using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DuAnTN.Models;
using System.Collections.Generic;

namespace DuAnTN.Services
{
    public class AddressService
    {
        private readonly HttpClient _httpClient;

        public AddressService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Lấy danh sách địa chỉ từ API
        public async Task<List<AddressModel>> GetAddressesAsync()
        {
            var response = await _httpClient.GetAsync("https://localhost:7248/api/address");

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
            var response = await _httpClient.PostAsJsonAsync("https://localhost:7248/api/address", address);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> DeleteAddressAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"https://localhost:7248/api/address/{id}");
            return response.IsSuccessStatusCode;  // Trả về true nếu xóa thành công
        }


        // Lấy địa chỉ theo ID từ API
        public async Task<AddressModel> GetAddressByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7248/api/address/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AddressModel>(content); // Trả về địa chỉ tìm thấy
            }

            return null; // Nếu không tìm thấy, trả về null
        }
        public async Task<bool> UpdateAddressAsync(AddressModel address)
        {
            var response = await _httpClient.PutAsJsonAsync($"https://localhost:7248/api/address/{address.Id}", address);
            return response.IsSuccessStatusCode; // Trả về true nếu cập nhật thành công
        }
        // Lấy thông tin người dùng theo ID
        public async Task<UserInfo> GetUserInfoByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7248/api/userinfo/{id}");  // Lấy thông tin từ API

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserInfo>(content);  // Trả về thông tin UserInfo
            }

            return null;  // Nếu không tìm thấy người dùng
        }

    }
}

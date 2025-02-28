using DuAnTN.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;  // Hoặc namespace chứa AddressModel

namespace DuAnTN.Models

{
    public class Address
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên địa chỉ không được để trống.")]
        [MaxLength(255, ErrorMessage = "Tên địa chỉ không được vượt quá 255 ký tự.")]
        public string NameAddress { get; set; }
        public string Description { get; set; }
        // Khóa ngoại liên kết với User (1-n)
        public int UserId { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }
    }
}

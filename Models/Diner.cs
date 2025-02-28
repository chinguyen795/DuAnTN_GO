using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DuAnTN.Models
{
    public class Diner
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên quán ăn không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên quán ăn không được vượt quá 150 ký tự.")]
        public required string DinerName { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        [MaxLength(250, ErrorMessage = "Địa chỉ không được vượt quá 250 ký tự.")]
        public required string DinerAddress { get; set; }

        [Required(ErrorMessage = "Ảnh chính không được để trống.")]
        public required string MainImage { get; set; }

        public string? Image1 { get; set; }
        public string? Image2 { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 chữ số.")]
        public required string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Mã số thuế không được để trống.")]
        [RegularExpression(@"^\d{10,13}$", ErrorMessage = "Mã số thuế phải có từ 10 đến 13 chữ số.")]
        public required string TaxCode { get; set; }

        // Thời gian tạo
        public DateTime CreateAt { get; set; } = DateTime.Now;

        // Khóa ngoại đến User (1-n)
        public int UserId { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }

        // 1-n với Food
        [JsonIgnore]
        public virtual ICollection<Food> Foods { get; set; } = new HashSet<Food>();
    }

}
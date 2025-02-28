using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DuAnTN.Models
{
    public class UserInfo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [MinLength(3, ErrorMessage = "Họ và tên không được ít hơn 3 ký tự.")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Giới tính không được để trống.")]
        [RegularExpression(@"^(Nam|Nữ)$", ErrorMessage = "Giới tính chỉ được là 'Nam' hoặc 'Nữ'.")]
        public required string Gender { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải có đúng 10 chữ số.")]
        public required string Phone { get; set; }


        public DateTime BirthDay { get; set; }

        public string? IdentityImageCard { get; set; }

        [Required(ErrorMessage = "Số căn cước công dân không được để trống.")]
        [RegularExpression(@"^\d{9}$|^\d{12}$", ErrorMessage = "Số căn cước công dân phải có 9 hoặc 12 chữ số.")]
        public required string IdentityCard { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;

        public string? Avatar { get; set; }

        // Khóa ngoại đến User (1-1)
        public int UserId { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }
    }

}

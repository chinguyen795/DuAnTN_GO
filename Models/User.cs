using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DuAnTN.Models
{
        public class User
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Email không được để trống.")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
            [MaxLength(150, ErrorMessage = "Email không được vượt quá 150 ký tự.")]
            public required string Email { get; set; }

            [Required(ErrorMessage = "Mật khẩu không được để trống.")]
            [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
            public required string Password { get; set; }

            [NotMapped] // Không lưu vào cơ sở dữ liệu
            [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu.")]
            [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp.")]
            public required string RePassword { get; set; }
            // 1-1 với UserInfo
            [JsonIgnore]
            public virtual UserInfo? UserInfo { get; set; }

            // 1-n với Order
            [JsonIgnore]
            public virtual ICollection<Order>? Orders { get; set; }

            // 1-n với Diner
            [JsonIgnore]
            public virtual ICollection<Diner>? Diners { get; set; }

            // 1-n với Comment
            [JsonIgnore]
            public virtual ICollection<Comment>? Comments { get; set; }

            [JsonIgnore]
            public virtual ICollection<Address>? Addresses { get; set; }

            public int RoleId { get; set; }

            [JsonIgnore]
            public virtual Role? role { get; set; }
        }
}

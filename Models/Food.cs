using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DuAnTN.Models
{
    public class Food
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên món ăn không được để trống.")]
        [MaxLength(150, ErrorMessage = "Tên món ăn không được vượt quá 150 ký tự.")]
        public required string FoodName { get; set; }

        [Required(ErrorMessage = "Giá không được để trống.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
        public decimal Price { get; set; }

        [MaxLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Ảnh chính không được để trống.")]
        public required string MainImage { get; set; }

        [Required(ErrorMessage = "Ảnh không được để trống.")]
        public string? Image1 { get; set; }

        [Required(ErrorMessage = "Ảnh không được để trống.")]
        public string? Image2 { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống.")]
        [MaxLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự.")]
        public required string Status { get; set; }

        // Khóa ngoại đến Diner (1-n)
        public int DinerId { get; set; }

        [JsonIgnore]
        public virtual Diner? Diner { get; set; }

        // Khóa ngoại đến Category (1-n)
        public int CategoryId { get; set; }

        [JsonIgnore]
        public virtual Category? Category { get; set; }

        // 1-n với OrderDetails
        [JsonIgnore]
        public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new HashSet<OrderDetails>();

        // 1-n với Comment
        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        // n-n với Voucher qua FoodVoucher
        [JsonIgnore]
        public virtual ICollection<FoodVoucher> FoodVouchers { get; set; } = new HashSet<FoodVoucher>();
    }
}

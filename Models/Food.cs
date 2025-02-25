using System.Text.Json.Serialization;

namespace API_DUAN_C5.Models
{
    public class Food
    {
        public int Id { get; set; }
        public string FoodName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string MainImage { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Status { get; set; }

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
        public virtual ICollection<OrderDetails>? OrderDetails { get; set; }

        // 1-n với Comment
        [JsonIgnore]
        public virtual ICollection<Comment>? Comments { get; set; }

        // n-n với Voucher qua FoodVoucher
        [JsonIgnore]
        public virtual ICollection<FoodVoucher>? FoodVouchers { get; set; }
    }
}

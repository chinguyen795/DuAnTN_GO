using System.Text.Json.Serialization;

namespace API_DUAN_C5.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }   // Khóa chính riêng

        // Khóa ngoại đến Order
        public int OrderId { get; set; }

        [JsonIgnore]
        public virtual Order? Order { get; set; }

        // Khóa ngoại đến Food
        public int FoodId { get; set; }

        [JsonIgnore]
        public virtual Food? Food { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

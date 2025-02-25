using System.Text.Json.Serialization;

namespace API_DUAN_C5.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }

        // Khóa ngoại đến User (1-n)
        public int UserId { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }

        // 1-n với OrderDetails
        [JsonIgnore]
        public virtual ICollection<OrderDetails>? OrderDetails { get; set; }
    }
}

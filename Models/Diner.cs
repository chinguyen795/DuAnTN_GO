using System.Text.Json.Serialization;

namespace DuAnTN.Models
{
    public class Diner
    {
        public int Id { get; set; }
        public string DinerName { get; set; }
        public string DinerAddress { get; set; }
        public string MainImage { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string PhoneNumber { get; set; }

        // Khóa ngoại đến User (1-n)
        public int UserId { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }

        // 1-n với Food
        [JsonIgnore]
        public virtual ICollection<Food>? Foods { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace DuAnTN.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreateAt { get; set; }

        // Khóa ngoại đến User (1-n)
        public int UserId { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }

        // Khóa ngoại đến Food (1-n)
        public int FoodId { get; set; }

        [JsonIgnore]
        public virtual Food? Food { get; set; }
    }
}

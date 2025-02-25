using System.Text.Json.Serialization;

namespace API_DUAN_C5.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }

        // 1-n với Food
        [JsonIgnore]
        public virtual ICollection<Food>? Foods { get; set; }
    }
}

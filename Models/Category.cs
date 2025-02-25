using System.Text.Json.Serialization;

namespace DuAnTN.Models
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

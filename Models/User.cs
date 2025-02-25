using System.Text.Json.Serialization;

namespace API_DUAN_C5.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }  // Theo sơ đồ bạn có cột Role

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
    }
}

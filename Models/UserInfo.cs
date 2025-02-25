using System.Text.Json.Serialization;

namespace API_DUAN_C5.Models
{
    public class UserInfo
    {
        public int Id { get; set; }

        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime BirthDay { get; set; }
        public string IdentityCard { get; set; }
        public DateTime CreateAt { get; set; }
        public string Avatar { get; set; }

        // Khóa ngoại đến User (1-1)
        public int UserId { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }
    }
}

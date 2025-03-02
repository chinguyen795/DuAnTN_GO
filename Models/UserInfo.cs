using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DuAnTN.Models
{
    public class UserInfo
    {
        public int Id { get; set; }

        public required string Gender { get; set; }

        public DateTime BirthDay { get; set; }

        public string? IdentityImageCard { get; set; }

        public required string IdentityCard { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;

        public string? Avatar { get; set; }

        // Khóa ngoại đến User (1-1)
        public int UserId { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }
    }

}

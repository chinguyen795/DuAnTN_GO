using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DuAnTN.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống.")]
        [MaxLength(50, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")]
        public required string CategoryName { get; set; }

        // 1-n với Food
        [JsonIgnore]
        public virtual ICollection<Food>? Foods { get; set; }
    }
}

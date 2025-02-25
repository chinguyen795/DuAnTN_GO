using System.Text.Json.Serialization;

namespace DuAnTN.Models
{
    public class Voucher
    {
        public int Id { get; set; }
        public decimal Percent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // n-n với Food qua FoodVoucher
        [JsonIgnore]
        public virtual ICollection<FoodVoucher>? FoodVouchers { get; set; }
    }
}

using System.Text.Json.Serialization;

namespace DuAnTN.Models
{
    public class FoodVoucher
    {
        public int Id { get; set; }  // PK riêng

        public int FoodId { get; set; }
        [JsonIgnore]
        public virtual Food? Food { get; set; }

        public int VoucherId { get; set; }
        [JsonIgnore]
        public virtual Voucher? Voucher { get; set; }
    }
}

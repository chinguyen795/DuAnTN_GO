using System.Text.Json.Serialization;

namespace API_DUAN_C5.Models
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

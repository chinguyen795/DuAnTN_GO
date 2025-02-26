using DuAnTN.Models;  // Hoặc namespace chứa AddressModel

namespace DuAnTN.Models

{
    public class AddressModel
    {
        public int Id { get; set; }  // Primary Key
        public string FullName { get; set; } // Full name of the person
        public string PhoneNumber { get; set; } // Phone number
        public string AddressLine { get; set; } // Address line (the address itself)
        public bool IsDefault { get; set; } // Is this the default address?
    }
}

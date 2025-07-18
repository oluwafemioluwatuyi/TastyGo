using TastyGo.Models;

namespace TastyGo.DTOs
{
    public class RegisterRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }
        public string DateOfBirth { get; set; } // Format: YYYY-MM-DD

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public UserType UserType { get; set; } // Enum: Customer, Vendor, Driver

        // Optional: For Drivers
        public string? NIN { get; set; } // National Identification Number

        public string? VehicleNumber { get; set; }
        public string? LicenseNumber { get; set; }

        // Optional: For Vendors (if needed)
        public string? BusinessName { get; set; }
    }
}

using System;
using TastyGo.Models;

namespace TastyGo.DTOs;

public class UserDto
{
    public Guid Id { get; set; }                     // Unique identifier
    public string FirstName { get; set; }            // User's first name
    public string LastName { get; set; }             // User's last name
    public string Email { get; set; }                // Email (login)
    public string Phone { get; set; }                // Optional
    public UserType UserType { get; set; }           // Enum: User, Vendor, Driver
    public bool IsEmailVerified { get; set; }        // Show verification status

    // Driver-specific
    public string? LicenseNumber { get; set; }
    public string? VehicleNumber { get; set; }

    // Vendor-specific
    public string? BusinessName { get; set; }

    public DateTime CreatedAt { get; set; }          // Optional: if you track creation
}

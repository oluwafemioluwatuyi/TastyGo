using System;
namespace TastyGo.DTOs;

public class VerifyLicenseDto
{
    public string LicenseNumber { get; set; }
    public string Name { get; set; } // Optional: if you're cross-checking names
    public string DateOfBirth { get; set; } // Optional: if required for verification
}

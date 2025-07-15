using System;

namespace TastyGo.DTOs;

public class ResetPinDto
{
    public string Email { get; set; }
    public string OldPin { get; set; } // The old pin to be verified
    public string NewPin { get; set; } // The new pin to be created
    public string ConfirmPin { get; set; }
}
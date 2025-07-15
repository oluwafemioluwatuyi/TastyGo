using System;
namespace TastyGo.DTOs;

public class CreatePinDto
{
    public string Email { get; set; }
    public string Pin { get; set; } // The pin to be created
    public string ConfirmPin { get; set; }
}
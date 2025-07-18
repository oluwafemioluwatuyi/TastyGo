using System;
namespace TastyGo.DTOs;

public class ResetPasswordRequestDto
{
    public string Email { get; set; }
    public string Token { get; set; }              // The reset token sent to their email
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}

using System;

namespace TastyGo.DTOs;

public class VerifyEmailRequestDto
{
    public string Email { get; set; }           // To identify the user
    public string Token { get; set; }           // The token sent in the verification email
}

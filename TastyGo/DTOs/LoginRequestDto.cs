namespace TastyGo.DTOs
{
    public class LoginRequestDto
    {
        public required string EmailOrPhone { get; set; }
        public required string Password { get; set; }
    }
}

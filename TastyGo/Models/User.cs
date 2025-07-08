namespace TastyGo.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public UserType userType { get; set; }
        public string NIN { get; set; }
        public string DateOfBirth { get; set; }
        public bool IsEmailVerified { get; set; }
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }

        //navigation
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();



    }
}

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
        public string Address { get; set; }
        public string NIN { get; set; }
        public string DateOfBirth { get; set; }

    }
}

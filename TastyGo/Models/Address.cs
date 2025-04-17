namespace TastyGo.Models
{
    public class Address: AuditableEntity
    {
        public Guid Id { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        // Navigation properties
        public User User { get; set; }
    }    
}

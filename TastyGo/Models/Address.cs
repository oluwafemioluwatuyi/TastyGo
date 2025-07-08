using System.ComponentModel.DataAnnotations.Schema;

namespace TastyGo.Models
{
    public class Address : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        // Navigation properties
        public User User { get; set; }
    }
}

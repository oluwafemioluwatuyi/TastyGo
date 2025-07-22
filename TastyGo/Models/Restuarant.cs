namespace TastyGo.Models
{
    public class Restaurant : AuditableEntity
    {

        public Guid Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool? IsOpen { get; set; }
        public TimeSpan OpensAt { get; set; }
        public TimeSpan ClosesAt { get; set; }

        // Navigation properties
        public Guid VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public ICollection<Menu> Menus { get; set; } = new List<Menu>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();


    }
}
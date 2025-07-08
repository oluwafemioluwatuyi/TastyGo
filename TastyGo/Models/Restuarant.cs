namespace TastyGo.Models
{
    public class Restaurant : AuditableEntity
    {

        public Guid Id { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public string Phone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public TimeSpan OpensAt { get; set; }
        public TimeSpan ClosesAt { get; set; }

        // Navigation properties
        //public ICollection<Menu> Menus { get; set; } = new List<Menu>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();


    }
}
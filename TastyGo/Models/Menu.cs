namespace TastyGo.Models
{
    public class Menu : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool IsAvailable { get; set; }
        public string PhotoUrl { get; set; }

        // Navigation properties
        public Restaurant Restaurant { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

    }
}

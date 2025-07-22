using System.ComponentModel.DataAnnotations.Schema;

namespace TastyGo.Models
{
    public class Menu : AuditableEntity
    {
        public Guid Id { get; set; }

        [ForeignKey(nameof(Restaurant))]
        public Guid RestaurantId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public MenuCategory MenuCategory { get; set; }
        public bool IsAvailable { get; set; }
        public string? PhotoUrl { get; set; }

        // Navigation properties
        public Restaurant? Restaurant { get; set; }


    }
}

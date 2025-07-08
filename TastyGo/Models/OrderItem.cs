using System.ComponentModel.DataAnnotations.Schema;

namespace TastyGo.Models
{
    public class OrderItem : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid MenuId { get; set; }

        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        // navigation properties
        public Order Order { get; set; }
        public Menu Menu { get; set; }
    }
}

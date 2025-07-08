using System.ComponentModel.DataAnnotations.Schema;

namespace TastyGo.Models
{
    public class Payment : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        // navigation properties
        public Order Order { get; set; }
    }
}

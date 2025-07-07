namespace TastyGo.Models
{
    public class Order : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PaymentId { get; set; }
        public Guid OrderItemId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Price { get; set; }
        public string DeliveryMethod { get; set; } = string.Empty;
        // navigation properties
        public User User { get; set; }
        public Payment Payment { get; set; }
        public OrderItem OrderItem { get; set; }
        public Restaurant Restaurant { get; set; }


    }
}

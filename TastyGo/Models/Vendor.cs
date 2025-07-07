namespace TastyGo.Models
{
    public class Vendor : AuditableEntity
    {
        public string StoreName { get; set; }
        public string Description { get; set; }
        public User User { get; set; }

    }
}

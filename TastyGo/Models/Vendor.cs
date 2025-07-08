using System.ComponentModel.DataAnnotations.Schema;

namespace TastyGo.Models
{
    public class Vendor : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public string StoreName { get; set; }
        public string? Description { get; set; }
        public User User { get; set; }


    }
}

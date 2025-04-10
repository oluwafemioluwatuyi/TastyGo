using System.ComponentModel.DataAnnotations.Schema;

namespace TastyGo.Models
{
    public class AuditableEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CreatedBy")]
        public Guid CreatedById { get; set; }
        public User CreatedBy { get; set; }

        [ForeignKey("ModifiedBy")]
        public Guid ModifiedById { get; set; }
        public User ModifiedBy { get; set; }

    }
}

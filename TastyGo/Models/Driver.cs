using System.ComponentModel.DataAnnotations.Schema;

namespace TastyGo.Models
{
    public class Driver : AuditableEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public string PlateNumber { get; set; }
        public string LicenseNo { get; set; }

        public User User { get; set; }


    }
}

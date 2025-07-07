namespace TastyGo.Models
{
    public class Driver : AuditableEntity
    {
        public string PlateNumber { get; set; }
        public string LicenseNo { get; set; }

        public User User { get; set; }

    }
}

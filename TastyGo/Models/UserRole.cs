namespace TastyGo.Models
{
    public class UserRole : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        //navigation
        public User User { get; set; }
        public Role Role { get; set; }
    }
}

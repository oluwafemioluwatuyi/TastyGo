namespace TastyGo.Models
{
    public class RolePermission : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        //navigation properties
        public Role Role { get; set; }
        public Permission Permission { get; set; }
    }
}

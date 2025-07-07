namespace TastyGo.Models
{
    public class Permission : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public PermissionAction Action { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    }
}

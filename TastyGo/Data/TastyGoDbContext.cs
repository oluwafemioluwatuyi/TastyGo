using Microsoft.EntityFrameworkCore;
using TastyGo.Models;

namespace TastyGo.Data
{
    public class TastyGoDbContext : DbContext
    {
        public TastyGoDbContext(DbContextOptions<TastyGoDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
    }
}

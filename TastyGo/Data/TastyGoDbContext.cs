using System.Linq.Expressions;
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
            base.OnModelCreating(modelBuilder);

            // Configure the enum converter for MyEntity and its MyEnumProperty
            ConfigureEnumConverter<User, UserType>(modelBuilder, e => e.userType);
            ConfigureEnumConverter<Permission, PermissionAction>(modelBuilder, e => e.Action);
            ConfigureEnumConverter<Module, Modules>(modelBuilder, e => e.Identifier);

            // Configure AuditableEntity
            ConfigureAuditableEntity<Address>(modelBuilder);
            ConfigureAuditableEntity<Driver>(modelBuilder);
            ConfigureAuditableEntity<Vendor>(modelBuilder);
            ConfigureAuditableEntity<Restaurant>(modelBuilder);
            ConfigureAuditableEntity<Menu>(modelBuilder);
            ConfigureAuditableEntity<Order>(modelBuilder);
            ConfigureAuditableEntity<OrderItem>(modelBuilder);
            ConfigureAuditableEntity<Payment>(modelBuilder);
            ConfigureAuditableEntity<Role>(modelBuilder);
            ConfigureAuditableEntity<Permission>(modelBuilder);
            ConfigureAuditableEntity<UserRole>(modelBuilder);
            ConfigureAuditableEntity<RolePermission>(modelBuilder);
            ConfigureAuditableEntity<Module>(modelBuilder);

            // Configure unique properties




        }


        private void ConfigureAuditableEntity<TEntity>(ModelBuilder modelBuilder) where TEntity : AuditableEntity
        {
            modelBuilder.Entity<TEntity>()
                .Property(e => e.CreatedAt)
                .IsRequired();

            modelBuilder.Entity<TEntity>()
                .Property(e => e.ModifiedAt);

            modelBuilder.Entity<TEntity>()
                .HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TEntity>()
                .HasOne(e => e.ModifiedBy)
                .WithMany()
                .HasForeignKey(e => e.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);
        }



        private void ConfigureEnumConverter<TEntity, TEnum>(ModelBuilder modelBuilder, Expression<Func<TEntity, TEnum>> propertyExpression) where TEntity : class
        {
            modelBuilder.Entity<TEntity>()
                .Property(propertyExpression)
                .HasConversion<string>();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Module> Modules { get; set; }
    }
}

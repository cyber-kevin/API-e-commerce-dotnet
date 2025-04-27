using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Persistence;

public class DefaultContext : IdentityDbContext<User> {

    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) {
            optionsBuilder.UseNpgsql("DefaultConnection");
        }
    }

    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder) {
        builder.HasPostgresExtension("uuid-ossp");
        builder.ApplyConfigurationsFromAssembly(typeof(DefaultContext).Assembly);

        base.OnModelCreating(builder);

        builder.Entity<Customer>(entity =>
        {
            entity.HasIndex(c => c.CPF).IsUnique();
            entity.HasIndex(c => c.Email).IsUnique();
            
            entity.HasOne(c => c.User)
                  .WithOne(u => u.Customer)
                  .HasForeignKey<Customer>(c => c.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

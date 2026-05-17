using CrudSample.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace CrudSample.Core.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();

            // Email value object — stored as a normalised string.
            entity.Property(u => u.Email)
                .HasConversion(
                    v => v.Value,
                    v => Email.Parse(v))
                .HasMaxLength(254)
                .IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.CreatedAt).IsRequired();
        });
    }
}

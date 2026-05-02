using Microsoft.EntityFrameworkCore;
using WEBArtigos.Entities;

namespace WEBArtigos.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Title).IsRequired().HasMaxLength(300);
            entity.Property(a => a.Content).IsRequired();
            entity.Property(a => a.Author).IsRequired().HasMaxLength(150);
            entity.Property(a => a.CreatedAt).IsRequired();

            entity.HasIndex(a => a.Title);
            entity.HasIndex(a => a.Author);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).IsRequired().HasMaxLength(50);
            entity.Property(u => u.CreatedAt).IsRequired();

            entity.HasIndex(u => u.Email).IsUnique();
        });
    }
}

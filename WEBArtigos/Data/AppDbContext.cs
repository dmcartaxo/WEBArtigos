using Microsoft.EntityFrameworkCore;
using WEBArtigos.Entities;

namespace WEBArtigos.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Article> Articles => Set<Article>();

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
    }
}

using Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Document> Documents => Set<Document>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(e =>
        {
            e.HasKey(d => d.Id);
            e.Property(d => d.FileName).HasMaxLength(255).IsRequired();
            e.Property(d => d.Status).HasConversion<string>().HasMaxLength(20);
            e.Property(d => d.StoragePath).HasMaxLength(1024).IsRequired();
            // SQL datetime2 has no kind; mark values read back as UTC so they serialize with "Z".
            e.Property(d => d.UploadedAt)
                .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
            e.HasIndex(d => d.UploadedAt);
        });
    }
}

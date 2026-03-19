using Microsoft.EntityFrameworkCore;
using MediaApp.Models;

namespace MediaApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Tables
    public DbSet<MediaItem> MediaItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure MediaItem table
        modelBuilder.Entity<MediaItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.FileUrl).IsRequired();
            entity.Property(e => e.UploadedBy).IsRequired().HasMaxLength(100);

            // Store Tags list as a comma-separated string in the DB
            entity.Property(e => e.Tags)
                .HasConversion(
                    tags => string.Join(',', tags),
                    str => str.Length > 0
                        ? str.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                        : new List<string>()
                );
        });

        // Seed some sample data so the app has content on first run
        modelBuilder.Entity<MediaItem>().HasData(
            new MediaItem
            {
                Id = 1,
                Title = "Company Introduction Video",
                Description = "Welcome video for new employees",
                Type = MediaType.Video,
                FileUrl = "https://storage.example.com/videos/intro.mp4",
                ThumbnailUrl = "https://storage.example.com/thumbnails/intro.jpg",
                FileSizeBytes = 52428800,
                ContentType = "video/mp4",
                UploadedBy = "admin",
                UploadedAt = new DateTime(2025, 1, 15, 9, 0, 0, DateTimeKind.Utc),
                IsPublished = true,
                Tags = new List<string> { "company", "onboarding", "video" }
            },
            new MediaItem
            {
                Id = 2,
                Title = "Product Banner Image",
                Description = "Main banner for homepage",
                Type = MediaType.Image,
                FileUrl = "https://storage.example.com/images/banner.jpg",
                ThumbnailUrl = "https://storage.example.com/thumbnails/banner.jpg",
                FileSizeBytes = 2097152,
                ContentType = "image/jpeg",
                UploadedBy = "designer",
                UploadedAt = new DateTime(2025, 2, 1, 10, 0, 0, DateTimeKind.Utc),
                IsPublished = true,
                Tags = new List<string> { "product", "banner", "homepage" }
            },
            new MediaItem
            {
                Id = 3,
                Title = "Q1 Financial Report",
                Description = "Quarterly financial summary document",
                Type = MediaType.Document,
                FileUrl = "https://storage.example.com/docs/q1-report.pdf",
                ThumbnailUrl = "",
                FileSizeBytes = 1048576,
                ContentType = "application/pdf",
                UploadedBy = "finance",
                UploadedAt = new DateTime(2025, 3, 1, 8, 0, 0, DateTimeKind.Utc),
                IsPublished = false,
                Tags = new List<string> { "finance", "report", "q1" }
            }
        );
    }
}
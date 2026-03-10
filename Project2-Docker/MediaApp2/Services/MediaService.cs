using MediaApp2.Models;

namespace MediaApp2.Services;

public interface IMediaService
{
    List<MediaItem> GetAll();
    MediaItem? GetById(int id);
    MediaItem Create(CreateMediaDto dto);
    bool Delete(int id);
}

public class MediaService : IMediaService
{
    // In-memory store — simulates a database
    private static readonly List<MediaItem> _store = new()
    {
        new MediaItem { Id = 1, Title = "Company Banner", Description = "Homepage banner image", MediaType = "Image", FileUrl = "https://example.com/banner.jpg", UploadedBy = "admin", IsPublished = true, Tags = new() { "banner", "homepage" } },
        new MediaItem { Id = 2, Title = "Intro Video", Description = "Welcome video for new users", MediaType = "Video", FileUrl = "https://example.com/intro.mp4", UploadedBy = "admin", IsPublished = true, Tags = new() { "video", "onboarding" } },
        new MediaItem { Id = 3, Title = "Product Brochure", Description = "Q1 product catalog", MediaType = "Document", FileUrl = "https://example.com/brochure.pdf", UploadedBy = "marketing", IsPublished = false, Tags = new() { "document", "product" } }
    };

    private static int _nextId = 4;

    public List<MediaItem> GetAll() => _store.OrderByDescending(m => m.UploadedAt).ToList();

    public MediaItem? GetById(int id) => _store.FirstOrDefault(m => m.Id == id);

    public MediaItem Create(CreateMediaDto dto)
    {
        var item = new MediaItem
        {
            Id = _nextId++,
            Title = dto.Title,
            Description = dto.Description,
            MediaType = dto.MediaType,
            FileUrl = dto.FileUrl,
            UploadedBy = dto.UploadedBy,
            Tags = dto.Tags,
            UploadedAt = DateTime.UtcNow,
            IsPublished = false
        };
        _store.Add(item);
        return item;
    }

    public bool Delete(int id)
    {
        var item = _store.FirstOrDefault(m => m.Id == id);
        if (item == null) return false;
        _store.Remove(item);
        return true;
    }
}

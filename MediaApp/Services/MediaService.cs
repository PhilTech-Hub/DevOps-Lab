using Microsoft.EntityFrameworkCore;
using MediaApp.Data;
using MediaApp.Models;

namespace MediaApp.Services;

// ── Interface (makes the service testable) ────────────────────────────────────
public interface IMediaService
{
    Task<List<MediaItemResponse>> GetAllAsync(MediaType? typeFilter = null, string? tag = null);
    Task<MediaItemResponse?> GetByIdAsync(int id);
    Task<MediaItemResponse> CreateAsync(CreateMediaItemDto dto);
    Task<MediaItemResponse?> UpdateAsync(int id, UpdateMediaItemDto dto);
    Task<bool> DeleteAsync(int id);
    Task<MediaItemResponse?> PublishAsync(int id);
    Task<List<MediaItemResponse>> SearchAsync(string query);
}

// ── Implementation ────────────────────────────────────────────────────────────
public class MediaService : IMediaService
{
    private readonly AppDbContext _db;

    public MediaService(AppDbContext db)
    {
        _db = db;
    }

    // Get all media items, with optional filter by type or tag
    public async Task<List<MediaItemResponse>> GetAllAsync(MediaType? typeFilter = null, string? tag = null)
    {
        var query = _db.MediaItems.AsQueryable();

        if (typeFilter.HasValue)
            query = query.Where(m => m.Type == typeFilter.Value);

        if (!string.IsNullOrWhiteSpace(tag))
            query = query.Where(m => m.Tags.Contains(tag));

        var items = await query.OrderByDescending(m => m.UploadedAt).ToListAsync();
        return items.Select(MapToResponse).ToList();
    }

    // Get a single item by ID
    public async Task<MediaItemResponse?> GetByIdAsync(int id)
    {
        var item = await _db.MediaItems.FindAsync(id);
        return item == null ? null : MapToResponse(item);
    }

    // Create a new media item
    public async Task<MediaItemResponse> CreateAsync(CreateMediaItemDto dto)
    {
        var item = new MediaItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Type = dto.Type,
            FileUrl = dto.FileUrl,
            ThumbnailUrl = dto.ThumbnailUrl,
            FileSizeBytes = dto.FileSizeBytes,
            ContentType = dto.ContentType,
            UploadedBy = dto.UploadedBy,
            UploadedAt = DateTime.UtcNow,
            Tags = dto.Tags,
            IsPublished = false
        };

        _db.MediaItems.Add(item);
        await _db.SaveChangesAsync();
        return MapToResponse(item);
    }

    // Update an existing media item
    public async Task<MediaItemResponse?> UpdateAsync(int id, UpdateMediaItemDto dto)
    {
        var item = await _db.MediaItems.FindAsync(id);
        if (item == null) return null;

        item.Title = dto.Title;
        item.Description = dto.Description;
        item.ThumbnailUrl = dto.ThumbnailUrl;
        item.IsPublished = dto.IsPublished;
        item.Tags = dto.Tags;
        item.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapToResponse(item);
    }

    // Delete a media item
    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _db.MediaItems.FindAsync(id);
        if (item == null) return false;

        _db.MediaItems.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }

    // Publish a media item (make it publicly visible)
    public async Task<MediaItemResponse?> PublishAsync(int id)
    {
        var item = await _db.MediaItems.FindAsync(id);
        if (item == null) return null;

        item.IsPublished = true;
        item.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return MapToResponse(item);
    }

    // Search by title or description
    public async Task<List<MediaItemResponse>> SearchAsync(string query)
    {
        var lower = query.ToLower();
        var items = await _db.MediaItems
            .Where(m => m.Title.ToLower().Contains(lower) ||
                        m.Description.ToLower().Contains(lower))
            .OrderByDescending(m => m.UploadedAt)
            .ToListAsync();

        return items.Select(MapToResponse).ToList();
    }

    // ── Helper: convert entity → response DTO ─────────────────────────────────
    private static MediaItemResponse MapToResponse(MediaItem item) => new()
    {
        Id = item.Id,
        Title = item.Title,
        Description = item.Description,
        Type = item.Type.ToString(),
        FileUrl = item.FileUrl,
        ThumbnailUrl = item.ThumbnailUrl,
        FileSizeFormatted = FormatFileSize(item.FileSizeBytes),
        ContentType = item.ContentType,
        UploadedBy = item.UploadedBy,
        UploadedAt = item.UploadedAt,
        IsPublished = item.IsPublished,
        Tags = item.Tags
    };

    private static string FormatFileSize(long bytes)
    {
        return bytes switch
        {
            < 1024 => $"{bytes} B",
            < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
            < 1024 * 1024 * 1024 => $"{bytes / (1024.0 * 1024):F1} MB",
            _ => $"{bytes / (1024.0 * 1024 * 1024):F1} GB"
        };
    }
}
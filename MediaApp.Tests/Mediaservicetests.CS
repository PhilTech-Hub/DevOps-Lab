using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using MediaApp.Data;
using MediaApp.Models;
using MediaApp.Services;

namespace MediaApp.Tests;

// =============================================================================
// UNIT TESTS — test MediaService logic directly using an in-memory database
// =============================================================================
public class MediaServiceTests
{
    // Helper: creates a fresh in-memory DB for each test (tests don't affect each other)
    private static AppDbContext CreateFreshDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddMediaItem()
    {
        // Arrange
        using var db = CreateFreshDb("CreateTest");
        var service = new MediaService(db);
        var dto = new CreateMediaItemDto
        {
            Title = "Test Video",
            Description = "A test video file",
            Type = MediaType.Video,
            FileUrl = "https://example.com/test.mp4",
            ContentType = "video/mp4",
            FileSizeBytes = 1024000,
            UploadedBy = "testuser",
            Tags = new List<string> { "test", "video" }
        };

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Video", result.Title);
        Assert.Equal("Video", result.Type);
        Assert.False(result.IsPublished); // new items are unpublished by default
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnItem_WhenExists()
    {
        // Arrange
        using var db = CreateFreshDb("GetByIdTest");
        var service = new MediaService(db);
        var created = await service.CreateAsync(new CreateMediaItemDto
        {
            Title = "My Image",
            Type = MediaType.Image,
            FileUrl = "https://example.com/img.jpg",
            ContentType = "image/jpeg",
            UploadedBy = "admin"
        });

        // Act
        var result = await service.GetByIdAsync(created.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("My Image", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        using var db = CreateFreshDb("GetByIdNullTest");
        var service = new MediaService(db);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyItem()
    {
        // Arrange
        using var db = CreateFreshDb("UpdateTest");
        var service = new MediaService(db);
        var created = await service.CreateAsync(new CreateMediaItemDto
        {
            Title = "Old Title",
            Type = MediaType.Document,
            FileUrl = "https://example.com/doc.pdf",
            ContentType = "application/pdf",
            UploadedBy = "admin"
        });

        // Act
        var updated = await service.UpdateAsync(created.Id, new UpdateMediaItemDto
        {
            Title = "New Title",
            Description = "Updated description",
            IsPublished = true,
            Tags = new List<string> { "updated" }
        });

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("New Title", updated.Title);
        Assert.True(updated.IsPublished);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveItem()
    {
        // Arrange
        using var db = CreateFreshDb("DeleteTest");
        var service = new MediaService(db);
        var created = await service.CreateAsync(new CreateMediaItemDto
        {
            Title = "To Delete",
            Type = MediaType.Audio,
            FileUrl = "https://example.com/audio.mp3",
            ContentType = "audio/mp3",
            UploadedBy = "admin"
        });

        // Act
        var deleted = await service.DeleteAsync(created.Id);
        var afterDelete = await service.GetByIdAsync(created.Id);

        // Assert
        Assert.True(deleted);
        Assert.Null(afterDelete);
    }

    [Fact]
    public async Task PublishAsync_ShouldSetIsPublishedTrue()
    {
        // Arrange
        using var db = CreateFreshDb("PublishTest");
        var service = new MediaService(db);
        var created = await service.CreateAsync(new CreateMediaItemDto
        {
            Title = "Draft Item",
            Type = MediaType.Image,
            FileUrl = "https://example.com/draft.jpg",
            ContentType = "image/jpeg",
            UploadedBy = "admin"
        });
        Assert.False(created.IsPublished);

        // Act
        var published = await service.PublishAsync(created.Id);

        // Assert
        Assert.NotNull(published);
        Assert.True(published.IsPublished);
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingItems()
    {
        // Arrange
        using var db = CreateFreshDb("SearchTest");
        var service = new MediaService(db);
        await service.CreateAsync(new CreateMediaItemDto { Title = "Banner Image", Type = MediaType.Image, FileUrl = "x", ContentType = "image/jpeg", UploadedBy = "admin" });
        await service.CreateAsync(new CreateMediaItemDto { Title = "Profile Photo", Type = MediaType.Image, FileUrl = "x", ContentType = "image/jpeg", UploadedBy = "admin" });
        await service.CreateAsync(new CreateMediaItemDto { Title = "Intro Video", Type = MediaType.Video, FileUrl = "x", ContentType = "video/mp4", UploadedBy = "admin" });

        // Act
        var results = await service.SearchAsync("banner");

        // Assert
        Assert.Single(results);
        Assert.Equal("Banner Image", results[0].Title);
    }

    [Fact]
    public async Task GetAllAsync_WithTypeFilter_ShouldFilterCorrectly()
    {
        // Arrange
        using var db = CreateFreshDb("FilterTest");
        var service = new MediaService(db);
        await service.CreateAsync(new CreateMediaItemDto { Title = "Vid1", Type = MediaType.Video, FileUrl = "x", ContentType = "video/mp4", UploadedBy = "admin" });
        await service.CreateAsync(new CreateMediaItemDto { Title = "Img1", Type = MediaType.Image, FileUrl = "x", ContentType = "image/jpeg", UploadedBy = "admin" });
        await service.CreateAsync(new CreateMediaItemDto { Title = "Vid2", Type = MediaType.Video, FileUrl = "x", ContentType = "video/mp4", UploadedBy = "admin" });

        // Act
        var videos = await service.GetAllAsync(typeFilter: MediaType.Video);

        // Assert
        Assert.Equal(2, videos.Count);
        Assert.All(videos, v => Assert.Equal("Video", v.Type));
    }
}

// =============================================================================
// INTEGRATION TESTS — spin up the real app and make HTTP requests
// =============================================================================
public class MediaApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MediaApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace SQL Server with InMemory for tests
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("IntegrationTestDb"));
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GET_AllMedia_Returns200()
    {
        var response = await _client.GetAsync("/api/media");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_HealthEndpoint_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/api/media/health");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("healthy", content);
    }

    [Fact]
    public async Task POST_CreateMedia_Returns201()
    {
        var dto = new CreateMediaItemDto
        {
            Title = "Integration Test Image",
            Description = "Created during integration test",
            Type = MediaType.Image,
            FileUrl = "https://example.com/test.jpg",
            ContentType = "image/jpeg",
            FileSizeBytes = 512000,
            UploadedBy = "testrunner",
            Tags = new List<string> { "test" }
        };

        var response = await _client.PostAsJsonAsync("/api/media", dto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GET_NonExistentMedia_Returns404()
    {
        var response = await _client.GetAsync("/api/media/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
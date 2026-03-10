using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using MediaApp2.Models;
using MediaApp2.Services;

namespace MediaApp2.Tests;

// =============================================================================
// UNIT TESTS — test MediaService directly
// =============================================================================
public class MediaServiceTests
{
    private readonly IMediaService _service = new MediaService();

    [Fact]
    public void GetAll_ShouldReturnItems()
    {
        var items = _service.GetAll();
        Assert.NotEmpty(items);
    }

    [Fact]
    public void GetById_ShouldReturnItem_WhenExists()
    {
        var item = _service.GetById(1);
        Assert.NotNull(item);
        Assert.Equal(1, item.Id);
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenNotExists()
    {
        var item = _service.GetById(9999);
        Assert.Null(item);
    }

    [Fact]
    public void Create_ShouldAddNewItem()
    {
        var dto = new CreateMediaDto
        {
            Title = "Test Image",
            Description = "A test image",
            MediaType = "Image",
            FileUrl = "https://example.com/test.jpg",
            UploadedBy = "testuser",
            Tags = new List<string> { "test" }
        };

        var created = _service.Create(dto);

        Assert.NotNull(created);
        Assert.Equal("Test Image", created.Title);
        Assert.False(created.IsPublished);
    }

    [Fact]
    public void Delete_ShouldRemoveItem()
    {
        var dto = new CreateMediaDto
        {
            Title = "To Delete",
            MediaType = "Video",
            FileUrl = "https://example.com/delete.mp4",
            UploadedBy = "admin"
        };
        var created = _service.Create(dto);

        var result = _service.Delete(created.Id);
        var afterDelete = _service.GetById(created.Id);

        Assert.True(result);
        Assert.Null(afterDelete);
    }

    [Fact]
    public void Delete_ShouldReturnFalse_WhenNotExists()
    {
        var result = _service.Delete(9999);
        Assert.False(result);
    }
}

// =============================================================================
// INTEGRATION TESTS — spin up the real app and test HTTP endpoints
// =============================================================================
public class MediaControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MediaControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_AllMedia_Returns200()
    {
        var response = await _client.GetAsync("/api/media2");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_Health_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/api/media2/health");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("healthy", content);
    }

    [Fact]
    public async Task GET_ContainerInfo_ReturnsInfo()
    {
        var response = await _client.GetAsync("/api/media2/container-info");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Docker", content);
    }

    [Fact]
    public async Task POST_CreateMedia_Returns201()
    {
        var dto = new CreateMediaDto
        {
            Title = "Integration Test",
            MediaType = "Image",
            FileUrl = "https://example.com/test.jpg",
            UploadedBy = "testrunner"
        };

        var response = await _client.PostAsJsonAsync("/api/media2", dto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GET_NonExistent_Returns404()
    {
        var response = await _client.GetAsync("/api/media2/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

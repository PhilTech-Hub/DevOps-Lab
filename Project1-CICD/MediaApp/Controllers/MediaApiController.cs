using Microsoft.AspNetCore.Mvc;
using MediaApp.Models;
using MediaApp.Services;

namespace MediaApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;
    private readonly ILogger<MediaController> _logger;

    public MediaController(IMediaService mediaService, ILogger<MediaController> logger)
    {
        _mediaService = mediaService;
        _logger = logger;
    }

    // GET /api/media
    // GET /api/media?type=Video
    // GET /api/media?tag=banner
    [HttpGet]
    public async Task<ActionResult<List<MediaItemResponse>>> GetAll(
        [FromQuery] MediaType? type = null,
        [FromQuery] string? tag = null)
    {
        _logger.LogInformation("Fetching all media items. Filter: type={Type}, tag={Tag}", type, tag);
        var items = await _mediaService.GetAllAsync(type, tag);
        return Ok(items);
    }

    // GET /api/media/5
    [HttpGet("{id}")]
    public async Task<ActionResult<MediaItemResponse>> GetById(int id)
    {
        var item = await _mediaService.GetByIdAsync(id);
        if (item == null)
        {
            _logger.LogWarning("Media item {Id} not found", id);
            return NotFound(new { message = $"Media item {id} not found" });
        }
        return Ok(item);
    }

    // GET /api/media/search?q=banner
    [HttpGet("search")]
    public async Task<ActionResult<List<MediaItemResponse>>> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { message = "Search query cannot be empty" });

        var results = await _mediaService.SearchAsync(q);
        return Ok(results);
    }

    // POST /api/media
    [HttpPost]
    public async Task<ActionResult<MediaItemResponse>> Create([FromBody] CreateMediaItemDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _logger.LogInformation("Creating new media item: {Title}", dto.Title);
        var created = await _mediaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/media/5
    [HttpPut("{id}")]
    public async Task<ActionResult<MediaItemResponse>> Update(int id, [FromBody] UpdateMediaItemDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updated = await _mediaService.UpdateAsync(id, dto);
        if (updated == null)
            return NotFound(new { message = $"Media item {id} not found" });

        _logger.LogInformation("Updated media item {Id}", id);
        return Ok(updated);
    }

    // DELETE /api/media/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mediaService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Media item {id} not found" });

        _logger.LogInformation("Deleted media item {Id}", id);
        return NoContent();
    }

    // POST /api/media/5/publish
    [HttpPost("{id}/publish")]
    public async Task<ActionResult<MediaItemResponse>> Publish(int id)
    {
        var published = await _mediaService.PublishAsync(id);
        if (published == null)
            return NotFound(new { message = $"Media item {id} not found" });

        _logger.LogInformation("Published media item {Id}", id);
        return Ok(published);
    }

    // GET /api/media/health
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "MediaApp", timestamp = DateTime.UtcNow });
    }
}
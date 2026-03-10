using Microsoft.AspNetCore.Mvc;
using MediaApp2.Models;
using MediaApp2.Services;

namespace MediaApp2.Controllers;

[ApiController]
[Route("api/media2")]
[Produces("application/json")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _service;
    private readonly ILogger<MediaController> _logger;

    public MediaController(IMediaService service, ILogger<MediaController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // GET /api/media2
    [HttpGet]
    public ActionResult<List<MediaItem>> GetAll()
    {
        _logger.LogInformation("Fetching all media items via NGINX reverse proxy");
        return Ok(_service.GetAll());
    }

    // GET /api/media2/5
    [HttpGet("{id}")]
    public ActionResult<MediaItem> GetById(int id)
    {
        var item = _service.GetById(id);
        if (item == null) return NotFound(new { message = $"Item {id} not found" });
        return Ok(item);
    }

    // POST /api/media2
    [HttpPost]
    public ActionResult<MediaItem> Create([FromBody] CreateMediaDto dto)
    {
        var created = _service.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // DELETE /api/media2/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = _service.Delete(id);
        if (!deleted) return NotFound(new { message = $"Item {id} not found" });
        return NoContent();
    }

    // GET /api/media2/health
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            service = "MediaApp2",
            container = Environment.MachineName,  // shows Docker container ID
            timestamp = DateTime.UtcNow,
            proxiedVia = "NGINX"
        });
    }

    // GET /api/media2/container-info
    [HttpGet("container-info")]
    public IActionResult ContainerInfo()
    {
        return Ok(new
        {
            containerName = Environment.MachineName,
            os = Environment.OSVersion.ToString(),
            dotnetVersion = Environment.Version.ToString(),
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            message = "Running inside Docker container behind NGINX reverse proxy"
        });
    }
}

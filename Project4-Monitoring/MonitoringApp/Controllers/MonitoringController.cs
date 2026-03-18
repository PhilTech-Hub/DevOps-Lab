using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MonitoringApp.Models;
using MonitoringApp.Services;

namespace MonitoringApp.Controllers;

// =============================================================================
// MonitoringController — health checks, metrics, logs dashboard
// =============================================================================
[ApiController]
[Route("api/monitoring")]
[Produces("application/json")]
public class MonitoringController : ControllerBase
{
    private readonly IMetricsService _metrics;
    private readonly ILogger<MonitoringController> _logger;
    private readonly HealthCheckService _healthCheck;

    public MonitoringController(
        IMetricsService metrics,
        ILogger<MonitoringController> logger,
        HealthCheckService healthCheck)
    {
        _metrics = metrics;
        _logger = logger;
        _healthCheck = healthCheck;
    }

    // GET /api/monitoring/health — simple health check
    [HttpGet("health")]
    public async Task<IActionResult> Health()
    {
        var result = await _healthCheck.CheckHealthAsync();
        var status = result.Status == HealthStatus.Healthy ? "healthy"
                   : result.Status == HealthStatus.Degraded ? "degraded"
                   : "unhealthy";

        var response = new
        {
            status,
            checks = result.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString().ToLower(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds
            }),
            totalDuration = result.TotalDuration.TotalMilliseconds,
            timestamp = DateTime.UtcNow
        };

        var statusCode = result.Status == HealthStatus.Healthy ? 200
                       : result.Status == HealthStatus.Degraded ? 200
                       : 503;

        return StatusCode(statusCode, response);
    }

    // GET /api/monitoring/metrics — system metrics
    [HttpGet("metrics")]
    public IActionResult GetMetrics()
    {
        _logger.LogInformation("Metrics requested");
        var metrics = _metrics.GetSystemMetrics();
        return Ok(new ApiResponse<SystemMetrics> { Success = true, Data = metrics });
    }

    // GET /api/monitoring/logs/test — generate test log entries
    [HttpGet("logs/test")]
    public IActionResult TestLogs()
    {
        _logger.LogDebug("This is a DEBUG log — detailed diagnostic info");
        _logger.LogInformation("This is an INFO log — normal operation");
        _logger.LogWarning("This is a WARNING log — something to watch");
        _logger.LogError("This is an ERROR log — something went wrong");

        _logger.LogInformation("User action logged | Action:{Action} | User:{User}",
            "ViewMetrics", "demo-user");

        return Ok(new
        {
            message = "Test logs generated successfully",
            logsGenerated = 5,
            levels = new[] { "Debug", "Information", "Warning", "Error" },
            timestamp = DateTime.UtcNow,
            hint = "Check the console output or logs/app.log file to see these logs"
        });
    }

    // POST /api/monitoring/logs/simulate-error — simulate an error for testing
    [HttpPost("logs/simulate-error")]
    public IActionResult SimulateError([FromQuery] string type = "general")
    {
        _logger.LogWarning("Simulating error | Type:{ErrorType}", type);
        _metrics.RecordError("/api/monitoring/simulate-error", type);

        try
        {
            throw new InvalidOperationException($"Simulated {type} error for testing");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Simulated error captured | Type:{ErrorType}", type);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Error = $"Simulated error: {ex.Message}",
                Data = new { errorType = type, simulatedAt = DateTime.UtcNow }
            });
        }
    }
}

// =============================================================================
// MediaController — sample business API with built-in logging
// =============================================================================
[ApiController]
[Route("api/media")]
[Produces("application/json")]
public class MediaController : ControllerBase
{
    private readonly ILogger<MediaController> _logger;
    private readonly IMetricsService _metrics;

    // Sample in-memory data
    private static readonly List<MediaItem> _items = new()
    {
        new MediaItem { Id = 1, Title = "Company Banner", Type = "Image" },
        new MediaItem { Id = 2, Title = "Intro Video", Type = "Video" },
        new MediaItem { Id = 3, Title = "Product Brochure", Type = "Document" }
    };

    public MediaController(ILogger<MediaController> logger, IMetricsService metrics)
    {
        _logger = logger;
        _metrics = metrics;
    }

    // GET /api/media
    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("Fetching all media items | Count:{Count}", _items.Count);
        return Ok(new ApiResponse<List<MediaItem>> { Success = true, Data = _items });
    }

    // GET /api/media/5
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var item = _items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            _logger.LogWarning("Media item not found | Id:{Id}", id);
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Error = $"Item {id} not found"
            });
        }

        _logger.LogInformation("Media item retrieved | Id:{Id} | Title:{Title}", id, item.Title);
        return Ok(new ApiResponse<MediaItem> { Success = true, Data = item });
    }
}

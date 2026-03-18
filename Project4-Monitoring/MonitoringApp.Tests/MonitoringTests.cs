using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;
using MonitoringApp.Services;

namespace MonitoringApp.Tests;

// =============================================================================
// UNIT TESTS — test MetricsService directly
// =============================================================================
public class MetricsServiceTests
{
    private readonly IMetricsService _service = new MetricsService();

    [Fact]
    public void GetSystemMetrics_ShouldReturnMetrics()
    {
        var metrics = _service.GetSystemMetrics();

        Assert.NotNull(metrics);
        Assert.True(metrics.MemoryUsedBytes > 0);
        Assert.True(metrics.UptimeSeconds >= 0);
        Assert.NotEmpty(metrics.Environment);
        Assert.NotEmpty(metrics.Version);
    }

    [Fact]
    public void RecordRequest_ShouldNotThrow()
    {
        var exception = Record.Exception(() =>
            _service.RecordRequest("/api/test", "GET", 200, 45.5));

        Assert.Null(exception);
    }

    [Fact]
    public void RecordError_ShouldNotThrow()
    {
        var exception = Record.Exception(() =>
            _service.RecordError("/api/test", "InvalidOperationException"));

        Assert.Null(exception);
    }

    [Fact]
    public void GetSystemMetrics_UptimeShouldIncrease()
    {
        var first = _service.GetSystemMetrics();
        System.Threading.Thread.Sleep(100);
        var second = _service.GetSystemMetrics();

        Assert.True(second.UptimeSeconds >= first.UptimeSeconds);
    }
}

// =============================================================================
// INTEGRATION TESTS — test real HTTP endpoints
// =============================================================================
public class MonitoringIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MonitoringIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_Health_Returns200()
    {
        var response = await _client.GetAsync("/api/monitoring/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_Health_ReturnsHealthyStatus()
    {
        var response = await _client.GetAsync("/api/monitoring/health");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("healthy", content);
    }

    [Fact]
    public async Task GET_Metrics_Returns200()
    {
        var response = await _client.GetAsync("/api/monitoring/metrics");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_Metrics_ReturnsSystemInfo()
    {
        var response = await _client.GetAsync("/api/monitoring/metrics");
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("memoryUsedBytes", content);
        Assert.Contains("uptimeSeconds", content);
    }

    [Fact]
    public async Task GET_TestLogs_Returns200()
    {
        var response = await _client.GetAsync("/api/monitoring/logs/test");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_AllMedia_Returns200()
    {
        var response = await _client.GetAsync("/api/media");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_MediaById_Returns200_WhenExists()
    {
        var response = await _client.GetAsync("/api/media/1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_MediaById_Returns404_WhenNotExists()
    {
        var response = await _client.GetAsync("/api/media/9999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GET_HealthCheck_Endpoint_Returns200()
    {
        var response = await _client.GetAsync("/healthz/live");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GET_PrometheusMetrics_Returns200()
    {
        var response = await _client.GetAsync("/metrics");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

using Prometheus;
using MonitoringApp.Models;

namespace MonitoringApp.Services;

// ── Interface ──────────────────────────────────────────────────────────────────
public interface IMetricsService
{
    void RecordRequest(string endpoint, string method, int statusCode, double durationMs);
    void RecordError(string endpoint, string errorType);
    SystemMetrics GetSystemMetrics();
}

// ── Implementation ─────────────────────────────────────────────────────────────
public class MetricsService : IMetricsService
{
    // Prometheus counters and histograms
    // These are automatically exported to /metrics endpoint
    private static readonly Counter RequestCounter = Metrics
        .CreateCounter("app_requests_total", "Total number of HTTP requests",
            new CounterConfiguration
            {
                LabelNames = new[] { "endpoint", "method", "status_code" }
            });

    private static readonly Histogram RequestDuration = Metrics
        .CreateHistogram("app_request_duration_ms", "HTTP request duration in milliseconds",
            new HistogramConfiguration
            {
                LabelNames = new[] { "endpoint", "method" },
                Buckets = new[] { 10.0, 50.0, 100.0, 200.0, 500.0, 1000.0, 2000.0 }
            });

    private static readonly Counter ErrorCounter = Metrics
        .CreateCounter("app_errors_total", "Total number of application errors",
            new CounterConfiguration
            {
                LabelNames = new[] { "endpoint", "error_type" }
            });

    private static readonly Gauge ActiveRequests = Metrics
        .CreateGauge("app_active_requests", "Number of currently active requests");

    private static long _totalRequests = 0;
    private static readonly DateTime _startTime = DateTime.UtcNow;

    public void RecordRequest(string endpoint, string method, int statusCode, double durationMs)
    {
        Interlocked.Increment(ref _totalRequests);
        RequestCounter.WithLabels(endpoint, method, statusCode.ToString()).Inc();
        RequestDuration.WithLabels(endpoint, method).Observe(durationMs);
    }

    public void RecordError(string endpoint, string errorType)
    {
        ErrorCounter.WithLabels(endpoint, errorType).Inc();
    }

    public SystemMetrics GetSystemMetrics()
    {
        var process = System.Diagnostics.Process.GetCurrentProcess();
        var totalMemory = GC.GetTotalMemory(false);

        return new SystemMetrics
        {
            CpuUsagePercent = Math.Round(process.TotalProcessorTime.TotalMilliseconds /
                (Environment.ProcessorCount * (DateTime.UtcNow - _startTime).TotalMilliseconds) * 100, 2),
            MemoryUsedBytes = process.WorkingSet64,
            MemoryTotalBytes = totalMemory,
            MemoryUsagePercent = Math.Round((double)process.WorkingSet64 / (1024 * 1024 * 1024) * 100, 2),
            TotalRequests = _totalRequests,
            UptimeSeconds = (DateTime.UtcNow - _startTime).TotalSeconds,
            Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
            Version = "1.0.0",
            CollectedAt = DateTime.UtcNow
        };
    }
}

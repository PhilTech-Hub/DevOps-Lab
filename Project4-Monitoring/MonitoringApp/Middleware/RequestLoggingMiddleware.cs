using System.Diagnostics;
using MonitoringApp.Services;

namespace MonitoringApp.Middleware;

// =============================================================================
// RequestLoggingMiddleware
// Automatically logs every HTTP request with timing information
// This runs on EVERY request — you don't need to add logging to each controller
// =============================================================================
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly IMetricsService _metrics;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger,
        IMetricsService metrics)
    {
        _next = next;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8];

        // Log the incoming request
        _logger.LogInformation(
            "Request started | ID:{RequestId} | {Method} {Path} | IP:{IP}",
            requestId,
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress);

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Request failed | ID:{RequestId} | {Method} {Path} | Error:{Error}",
                requestId,
                context.Request.Method,
                context.Request.Path,
                ex.Message);

            _metrics.RecordError(context.Request.Path, ex.GetType().Name);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            var duration = stopwatch.Elapsed.TotalMilliseconds;

            // Record metrics for Prometheus
            _metrics.RecordRequest(
                context.Request.Path,
                context.Request.Method,
                context.Response.StatusCode,
                duration);

            // Log the completed request with status and duration
            var logLevel = context.Response.StatusCode >= 500 ? LogLevel.Error
                         : context.Response.StatusCode >= 400 ? LogLevel.Warning
                         : LogLevel.Information;

            _logger.Log(logLevel,
                "Request completed | ID:{RequestId} | {Method} {Path} | Status:{StatusCode} | Duration:{Duration}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                Math.Round(duration, 2));
        }
    }
}

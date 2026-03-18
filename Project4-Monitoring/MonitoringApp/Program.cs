using Serilog;
using Serilog.Formatting.Compact;
using Prometheus;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MonitoringApp.Middleware;
using MonitoringApp.Services;

// =============================================================================
// Configure Serilog — structured logging to console + file
// =============================================================================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(new CompactJsonFormatter())   // JSON logs to console
    .WriteTo.File(
        path: "logs/app.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        formatter: new CompactJsonFormatter())     // JSON logs to file
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog instead of default logging
builder.Host.UseSerilog();

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Monitoring & Logging API",
        Version = "v1",
        Description = "ASP.NET Core API with full observability — structured logging, health checks, and Prometheus metrics"
    });
});

// Register metrics service
builder.Services.AddSingleton<IMetricsService, MetricsService>();

// ── Health Checks ─────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("Application is running"), tags: new[] { "live" })
    .AddCheck("memory", () =>
    {
        var allocated = GC.GetTotalMemory(false);
        var threshold = 500 * 1024 * 1024; // 500MB
        return allocated < threshold
            ? HealthCheckResult.Healthy($"Memory OK: {allocated / 1024 / 1024}MB used")
            : HealthCheckResult.Degraded($"Memory high: {allocated / 1024 / 1024}MB used");
    }, tags: new[] { "memory" })
    .AddCheck("disk", () =>
    {
        var drive = new DriveInfo(Path.GetPathRoot(Directory.GetCurrentDirectory()) ?? "C:");
        var freeSpaceGB = drive.AvailableFreeSpace / (1024.0 * 1024 * 1024);
        return freeSpaceGB > 1
            ? HealthCheckResult.Healthy($"Disk OK: {freeSpaceGB:F1}GB free")
            : HealthCheckResult.Unhealthy($"Disk low: {freeSpaceGB:F1}GB free");
    }, tags: new[] { "disk" });

// Health Checks UI
builder.Services.AddHealthChecksUI(options =>
{
    options.SetEvaluationTimeInSeconds(30);
    options.AddHealthCheckEndpoint("MonitoringApp", "/healthz");
}).AddInMemoryStorage();

// ── Build app ─────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Monitoring API v1");
    c.RoutePrefix = "swagger";
});

// Custom request logging middleware
app.UseMiddleware<RequestLoggingMiddleware>();

// Prometheus metrics endpoint — visit /metrics to see all metrics
app.UseMetricServer();
app.UseHttpMetrics();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Health check endpoints
app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

// Health Checks UI dashboard — visit /healthchecks-ui
app.MapHealthChecksUI();

Log.Information("MonitoringApp started | Environment:{Env}", app.Environment.EnvironmentName);

app.Run();

public partial class Program { }

namespace MonitoringApp.Models;

// ── API response models ────────────────────────────────────────────────────────
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// ── Media item model ───────────────────────────────────────────────────────────
public class MediaItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ── System metrics model ───────────────────────────────────────────────────────
public class SystemMetrics
{
    public double CpuUsagePercent { get; set; }
    public long MemoryUsedBytes { get; set; }
    public long MemoryTotalBytes { get; set; }
    public double MemoryUsagePercent { get; set; }
    public long TotalRequests { get; set; }
    public double UptimeSeconds { get; set; }
    public string Environment { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public DateTime CollectedAt { get; set; } = DateTime.UtcNow;
}

// ── Log entry model ────────────────────────────────────────────────────────────
public class LogEntry
{
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Properties { get; set; } = new();
}

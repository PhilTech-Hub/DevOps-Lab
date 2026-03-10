using Microsoft.EntityFrameworkCore;
using MediaApp.Data;
using MediaApp.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────

builder.Services.AddControllers();

// Swagger/OpenAPI — lets you test the API in the browser
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Media Content Management API",
        Version = "v1",
        Description = "REST API for managing media content — images, videos, audio, and documents"
    });
});

// Database — uses InMemory in Development, SQL Server in Production
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("MediaDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

// Register our MediaService
builder.Services.AddScoped<IMediaService, MediaService>();

// ── Build & configure pipeline ────────────────────────────────────────────────

var app = builder.Build();

// Seed the in-memory database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Always show Swagger UI (useful for demo/portfolio)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Media API v1");
    c.RoutePrefix = string.Empty; // Swagger loads at root URL
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Required for integration testing
public partial class Program { }
using MediaApp2.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Media App 2 — Containerized API",
        Version = "v1",
        Description = "ASP.NET Core API running inside Docker behind NGINX reverse proxy"
    });
});

// Register services
builder.Services.AddSingleton<IMediaService, MediaService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Media API v2");
    c.RoutePrefix = "swagger";
});

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }

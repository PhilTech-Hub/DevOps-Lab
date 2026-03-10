# =============================================================================
# Multi-stage Dockerfile — MediaApp2
# This app runs BEHIND an NGINX reverse proxy (see docker-compose.yml)
# =============================================================================

# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MediaApp2/MediaApp2.csproj             MediaApp2/
COPY MediaApp2.Tests/MediaApp2.Tests.csproj MediaApp2.Tests/
RUN dotnet restore MediaApp2/MediaApp2.csproj

COPY . .

WORKDIR /src/MediaApp2
RUN dotnet build MediaApp2.csproj -c Release -o /app/build --no-restore

# ── Stage 2: Publish ──────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish MediaApp2.csproj -c Release -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ── Stage 3: Final runtime image ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Non-root user for security
RUN addgroup --system appgroup \
    && adduser --system --ingroup appgroup appuser
USER appuser

COPY --from=publish /app/publish .

# App listens on 5000 — NGINX will forward to this port
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "MediaApp2.dll"]

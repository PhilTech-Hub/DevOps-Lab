# =============================================================================
# Multi-stage Dockerfile — Media Content Management System
# Stage 1: Build  (uses full SDK — bigger image)
# Stage 2: Final  (uses runtime only — small, secure image)
# =============================================================================

# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first for better layer caching
# Docker only re-runs restore when .csproj files change
COPY MediaApp/MediaApp.csproj             MediaApp/
COPY MediaApp.Tests/MediaApp.Tests.csproj MediaApp.Tests/
RUN dotnet restore MediaApp/MediaApp.csproj

# Copy all source code
COPY . .

# Build in Release mode
WORKDIR /src/MediaApp
RUN dotnet build MediaApp.csproj -c Release -o /app/build --no-restore

# ── Stage 2: Publish ──────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish MediaApp.csproj -c Release -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# ── Stage 3: Final runtime image ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Security: non-root user
RUN addgroup --system appgroup \
    && adduser --system --ingroup appgroup appuser
USER appuser

# Copy only published output (no SDK, no source code)
COPY --from=publish /app/publish .

EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "MediaApp.dll"]
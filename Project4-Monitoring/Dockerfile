# =============================================================================
# Multi-stage Dockerfile — MonitoringApp
# =============================================================================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY MonitoringApp/MonitoringApp.csproj             MonitoringApp/
COPY MonitoringApp.Tests/MonitoringApp.Tests.csproj MonitoringApp.Tests/
RUN dotnet restore MonitoringApp/MonitoringApp.csproj

COPY . .

WORKDIR /src/MonitoringApp
RUN dotnet build MonitoringApp.csproj -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish MonitoringApp.csproj -c Release -o /app/publish \
    --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN addgroup --system appgroup \
    && adduser --system --ingroup appgroup appuser

RUN mkdir -p /app/logs && chown appuser:appgroup /app/logs
USER appuser

COPY --from=publish /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "MonitoringApp.dll"]

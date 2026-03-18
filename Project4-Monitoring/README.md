# 📊 Project 4 — Monitoring & Logging System

> Full observability stack for ASP.NET Core — structured logging with Serilog,
> health checks, Prometheus metrics, and Grafana dashboards.

---

## 🏗️ Architecture

```
                    ┌─────────────────────────────────────┐
                    │         Monitoring Stack             │
                    │                                     │
Browser ──────────► │  MonitoringApp  ──metrics──► Prometheus  │
(port 5000)         │  (ASP.NET API)               (port 9090) │
                    │       │                          │        │
                    │       │ logs                     │        │
                    │       ▼                     ┌────▼────┐   │
                    │  logs/app.log               │ Grafana │   │
                    │                             │(port    │   │
                    │                             │ 3000)   │   │
                    └─────────────────────────────┴─────────┴───┘
```

---

## 🗂️ Project structure

```
Project4-Monitoring/
├── .github/workflows/
│   └── project4-ci-cd.yml          ← CI/CD pipeline
├── grafana/
│   └── datasource.yml              ← Auto-connects Grafana to Prometheus
├── prometheus/
│   └── prometheus.yml              ← Scrapes metrics every 15s
├── MonitoringApp/
│   ├── Controllers/
│   │   └── MonitoringController.cs ← Health, metrics, logs endpoints
│   ├── Middleware/
│   │   └── RequestLoggingMiddleware.cs ← Logs every HTTP request
│   ├── Models/
│   │   └── Models.cs
│   ├── Services/
│   │   └── MetricsService.cs       ← Prometheus counters and histograms
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Program.cs                  ← Serilog + health checks + Prometheus
│   ├── appsettings.json
│   └── MonitoringApp.csproj
├── MonitoringApp.Tests/
│   ├── MonitoringTests.cs          ← 10 tests
│   └── MonitoringApp.Tests.csproj
├── Dockerfile
├── docker-compose.yml              ← App + Prometheus + Grafana
├── .gitignore
├── .dockerignore
└── MonitoringApp.sln
```

---

## 🛠️ Run locally

### Option A — dotnet run
```bash
cd MonitoringApp
dotnet run
```

| URL | What you see |
|-----|-------------|
| `http://localhost:5000/swagger` | API documentation |
| `http://localhost:5000/api/monitoring/health` | Health check |
| `http://localhost:5000/api/monitoring/metrics` | System metrics |
| `http://localhost:5000/metrics` | Prometheus metrics |
| `http://localhost:5000/healthchecks-ui` | Health dashboard |

### Option B — Full stack (App + Prometheus + Grafana)
```bash
docker-compose up --build
```

| URL | What you see |
|-----|-------------|
| `http://localhost:5000/swagger` | API |
| `http://localhost:9090` | Prometheus |
| `http://localhost:3000` | Grafana (admin/admin123) |

---

## 🧪 Run tests
```bash
dotnet test
# Expected: Passed! - 14 tests ✅
```

---

## 📈 What this demonstrates on your CV

| Skill | Evidence |
|---|---|
| Structured logging | Serilog with JSON output to console + file |
| Health checks | 3 checks: self, memory, disk |
| Prometheus metrics | Request counters, duration histograms, error counters |
| Grafana dashboards | Auto-provisioned datasource |
| Observability middleware | Every request logged automatically |
| Docker monitoring stack | 3-service docker-compose setup |

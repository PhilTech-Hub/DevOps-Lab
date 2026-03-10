# 🐳 Project 2 — Containerized Web Application Deployment

> ASP.NET Core API containerized with Docker and served through an NGINX reverse proxy.
> Part of the DevOps-Lab portfolio.

---

## 🏗️ Architecture

```
                    Docker Network (app-network)
                 ┌─────────────────────────────┐
                 │                             │
Browser ──80──►  │  NGINX        ──5000──►  MediaApp2  │
(public)         │  (reverse proxy)  (internal only)   │
                 │                             │
                 └─────────────────────────────┘
```

- **NGINX** is the only container exposed to the internet (port 80)
- **MediaApp2** runs internally — not directly accessible
- NGINX forwards all requests to MediaApp2

---

## 🗂️ Project structure

```
Project2-Docker/
├── .github/workflows/
│   └── project2-ci-cd.yml     ← CI/CD pipeline
├── nginx/
│   └── nginx.conf             ← NGINX reverse proxy config
├── MediaApp2/
│   ├── Controllers/
│   │   └── MediaController.cs ← API endpoints
│   ├── Models/
│   │   └── Models.cs
│   ├── Services/
│   │   └── MediaService.cs
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Program.cs
│   ├── appsettings.json
│   └── MediaApp2.csproj
├── MediaApp2.Tests/
│   ├── MediaApp2Tests.cs      ← 10 tests
│   └── MediaApp2.Tests.csproj
├── Dockerfile                 ← Multi-stage build
├── docker-compose.yml         ← Runs NGINX + App together
├── .gitignore
├── .dockerignore
└── MediaApp2.sln
```

---

## 🛠️ Run locally

### Option A — dotnet run (no Docker needed)
```bash
cd MediaApp2
dotnet run
# Visit: http://localhost:5000/swagger
```

### Option B — Full Docker stack (NGINX + App)
```bash
docker-compose up --build
# Visit: http://localhost        (through NGINX)
# Visit: http://localhost/swagger (Swagger UI)
```

### Stop containers
```bash
docker-compose down
```

---

## 📡 API Endpoints (via NGINX on port 80)

| Method | Endpoint | What it does |
|--------|----------|-------------|
| GET | `/api/media2` | Get all media items |
| GET | `/api/media2/{id}` | Get one item |
| POST | `/api/media2` | Create new item |
| DELETE | `/api/media2/{id}` | Delete item |
| GET | `/api/media2/health` | Health check |
| GET | `/api/media2/container-info` | Shows Docker container details |

---

## 🧪 Run tests
```bash
dotnet test
# Expected: Passed! - 10 tests ✅
```

---

## 📈 What this demonstrates on your CV

| Skill | Evidence |
|---|---|
| Docker containerization | Multi-stage Dockerfile |
| Container networking | NGINX + App on shared Docker network |
| Reverse proxy config | NGINX forwarding traffic to ASP.NET |
| docker-compose | Multi-service orchestration |
| Security | App not exposed directly, non-root user |
| CI/CD | 3-job pipeline with integration tests |

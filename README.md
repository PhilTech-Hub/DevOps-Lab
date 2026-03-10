# 🎬 Media Content Management System

> A REST API for managing media assets — images, videos, audio, and documents.
> Built with ASP.NET Core 8, Entity Framework, Docker, and GitHub Actions CI/CD.

---

## 🗂️ Project structure

```
MediaContentManagement/
├── .github/
│   └── workflows/
│       └── ci-cd.yml                  ← GitHub Actions pipeline
│
├── MediaApp/                          ← The ASP.NET Web API
│   ├── Controllers/
│   │   └── MediaApiController.cs      ← All REST endpoints (GET, POST, PUT, DELETE)
│   ├── Models/
│   │   └── MediaModels.cs             ← MediaItem, DTOs, enums
│   ├── Services/
│   │   └── MediaService.cs            ← Business logic (create, update, search...)
│   ├── Data/
│   │   └── AppDbContext.cs            ← EF Core database context + seed data
│   ├── Properties/
│   │   └── launchSettings.json        ← Runs on http://localhost:5000
│   ├── appsettings.json
│   ├── Program.cs                     ← Dependency injection + Swagger setup
│   └── MediaApp.csproj
│
├── MediaApp.Tests/                    ← All tests (11 total)
│   ├── MediaServiceTests.cs           ← 7 unit tests + 4 integration tests
│   └── MediaApp.Tests.csproj
│
├── Dockerfile                         ← Multi-stage Docker build
├── docker-compose.yml                 ← Runs app + SQL Server together
├── .gitignore
├── .dockerignore
└── MediaApp.sln
```

---

## 🛠️ Run locally

### Option A — dotnet run (quickest)
```bash
cd MediaApp
dotnet run
```
Open browser → `http://localhost:5000`
You'll see **Swagger UI** — an interactive page to test all API endpoints.

### Option B — Docker Compose (full stack with database)
```bash
docker-compose up
```
Open browser → `http://localhost:8080`

---

## 🧪 Run tests
```bash
dotnet test
# Expected: Passed! - 11 tests ✅
```

---

## 📡 API Endpoints

| Method | Endpoint | What it does |
|--------|----------|-------------|
| GET | `/api/media` | Get all media items |
| GET | `/api/media?type=Video` | Filter by type (Image/Video/Audio/Document) |
| GET | `/api/media?tag=banner` | Filter by tag |
| GET | `/api/media/{id}` | Get one item by ID |
| GET | `/api/media/search?q=banner` | Search by title or description |
| POST | `/api/media` | Upload/register a new media item |
| PUT | `/api/media/{id}` | Update a media item |
| DELETE | `/api/media/{id}` | Delete a media item |
| POST | `/api/media/{id}/publish` | Publish a draft item |
| GET | `/api/media/health` | Health check |

---

## ⚙️ Push to GitHub

```bash
git init
git add .
git commit -m "feat: Media Content Management System with CI/CD"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/DevOps-Lab.git
git push -u origin main
```

Go to **Actions tab** → watch the pipeline run automatically ✅
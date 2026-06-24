# Enterprise IDP — Internal Developer Platform

A portfolio-grade Internal Developer Platform inspired by Spotify Backstage and modern Platform Engineering practices, built with **Clean Architecture**, **DDD**, and **CQRS**.

> Status: 🟢 Phase 1 — Core platform running end-to-end (Auth, Service Catalog, CI/CD, GitOps, Kubernetes namespace provisioning)

## Tech Stack

- **Backend:** ASP.NET Core 9 Web API, EF Core 9 + Npgsql, MediatR (CQRS), FluentValidation, AutoMapper, Serilog
- **Database:** PostgreSQL
- **Cache:** Redis
- **Auth:** JWT Bearer, BCrypt password hashing
- **Integrations:** Octokit (GitHub), simulated Kubernetes service (Phase 1 stub)
- **Containerization:** Docker Compose (local dev)

## Architecture

Clean Architecture with strict dependency direction:
Domain → Application → Infrastructure → API
- **Domain:** Entities, Value Objects, Domain Events — no external dependencies. Entities use private constructors + static `Create()` factories returning `ErrorOr<T>`.
- **Application:** CQRS handlers (MediatR), validation pipeline behavior, application interfaces (ports).
- **Infrastructure:** EF Core persistence, Repository<T> + UnitOfWork, JWT/password services, GitHub/Kubernetes service implementations (adapters).
- **API:** Controllers, middleware (global exception handling), DI wiring, Swagger.

## Features (Phase 1)

| Module | Capabilities |
|---|---|
| **Auth** | Register, Login (JWT), Team creation, RBAC roles (Admin / Platform Engineer / Developer) |
| **Service Catalog** | Register services, list/filter/search, service detail with tags & dependencies |
| **CI/CD** | Trigger pipeline runs, list pipelines per service |
| **GitOps** | GitHub repository creation via Octokit |
| **Kubernetes** | Namespace provisioning (simulated), resource quotas, deployment listing |

## Project Structure

enterprise-idp/

├── src/backend/

│   ├── EnterpriseIDP.Domain/          # Entities, Value Objects, Events

│   ├── EnterpriseIDP.Application/     # CQRS Features, Interfaces, Behaviors

│   ├── EnterpriseIDP.Infrastructure/  # EF Core, Repositories, Auth, GitHub/K8s services

│   ├── EnterpriseIDP.API/             # Controllers, Program.cs, Swagger

│   └── EnterpriseIDP.Contracts/       # Shared contracts (WIP)

├── infrastructure/                    # Terraform, Helm, Kubernetes manifests (Phase 2+)

├── monitoring/                        # Prometheus, Grafana, Loki configs (Phase 2+)

└── docker-compose.yml                 # Local Postgres + Redis

## Getting Started

### Prerequisites
- .NET 9 SDK
- Docker Desktop

### 1. Start infrastructure (Postgres + Redis)
```bash
docker compose up -d
```

### 2. Apply database migrations
```bash
cd src/backend/EnterpriseIDP.Infrastructure
dotnet ef database update --startup-project ../EnterpriseIDP.API --project .
```

### 3. Run the API
```bash
cd ../EnterpriseIDP.API
dotnet run
```

API will be available at `http://localhost:5139`, Swagger UI at `http://localhost:5139/swagger`.

### 4. Try it out
1. `POST /api/auth/register` — create a user
2. `POST /api/auth/login` — get a JWT
3. Use the token (`Authorize` button in Swagger) to call `POST /api/auth/teams`, `POST /api/services`, etc.

## Configuration

Key settings in `src/backend/EnterpriseIDP.API/appsettings.json`:

| Key | Purpose |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `Jwt:Secret` / `Issuer` / `Audience` | JWT signing config — **change `Secret` before any real deployment** |
| `GitHub:OrgName` / `AccessToken` | Used by `GitHubService` for repository creation |

## Known Limitations (Phase 1)

- `IKubernetesService` is an **in-memory simulation** — no real cluster calls yet. Will be replaced with a real Kubernetes client in Phase 2.
- `AutoMapper 13.0.1` has a known NuGet advisory (NU1903) — scheduled for upgrade.
- `AspNet.Security.OAuth.GitHub` resolves to 9.0.0 instead of pinned 8.6.0 — harmless version mismatch, to be pinned explicitly.
- No automated tests yet.

## Roadmap

- **Phase 2:** Observability (Prometheus/Grafana/Loki), real GitOps (ArgoCD), Terraform-backed environment provisioning.
- **Phase 3:** Vault secrets management, cost management, incident management, DevSecOps scanning (SonarQube, Trivy).

## License

Personal portfolio project.

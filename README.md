# Enterprise IDP — Internal Developer Platform

A portfolio-grade Internal Developer Platform inspired by Spotify Backstage and modern Platform Engineering practices, built with **Clean Architecture**, **DDD**, and **CQRS**.

> Status: 🟢 Phase 1 — Core platform running end-to-end (Auth, Service Catalog verified live; CI/CD, GitOps, Kubernetes implemented but not yet manually verified)

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

| Module | Capabilities | Verified? |
|---|---|---|
| **Auth** | Register, Login (JWT), Team creation, RBAC roles (Admin / Platform Engineer / Developer) | ✅ Register, Team create/list verified live |
| **Service Catalog** | Register services, list/filter/search, service detail with tags & dependencies | ✅ Create/list verified live (tags has a known bug, see below) |
| **CI/CD** | Trigger pipeline runs, list pipelines per service | ⏳ Implemented, not yet manually tested |
| **GitOps** | GitHub repository creation via Octokit | ⏳ Implemented, **no controller route exposed yet** |
| **Kubernetes** | Namespace provisioning (simulated), resource quotas, deployment listing | ⏳ Implemented, not yet manually tested |

## Project Structure

enterprise-idp/

├── src/backend/

│   ├── EnterpriseIDP.Domain/          # Entities, Value Objects, Events

│   ├── EnterpriseIDP.Application/     # CQRS Features, Interfaces, Behaviors

│   ├── EnterpriseIDP.Infrastructure/  # EF Core, Repositories, Auth, GitHub/K8s services

│   ├── EnterpriseIDP.API/             # Controllers, Program.cs, Swagger

│   └── EnterpriseIDP.Contracts/       # Shared contracts (currently unused, WIP)

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
> Note: migrations also auto-apply on API startup in the Development environment, so this step is optional for local dev.

### 3. Run the API
```bash
cd ../EnterpriseIDP.API
dotnet run
```

API will be available at `http://localhost:5139`, Swagger UI at `http://localhost:5139/swagger`.

> **Windows users:** if you restart the API after a code change, make sure the previous `dotnet run` process has actually exited first. A still-running process locks `EnterpriseIDP.API.exe` and the next build fails with `MSB3027: Could not copy ... apphost.exe`. Find and stop it with `tasklist | grep EnterpriseIDP` then `taskkill //PID <pid> //F`.

### 4. Try it out
1. `POST /api/auth/register` — create a user (body: `firstName`, `lastName`, `email`, `password`, `role`)
2. `POST /api/auth/login` — get a JWT
3. Use the token (`Authorize` button in Swagger, or `Authorization: Bearer <token>` header with curl) to call `POST /api/auth/teams`, `GET /api/auth/teams`, `POST /api/services`, `GET /api/services`, etc.

## Configuration

Key settings in `src/backend/EnterpriseIDP.API/appsettings.json`:

| Key | Purpose |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `Jwt:Secret` / `Issuer` / `Audience` | JWT signing config — **change `Secret` before any real deployment** |
| `GitHub:OrgName` / `AccessToken` | Used by `GitHubService` for repository creation |

## Known Issues (found during manual verification, 2026-06-24)

- **Service `tags` not returned by `GET /api/services`** — tags are accepted on create but always come back empty on read. Likely the tag relationship isn't persisted or isn't included in the query.
- **Service `owner` field is overridden** — request sends a display name string, but the response returns the authenticated user's GUID instead. Behavior needs to be decided and made consistent.
- **Team `memberCount` stays 0 after creation** — the team owner doesn't appear to be added as a `TeamMember` automatically.
- **No `GitOpsController`** — `CreateRepository`/`GetRepositories` use cases exist in the Application layer but aren't wired to any HTTP route yet.

See `PROGRESS.md` for the full verification log and next steps.

## Known Limitations (Phase 1)

- `IKubernetesService` is an **in-memory simulation** — no real cluster calls yet. Will be replaced with a real Kubernetes client in Phase 2.
- `AutoMapper 13.0.1` has a known NuGet advisory (NU1903) — scheduled for upgrade.
- `AspNet.Security.OAuth.GitHub` resolves to 9.0.0 instead of pinned 8.6.0 — harmless version mismatch, to be pinned explicitly.
- No automated tests yet.
- No frontend yet — backend only.

## Roadmap

- **Phase 1 (current):** Finish verifying CI/CD, Kubernetes, GitOps endpoints; fix known bugs; build the React frontend.
- **Phase 2:** Observability (Prometheus/Grafana/Loki), real GitOps (ArgoCD), Terraform-backed environment provisioning.
- **Phase 3:** Vault secrets management, cost management, incident management, DevSecOps scanning (SonarQube, Trivy).

## License

Personal portfolio project.

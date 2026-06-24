# Enterprise IDP — Progress Tracker (Phase 1)

> Last updated: 2026-06-24
> Status: 🟢 Phase 1 backend verified end-to-end (Auth, Team, Service Catalog working via live API calls). CI/CD, GitOps, Kubernetes endpoints not yet manually verified. Frontend not started.

## Architecture
- Clean Architecture (Domain → Application → Infrastructure → API)
- CQRS via MediatR
- DDD: private setters, static `Create()` factories, `ErrorOr<T>` for domain validation
- .NET 9, EF Core 9 + Npgsql, FluentValidation, AutoMapper, Serilog, Redis, Octokit, JWT Bearer

## Layer Status

### Domain — ✅ Complete
- Entities: User, Team, TeamMember, Service, ServiceDependency, ServiceTag, Pipeline, PipelineRun, Repository, KubernetesNamespace, KubernetesDeployment
- ValueObjects: Email, ServiceSlug
- Enums: UserRole, ServiceType, ServiceStatus, PipelineStatus, DeploymentStatus, EnvironmentType
- Domain Events: UserCreatedEvent, TeamCreatedEvent, ServiceRegisteredEvent, RepositoryCreatedEvent
- Common: BaseEntity, IRepository<T>, IUnitOfWork, IDomainEvent

### Application — ✅ Complete (CQRS handlers wired, validators present)
- Features: Auth (RegisterUser, LoginUser, CreateTeam, GetCurrentUser, GetTeams), Catalog (RegisterService, GetServices, GetServiceById), CICD (TriggerPipeline, GetPipelines), GitOps (CreateRepository, GetRepositories), Kubernetes (CreateNamespace, GetDeployments)
- Common: LoggingBehavior, ValidationBehavior (MediatR pipeline), custom exceptions, `ErrorOrExtensions`
- Interfaces (ports): `ICurrentUserService`, `IGitHubService`, `IJwtTokenGenerator`, `IKubernetesService`, `IPasswordHasher`

### Infrastructure — ✅ Complete
- `ApplicationDbContext` with EF Core Fluent API configurations for every entity
- `Repository<T>` generic implementation + `UnitOfWork`
- `JwtTokenGenerator`, `PasswordHasher` (BCrypt), `CurrentUserService`
- `GitHubService` (Octokit-based), `KubernetesService` (Phase 1: simulated, not a real cluster client)
- First migration generated and applied: `20260624054047_InitialCreate`

### API — ✅ Complete for Phase 1 scope
- `Program.cs`: Serilog, JWT Bearer auth, CORS for frontend dev, Swagger with Bearer security scheme, global exception middleware, auto-migrate on startup (Development only)
- Controllers: `AuthController`, `CatalogController`, `CICDController`, `KubernetesController`
- ⚠️ No dedicated `GitOpsController` yet — GitOps handlers exist in Application layer but are not exposed via any route

### Contracts — ❌ Still empty
- Only default `Class1.cs`, not yet used. Decide whether this project is needed for Phase 1 or can be folded into Application/API DTOs.

## Build Status
- ✅ `dotnet build` — 0 Errors, 6 Warnings (NuGet advisories, see Tech Debt)
- ✅ `dotnet run` — starts cleanly, applies migrations, listens on `http://localhost:5139`

## Manually Verified (via curl, 2026-06-24)
- [x] `POST /api/auth/register` → returns user id + role + signed JWT (claims: `sub`, `email`, role claim, `jti`, `exp`, `iss`, `aud`)
- [x] `POST /api/auth/teams` → creates team; correctly returns `409 Conflict` on duplicate name
- [x] `GET /api/auth/teams` → **was missing, fixed today** (route added to `AuthController`, delegates to existing `GetTeamsQuery`/`GetTeamsHandler`) — now returns team list correctly
- [x] `POST /api/services` → creates service, auto-generates slug, validates `teamId` is non-empty and exists
- [x] `GET /api/services` → returns paginated list (`items`, `totalCount`, `page`, `pageSize`)
- [ ] `POST /api/pipelines/trigger`, `GET /api/pipelines` — not yet tested
- [ ] `POST /api/kubernetes/namespaces`, `GET /api/kubernetes/deployments` — not yet tested
- [ ] GitOps (`CreateRepository`, `GetRepositories`) — not yet tested, and **no controller route exists for these yet**

## Known Bugs (found during manual verification, not yet fixed)
1. **Service `tags` not persisted/returned** — `POST /api/services` accepts a `tags` array in the request, but `GET /api/services` always returns `tags: []`. Likely `ServiceTag` relationship isn't being saved in `RegisterServiceHandler`, or `GetServicesHandler` isn't including the navigation property.
2. **Service `owner` field mismatch** — request sends `owner` as a display name string (e.g. `"Admin User"`), but the stored/returned `owner` is the current user's GUID. Need to decide: should `owner` be a free-text field, or should it always resolve to the authenticated user's id? Pick one and fix the handler/DTO accordingly.
3. **Team `memberCount` stays 0 after creation** — `POST /api/auth/teams` sets an `ownerId` but does not appear to create a corresponding `TeamMember` row for the owner, so `GET /api/auth/teams` shows `memberCount: 0` even right after creation with an owner.
4. **No `GitOpsController`** — `CreateRepositoryCommand`/`GetRepositoriesQuery` exist in Application layer but have no HTTP route. Needs a controller (`/api/gitops/repositories` or similar).

## Immediate Next Steps (in order)
1. Manually verify CI/CD endpoints (`/api/pipelines`, `/api/pipelines/trigger`)
2. Manually verify Kubernetes endpoints (`/api/kubernetes/namespaces`, `/api/kubernetes/deployments`)
3. Add `GitOpsController` and verify GitOps endpoints
4. Fix known bugs #1–#3 above
5. Decide on `EnterpriseIDP.Contracts` project's purpose (or remove it)
6. Start frontend (React + TypeScript) — login/register pages, service catalog list/detail, team management
7. Add automated tests (unit tests for handlers/validators, integration tests for controllers)
8. Move into Phase 2 scope (Observability, real GitOps via ArgoCD, Terraform)

## Known Tech Debt / Things to Revisit
- `AutoMapper 13.0.1` has a known high-severity vulnerability (NU1903 warning) — consider upgrading or replacing with manual mapping
- `AspNet.Security.OAuth.GitHub` pinned to `>= 8.6.0` but NuGet resolves `9.0.0` — version mismatch warning, should pin explicitly in `.csproj`
- `IKubernetesService` is a Phase 1 in-memory simulation, not a real cluster client — fine for now, must be replaced before claiming "Kubernetes Platform" is real
- No automated tests yet
- Windows dev note: stop the running `dotnet run` process (or it will lock `EnterpriseIDP.API.exe` and the next `dotnet build`/`dotnet run` will fail with `MSB3027`/`MSB3021`). Find the PID via the terminal running it, or `tasklist | grep EnterpriseIDP`, then `taskkill //PID <pid> //F`.

## Session Log
- **2026-06-23**: Initial Application/Infrastructure scaffolding in progress, build not passing yet (see git history for earlier state — this file previously tracked that session).
- **2026-06-24**: Git repo initialized, `.gitignore` added, first commit (`edeb224`, 118 files). Build verified passing. Postgres + Redis brought up via Docker Compose. Backend run-verified. Auth, Team, and Service Catalog flows manually tested end-to-end via curl with real JWT tokens. Missing `GET /api/auth/teams` route discovered and fixed. Three minor bugs identified (tags, owner field, member count) and logged above for future fix.

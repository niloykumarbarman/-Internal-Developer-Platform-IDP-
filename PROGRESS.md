# Enterprise IDP — Progress Tracker (Phase 1)

> Last updated: 2026-06-23
> Status: 🟡 IN PROGRESS — Infrastructure layer being wired up, build not yet passing

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

### Application — 🟡 Mostly complete, bugs being fixed
- Features scaffolded: Auth, Catalog, CICD, GitOps, Kubernetes
- **Known issues fixed so far:**
  - [x] `CreateRepositoryHandler` — removed `IConfiguration` dependency (moved org name to `IGitHubService.OrgName`), switched to `Repository.Create()` factory + `ErrorOrExtensions.ThrowIfError()` bridge
  - [x] Added `ErrorOrExtensions.ThrowIfError<T>()` to bridge `ErrorOr<T>` domain results into `ValidationException` (FluentValidation-based)
  - [ ] `RegisterUserHandler` — still uses `new User(...)` directly (invalid, User has private ctor) — **NOT YET FIXED**
  - [ ] `LoginUserHandler` — still treats `IRepository.FindAsync` (returns a list) as a single nullable entity — **NOT YET FIXED**
- `IGitHubService` interface updated: removed `orgName` param from methods, added `OrgName` property, changed `CreateRepositoryAsync` to return `(long RepoId, string CloneUrl, string HtmlUrl)`

### Infrastructure — 🟡 In progress
- [x] `ApplicationDbContext` created (all DbSets wired, soft-delete global query filter applied)
- [ ] Repository<T> generic implementation — **NOT STARTED**
- [ ] UnitOfWork implementation — **NOT STARTED**
- [ ] JwtTokenGenerator — **NOT STARTED**
- [ ] PasswordHasher (BCrypt or ASP.NET Identity hasher) — **NOT STARTED**
- [ ] CurrentUserService — **NOT STARTED**
- [ ] GitHubService (Octokit-based) — **NOT STARTED**
- [ ] KubernetesService — **NOT STARTED**
- [ ] EF Core entity Configurations (Fluent API for keys, indexes, relationships) — **NOT STARTED**
- [ ] Migrations — **NOT STARTED** (blocked until configs + DbContext are final)

### API — ❌ Not started
- `Program.cs` is still the default ASP.NET template (weather forecast demo)
- No DI registration, no DbContext wiring, no JWT auth middleware, no Controllers/Minimal API endpoints, no Swagger config, no Serilog setup

### Contracts — ❌ Empty
- Only default `Class1.cs`, not yet used

## Build Status
- ❌ Solution does not build yet
- Last known error (fixed): `CS0234`/`CS0246` — `IConfiguration` missing in Application layer (GitOps handler) — fix applied, not yet re-verified with `dotnet build`
- **Next build check needed** after Repository<T>, UnitOfWork, and Auth handler fixes are in place

## Immediate Next Steps (in order)
1. Verify `CreateRepositoryHandler` fix compiles
2. Fix `RegisterUserHandler` and `LoginUserHandler` (factory methods + correct repository usage)
3. Implement `Repository<T>` + `UnitOfWork` in Infrastructure
4. Implement `PasswordHasher`, `JwtTokenGenerator`, `CurrentUserService`
5. Implement `GitHubService`, `KubernetesService` (stub/real)
6. Add EF Core entity configurations + generate first migration
7. Wire up `Program.cs` (DbContext, DI, JWT auth, Swagger, Serilog)
8. Add Controllers/Minimal API endpoints for Auth + Catalog
9. First successful `dotnet build` across the whole solution
10. Replace this file with a polished `README.md` once Phase 1 runs end-to-end

## Known Tech Debt / Things to Revisit
- `AutoMapper 13.0.1` has a known high-severity vulnerability (NU1903 warning) — consider upgrading or replacing with manual mapping
- `AspNet.Security.OAuth.GitHub` pinned to 8.6.0 but NuGet resolves 9.0.0 — version mismatch warning, should pin explicitly or update csproj
- No `.sln` file found at repo root — confirm solution structure before CI/CD setup

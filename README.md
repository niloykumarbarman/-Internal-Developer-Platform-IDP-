<div align="center">

# 🚀 Enterprise Internal Developer Platform (IDP)

**A production-style Internal Developer Platform inspired by Spotify Backstage and modern Platform Engineering practices.**

Built to demonstrate senior-level skills in **Platform Engineering, DevOps, Cloud-Native architecture, Kubernetes, GitOps, DevSecOps, Observability, and Infrastructure Automation.**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?logo=postgresql)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7-DC382D?logo=redis)](https://redis.io/)
[![Kubernetes](https://img.shields.io/badge/Kubernetes-Helm%20%2B%20ArgoCD-326CE5?logo=kubernetes)](https://kubernetes.io/)
[![Terraform](https://img.shields.io/badge/Terraform-IaC-7B42BC?logo=terraform)](https://www.terraform.io/)
[![Vault](https://img.shields.io/badge/HashiCorp-Vault-FFEC6E?logo=vault&logoColor=black)](https://www.vaultproject.io/)
[![Live](https://img.shields.io/badge/Status-Live%20on%20Render-brightgreen?logo=render)](https://enterprise-idp-backend.onrender.com/health/live)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

[Architecture](docs/architecture.md) · [API Guide](docs/api-guide.md) · [Deployment Guide](docs/deployment-guide.md) · [Progress Tracker](PROGRESS.md)

</div>

---

## 🌐 Live Demo

> Deployed on **Render** with full CI/CD via GitHub → Render Blueprint (`render.yaml`)

| Service | URL | Status |
|---------|-----|--------|
| 🖥️ Frontend | [enterprise-idp-frontend.onrender.com](https://enterprise-idp-frontend.onrender.com) | 🟢 Live |
| ⚙️ Backend API | [enterprise-idp-backend.onrender.com/swagger](https://enterprise-idp-backend.onrender.com/swagger) | 🟢 Live |
| ❤️ Health Check | [/health/live](https://enterprise-idp-backend.onrender.com/health/live) | 🟢 Healthy |

---

## 📖 What Is This?

Most platform teams stitch together GitHub, Kubernetes, Terraform, Vault, Prometheus, and a CI/CD system by hand for every new service. An **Internal Developer Platform** wraps all of that behind one self-service API and UI — so a developer can scaffold a service, get a namespace, a pipeline, monitoring, and secrets management, without filing five tickets to five different teams.

This repo is a from-scratch implementation of that idea, built backend-first with **Clean Architecture, DDD, and CQRS**, and rolled out in three deliberate phases rather than all at once — because a fresher/portfolio project that claims fifteen enterprise features built simultaneously isn't credible. Building it in verifiable phases is.

---

## 🧱 Tech Stack

| Layer | Technology |
|---|---|
| **Backend** | ASP.NET Core 9 Web API, C#, MediatR (CQRS), EF Core |
| **Database** | PostgreSQL 16 (Supabase cloud) |
| **Cache** | Redis 7 (Upstash cloud) |
| **Auth** | JWT (access + refresh tokens), GitHub OAuth2, RBAC |
| **Frontend** | React 19, TypeScript 6, Vite 8, TanStack Query v5, shadcn/ui, Radix UI, Zustand, React Router v7, Axios, Tailwind CSS v4, Lucide React — ✅ implemented |
| **Containerization** | Docker, Docker Compose |
| **Orchestration** | Kubernetes, Helm, ArgoCD (GitOps) |
| **IaC** | Terraform + Render Blueprint (`render.yaml`) |
| **Secrets** | HashiCorp Vault (Kubernetes auth method) |
| **Observability** | Prometheus, Grafana Cloud, Loki, Promtail, OpenTelemetry, Jaeger, Alertmanager |
| **CI/CD** | GitHub Actions, GHCR, Render Auto Deploy |
| **DevSecOps** | Trivy, CodeQL, Checkov, Gitleaks, SonarCloud |
| **Hosting** | Render.com (Blueprint IaC deployment) |

---

## 🏗️ Architecture

### Cloud Deployment Architecture (Render)

```
┌─────────────────────────────────────────────────────────┐
│                     GitHub Repository                    │
│              (Push → Auto Deploy via Render)             │
└──────────────────────┬──────────────────────────────────┘
                       │ render.yaml Blueprint
         ┌─────────────┴──────────────┐
         ▼                            ▼
┌────────────────┐          ┌──────────────────────┐
│    Frontend    │          │      Backend API      │
│ (React + Vite) │          │  (ASP.NET Core .NET)  │
│ Static Site    │◄────────►│  Web Service          │
│ Render CDN     │   REST   │  Port 8080            │
└────────────────┘          └──────────┬───────────┘
                                       │
                    ┌──────────────────┼──────────────────┐
                    ▼                  ▼                   ▼
           ┌──────────────┐  ┌──────────────┐  ┌──────────────────┐
           │  PostgreSQL  │  │    Redis     │  │  Grafana Cloud   │
           │  (Supabase)  │  │  (Upstash)   │  │  OTLP Metrics    │
           │  ap-southeast│  │  Mumbai      │  │  + Traces + Logs │
           └──────────────┘  └──────────────┘  └──────────────────┘
```

### Application Architecture (Clean Architecture + CQRS)

```
src/backend/
├── EnterpriseIDP.Domain          # Entities, enums, domain logic — no external dependencies
├── EnterpriseIDP.Application     # CQRS commands/queries, interfaces, validators (MediatR)
├── EnterpriseIDP.Infrastructure  # EF Core, Vault, Audit, Redis, external integrations
├── EnterpriseIDP.API             # Controllers, middleware, composition root
└── EnterpriseIDP.Contracts       # Shared DTOs
```

Full diagrams and data flow: [`docs/architecture.md`](docs/architecture.md)

---

## 🛣️ Build Roadmap — Why Phases?

Building "everything at once" doesn't survive a real code review. Each phase below ships a **working, demoable slice** before the next one starts — which is also how the commit history reads.

<table>
<tr><th>Phase</th><th>Theme</th><th>Status</th></tr>
<tr><td>1</td><td>Core Platform — Auth, Catalog, GitHub, CI/CD, Kubernetes</td><td>✅ Complete</td></tr>
<tr><td>2</td><td>Observability + GitOps + Terraform</td><td>✅ Complete</td></tr>
<tr><td>3</td><td>Vault + Incident Management + Audit + Cost + DevSecOps</td><td>✅ Complete (backend + dashboards)</td></tr>
<tr><td>4</td><td>React + TypeScript Frontend</td><td>✅ Implemented (15 routed pages)</td></tr>
<tr><td>5</td><td>Cloud Deployment — Render Blueprint, Supabase, Upstash, Grafana Cloud</td><td>✅ Live in Production</td></tr>
</table>

---

## ✅ Phase 1 — Core Platform

The foundation: a developer can authenticate, register a service, push code, and get it running in Kubernetes.

**Authentication & Authorization**
- JWT authentication with access + refresh tokens
- GitHub OAuth2 login flow
- Role-Based Access Control — `Admin`, `PlatformEngineer`, `Developer`

**Service Catalog**
- Full CRUD on services with metadata, ownership, and search
- Service dependency mapping

**GitOps & CI/CD Foundations**
- GitHub integration: repository creation, webhook registration
- CI/CD trigger API for build pipelines

**Kubernetes Platform**
- Namespace provisioning API
- Deployment management with rolling updates and HPA

**Infrastructure**
- Docker Compose for full local development (Postgres + Redis + backend)
- Base Helm chart for the platform itself
- ArgoCD "App of Apps" pattern introduced
- Baseline Kubernetes manifests (Deployments, HPA)

| Feature | Status |
|---|---|
| CQRS with MediatR | ✅ |
| PostgreSQL + EF Core migrations | ✅ |
| Redis caching (`ICacheService`) | ✅ |
| JWT auth (access + refresh) | ✅ |
| GitHub OAuth2 | ✅ |
| RBAC (Admin / PlatformEngineer / Developer) | ✅ |
| Service Catalog API (CRUD + search) | ✅ |
| GitHub integration (repo creation, webhooks) | ✅ |
| CI/CD trigger API | ✅ |
| Kubernetes namespace provisioning | ✅ |
| Docker Compose (local dev, all services) | ✅ |
| Helm chart (base) | ✅ |
| ArgoCD GitOps (App of Apps) | ✅ |
| Kubernetes manifests (Deployments, HPA) | ✅ |

---

## ✅ Phase 2 — Observability + GitOps + Terraform

With the platform running, Phase 2 answers: *can we see what's happening, automate the rollout, and provision infrastructure as code instead of by hand?*

**Observability Stack**
- **Prometheus** for metrics collection, with custom alerting rules
- **Grafana** dashboards for the IDP overview and Kubernetes cluster health
- **Loki + Promtail** for centralized log aggregation and shipping
- **Alertmanager** wired to email + webhook notification channels
- **OpenTelemetry Collector** unifying traces, metrics, and logs
- **Jaeger** for distributed tracing, deployed via Helm
- Grafana datasource provisioning for Prometheus, Loki, and Jaeger in one pane

**Infrastructure as Code**
- Terraform **root module** (`main` + `variables` + `outputs`) as the entry point
- Terraform **Kubernetes module** — namespace creation, deployment, HPA, all parameterized
- Terraform **Database module** — automated PostgreSQL provisioning

| Feature | Status | Notes |
|---|---|---|
| Prometheus config | ✅ | Alerts + scrape rules |
| Grafana dashboards | ✅ | IDP overview + K8s health |
| Loki config | ✅ | Log aggregation |
| Promtail config | ✅ | Log shipping agent |
| Alertmanager config | ✅ | Email + webhook routing |
| OpenTelemetry Collector | ✅ | Traces + metrics + logs |
| Jaeger tracing | ✅ | Deployed via Helm |
| Grafana datasources | ✅ | Prometheus + Loki + Jaeger |
| Terraform root module | ✅ | `main` + `variables` + `outputs` |
| Terraform Kubernetes module | ✅ | Namespace + Deploy + HPA |
| Terraform Database module | ✅ | PostgreSQL provisioning |

---

## ✅ Phase 3 — Vault, Incident Management, Audit & Cost

This is the phase that turns the platform from "deploys things" into "operates responsibly." It answers three questions a real platform team gets asked constantly: *where are our secrets, what broke and why, and what is this costing us?*

### 🔐 Secrets Management — HashiCorp Vault
- `VaultService` integration via the **Kubernetes auth method** — the API authenticates to Vault using its own pod's service account identity, not a static root token baked into config
- Full secret lifecycle: get / set / delete / **rotate**
- Vault policies for `idp-admin`, `idp-platform-engineer`, and `idp-developer`, scoped by path
- `VaultController` REST API with role-gated access (`Admin`, `PlatformEngineer`)
- Health-check endpoint for Vault connectivity

### 🚨 Incident Management
- Full domain model: `Incident`, `Timeline`, `Postmortem`
- CQRS commands/queries: create, update, resolve, add timeline events, generate postmortems
- Severity (`Low → Critical`) and status (`Open → Closed`) state machine
- Aggregate incident stats (open count, breakdown by severity)
- Every incident action automatically writes to the audit trail

### 📋 Audit & Compliance
- `AuditService` capturing action, entity, user, IP address, before/after values, and success/failure on every sensitive operation
- Filterable audit log API (by user, entity type, action, date range)
- Aggregate stats: actions by type, by entity, by user, success vs. failure rate

### 💰 Cost Management
- `CostReport` entity and reporting API per team / environment
- Budget alert thresholds with configurable periods

### 🛡️ DevSecOps Pipeline
- **Trivy** — filesystem and container image vulnerability scanning (SARIF → GitHub Security tab)
- **CodeQL** — static analysis for both C# and JavaScript/TypeScript
- **Checkov** — IaC security scanning for Helm charts and Kubernetes manifests
- **Gitleaks** — secret-scanning on every push
- **SonarCloud** — code quality gate (`sonar-project.properties`) wired into CI
- Dependency vulnerability scanning for both NuGet and npm

| Feature | Status | Notes |
|---|---|---|
| Vault integration (Kubernetes auth) | ✅ | `VaultService.cs` |
| Vault secret CRUD + rotation API | ✅ | `VaultController.cs` |
| Vault policies (admin/engineer/developer) | ✅ | Path-scoped HCL policies |
| Incident domain + CQRS | ✅ | Commands, queries, handlers |
| Incident timeline + postmortems | ✅ | Full REST API |
| Audit logging service | ✅ | Wired into Incident lifecycle |
| Audit log API + stats | ✅ | Filterable, paginated |
| Cost reports + budget alerts | ✅ | Team/environment scoped |
| EF Core migration (Phase 3 tables) | ✅ | `Phase3_Incidents_Audit_Cost` |
| Trivy + CodeQL + Checkov + Gitleaks (CI) | ✅ | `security-scan.yml` |
| SonarCloud config | ✅ | `sonar-project.properties` |
| API documentation | ✅ | `docs/api-guide.md` |
| DevSecOps / Incident dashboards (frontend) | ✅ | DevSecOpsPage.tsx, IncidentPage.tsx |
| Compliance reports (PDF export) | ⏳ Pending | Next up |

---

## ✅ Phase 5 — Cloud Deployment (Production)

Taking the platform from local Docker Compose to **fully managed cloud infrastructure**, deployed and live.

### ☁️ Infrastructure
| Component | Provider | Details |
|-----------|----------|---------|
| Backend Hosting | [Render.com](https://render.com) | Web Service, auto-deploy on push |
| Frontend Hosting | [Render.com](https://render.com) | Static Site, global CDN |
| Database | [Supabase](https://supabase.com) | PostgreSQL, `ap-southeast-2`, SSL enforced |
| Cache | [Upstash](https://upstash.com) | Redis, Mumbai region, TLS enabled |
| Observability | [Grafana Cloud](https://grafana.com) | OTLP metrics + traces, `ap-south-1` |
| IaC | `render.yaml` Blueprint | One-click full stack deploy |
| CI/CD | GitHub → Render | Auto deploy on every `main` push |

### 🔧 Environment Variables (Production)
| Variable | Purpose |
|----------|---------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__DefaultConnection` | Supabase PostgreSQL |
| `ConnectionStrings__Redis` | Upstash Redis (TLS) |
| `Jwt__Secret` | JWT signing key |
| `Observability__OtlpEndpoint` | Grafana OTLP gateway |
| `OTEL_EXPORTER_OTLP_HEADERS` | Grafana auth (Basic) |
| `Frontend__Url` | CORS origin whitelist |

### 📊 Observability in Production
- **Metrics** → Grafana Cloud via `OtlpMetricExporter` (every 60s)
- **Traces** → Grafana Cloud via `OtlpTraceExporter` (HttpProtobuf)
- **Logs** → Serilog structured JSON with `RequestId`, `UserId`, `Application`, `Version`
- **Health** → `/health/live` polled every 5s by Render

---

## 🧪 Testing & Verification

| Layer | Status | Notes |
|---|---|---|
| Backend build (`dotnet build`) | ✅ 0 errors | 4 nuget version warnings (non-blocking) |
| Frontend build (`npm run build`) | ✅ Passing | TypeScript strict + Vite production build |
| Unit tests (`EnterpriseIDP.Tests.Unit`) | ✅ 25 passing | Domain value objects + entity business rules |
| Integration tests (`EnterpriseIDP.Tests.Integration`) | ✅ 11 passing | Auth flow + Catalog flow, `WebApplicationFactory` + EF Core InMemory |
| E2E tests (`EnterpriseIDP.Tests.E2E`) | ✅ 3 passing | Full user journey: register→login→create team→register service→verify |
| Docker Compose stack | ✅ Verified | Postgres, Redis, Prometheus, Grafana all healthy |
| Production health check | ✅ Live | `GET /health/live → 200 Healthy` |

Run tests locally:

```bash
cd src/backend
dotnet test EnterpriseIDP.Tests.Unit
dotnet test EnterpriseIDP.Tests.Integration
```

---

## 📋 Roadmap

- [x] **Phase 4** — React + TypeScript frontend (Vite): login, service catalog UI, incident dashboard, DevSecOps security dashboard, cost reports UI
- [x] **Phase 5** — Cloud deployment: Render + Supabase + Upstash + Grafana Cloud
- [ ] Compliance report PDF export
- [ ] Database self-service provisioning UI (Postgres + Redis on-demand)
- [ ] Production Helm values + multi-environment (dev/staging/prod) promotion flow

---

## 🚀 Local Setup

```bash
git clone https://github.com/niloykumarbarman/-Internal-Developer-Platform-IDP-.git
cd -Internal-Developer-Platform-IDP-/enterprise-idp

# Start Postgres, Redis, Prometheus, Grafana, exporters
docker compose up -d

# Run the backend API
cd src/backend
dotnet restore EnterpriseIDP.sln
dotnet ef database update --project EnterpriseIDP.Infrastructure --startup-project EnterpriseIDP.API
dotnet run --project EnterpriseIDP.API
```

API will be available at `http://localhost:5000`, with Swagger UI at `http://localhost:5000/swagger`.

### Local Kubernetes (kind) Deploy

```bash
# Prerequisites: Docker, kind, helm, kubectl
kind create cluster --name idp-local
docker build -t enterprise-idp-backend:latest src/backend/
docker build -t enterprise-idp-frontend:latest src/frontend/
kind load docker-image enterprise-idp-backend:latest enterprise-idp-frontend:latest --name idp-local
helm install idp helm/enterprise-idp -f helm/enterprise-idp/values-local.yaml --namespace idp --create-namespace
kubectl get pods -n idp
```

Verify:
```bash
kubectl port-forward -n idp svc/idp-backend 8080:8080 &
curl http://localhost:8080/health/ready
```

Full production deployment steps: [`docs/deployment-guide.md`](docs/deployment-guide.md)

---

## 📚 Documentation

| Doc | Description |
|---|---|
| [`docs/architecture.md`](docs/architecture.md) | System architecture, layer responsibilities, data flow |
| [`docs/api-guide.md`](docs/api-guide.md) | Full REST API reference for all 10 controllers |
| [`docs/deployment-guide.md`](docs/deployment-guide.md) | Production deployment via Kubernetes/Helm/ArgoCD/Render |
| [`PROGRESS.md`](PROGRESS.md) | Granular, file-level build progress tracker |

---

## 👤 Author

**Niloy Kumar Barman**

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0A66C2?logo=linkedin&logoColor=white)](https://www.linkedin.com/in/niloy-kumar-barman-552634339)
[![Email](https://img.shields.io/badge/Email-niloybarman829%40gmail.com-D14836?logo=gmail&logoColor=white)](mailto:niloybarman829@gmail.com)
[![GitHub](https://img.shields.io/badge/GitHub-niloykumarbarman-181717?logo=github&logoColor=white)](https://github.com/niloykumarbarman)

---

<div align="center">

*Built as a hands-on demonstration of Platform Engineering, DevOps, and Cloud-Native architecture — one verifiable phase at a time.*

</div>

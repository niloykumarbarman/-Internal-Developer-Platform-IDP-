# Enterprise IDP — API Guide

Base URL (local): `http://localhost:5000/api`
Interactive docs (Swagger UI): `http://localhost:5000/swagger`

All endpoints (except `/auth/*`, OAuth callbacks, and health checks) require a Bearer JWT:

```
Authorization: Bearer <access_token>
```

## Roles

| Role | Description |
|---|---|
| `Admin` | Full platform access, including secret deletion and destructive operations |
| `PlatformEngineer` | Manages infrastructure, Vault secrets, CI/CD, Kubernetes resources |
| `Developer` | Self-service: create services, trigger pipelines, view own team's resources |

Most endpoints are reachable by any authenticated role unless a controller/action specifies `[Authorize(Roles = "...")]`.

---

## Table of Contents

1. [Auth](#1-auth)
2. [Service Catalog](#2-service-catalog)
3. [GitOps](#3-gitops)
4. [CI/CD](#4-cicd)
5. [Kubernetes](#5-kubernetes)
6. [Observability](#6-observability)
7. [Vault (Secrets Management)](#7-vault-secrets-management)
8. [Incident Management](#8-incident-management)
9. [Audit Logs](#9-audit-logs)
10. [Cost Management](#10-cost-management)
11. [Error Format](#11-error-format)
12. [Pagination Convention](#12-pagination-convention)

---

## 1. Auth

`AuthController` — `/api/auth`

| Method | Endpoint | Description | Roles |
|---|---|---|---|
| POST | `/auth/register` | Register a new local account | Anonymous |
| POST | `/auth/login` | Login with email/password, returns JWT access + refresh token | Anonymous |
| POST | `/auth/refresh` | Exchange a refresh token for a new access token | Anonymous |
| GET | `/auth/github` | Redirect to GitHub OAuth2 consent screen | Anonymous |
| GET | `/auth/github/callback` | GitHub OAuth2 callback, issues JWT on success | Anonymous |
| POST | `/auth/logout` | Revoke the current refresh token | Authenticated |
| GET | `/auth/me` | Get current authenticated user profile + role | Authenticated |

**Example — Login**

```http
POST /api/auth/login
Content-Type: application/json

{ "email": "admin@idp.local", "password": "********" }
```

```json
{
  "accessToken": "eyJhbGciOi...",
  "refreshToken": "8f3a1c...",
  "expiresIn": 3600,
  "user": { "id": "...", "email": "admin@idp.local", "role": "Admin" }
}
```

---

## 2. Service Catalog

`CatalogController` — `/api/catalog`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/catalog` | List all registered services (supports search, owner, tag filters) |
| GET | `/catalog/{id}` | Get service details, including metadata and ownership |
| POST | `/catalog` | Register a new service in the catalog |
| PUT | `/catalog/{id}` | Update service metadata / ownership / lifecycle stage |
| DELETE | `/catalog/{id}` | Deregister a service (Admin/PlatformEngineer) |
| GET | `/catalog/{id}/dependencies` | Get upstream/downstream dependency graph for a service |
| POST | `/catalog/{id}/dependencies` | Add a dependency edge between two services |
| GET | `/catalog/templates` | List available Golden Path / scaffolding templates |
| POST | `/catalog/scaffold` | Create a new service from a template (project scaffolding) |

---

## 3. GitOps

`GitOpsController` — `/api/gitops`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/gitops/repositories` | Create a new GitHub repository from a template |
| GET | `/gitops/repositories/{name}` | Get repository details (branches, webhooks, last commit) |
| POST | `/gitops/repositories/{name}/branches` | Create a new branch |
| POST | `/gitops/repositories/{name}/pull-requests` | Open a pull request |
| GET | `/gitops/repositories/{name}/pull-requests` | List open pull requests |
| POST | `/gitops/repositories/{name}/webhooks` | Register a webhook (e.g. for CI triggers) |
| POST | `/gitops/argocd/applications` | Register an ArgoCD Application for a service |
| GET | `/gitops/argocd/applications/{name}/sync-status` | Get ArgoCD sync/health status |
| POST | `/gitops/argocd/applications/{name}/sync` | Trigger a manual ArgoCD sync |

---

## 4. CI/CD

`CICDController` — `/api/cicd`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/cicd/pipelines` | List pipelines for a service/repo |
| GET | `/cicd/pipelines/{id}` | Get pipeline run details (status, stages, logs link) |
| POST | `/cicd/pipelines/{id}/trigger` | Manually trigger a build |
| POST | `/cicd/pipelines/{id}/cancel` | Cancel a running pipeline |
| GET | `/cicd/pipelines/{id}/logs` | Stream/fetch build logs |
| POST | `/cicd/releases` | Create a release (tag + changelog) |
| GET | `/cicd/releases` | List releases for a service |
| POST | `/cicd/releases/{id}/rollback` | Roll back to a previous release |

---

## 5. Kubernetes

`KubernetesController` — `/api/kubernetes`

| Method | Endpoint | Description | Roles |
|---|---|---|---|
| POST | `/kubernetes/namespaces` | Provision a namespace (with default ResourceQuota) | PlatformEngineer, Admin |
| GET | `/kubernetes/namespaces` | List namespaces managed by the platform | — |
| GET | `/kubernetes/namespaces/{name}` | Get namespace details + quota usage | — |
| POST | `/kubernetes/deployments` | Create/update a Deployment | — |
| GET | `/kubernetes/deployments/{name}` | Get deployment status (replicas, rollout state) | — |
| POST | `/kubernetes/deployments/{name}/scale` | Manually scale replica count | — |
| POST | `/kubernetes/deployments/{name}/rollout/restart` | Trigger a rolling restart | — |
| POST | `/kubernetes/hpa` | Configure Horizontal Pod Autoscaler | PlatformEngineer, Admin |
| POST | `/kubernetes/ingress` | Configure Ingress rules for a service | PlatformEngineer, Admin |
| GET | `/kubernetes/clusters/health` | Cluster-wide health summary | — |

---

## 6. Observability

`ObservabilityController` — `/api/observability`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/observability/metrics/{service}` | Proxy key Prometheus metrics for a service |
| GET | `/observability/dashboards` | List available Grafana dashboards |
| GET | `/observability/dashboards/{uid}/url` | Get a deep-link URL into a specific Grafana dashboard |
| GET | `/observability/logs/{service}` | Query recent logs for a service (via Loki) |
| GET | `/observability/traces/{service}` | Query recent traces for a service (via Jaeger) |
| GET | `/observability/alerts` | List currently firing alerts (Alertmanager) |
| POST | `/observability/alerts/{id}/silence` | Silence an alert for a time window |

---

## 7. Vault (Secrets Management)

`VaultController` — `/api/vault`

| Method | Endpoint | Description | Roles |
|---|---|---|---|
| GET | `/vault/health` | Vault connectivity health check | Anonymous |
| GET | `/vault/{path}` | Get all secrets at a path | Admin, PlatformEngineer |
| GET | `/vault/{path}/{key}` | Get a single secret value | Admin, PlatformEngineer |
| POST | `/vault/{path}/{key}` | Set/update a secret | Admin, PlatformEngineer |
| POST | `/vault/{path}/{key}/rotate` | Rotate a secret value | Admin, PlatformEngineer |
| DELETE | `/vault/{path}/{key}` | Delete a secret | Admin |

Authentication into Vault itself is via the **Kubernetes auth method** (`IVaultService.LoginWithKubernetesAsync`) — the API's own pod identity authenticates to Vault rather than using a static root token, so no long-lived Vault credential is stored in the app config.

**Example — Set a secret**

```http
POST /api/vault/database/connection-string
Authorization: Bearer <token>
Content-Type: application/json

{ "value": "Host=...;Database=...;Username=...;Password=..." }
```

---

## 8. Incident Management

`IncidentController` — `/api/incident`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/incident` | List incidents — filter by `status`, `severity`, `affectedService`; paginated |
| GET | `/incident/stats` | Aggregate incident stats (open count, MTTR, by severity) |
| GET | `/incident/{id}` | Get incident details |
| POST | `/incident` | Create a new incident |
| PUT | `/incident/{id}` | Update incident fields (status, severity, assignee, root cause) |
| POST | `/incident/{id}/resolve` | Resolve an incident with resolution + root cause |
| GET | `/incident/{id}/timeline` | Get the incident's timeline events |
| POST | `/incident/{id}/timeline` | Append a timeline event |
| GET | `/incident/{id}/postmortem` | Get the postmortem for an incident |
| POST | `/incident/{id}/postmortem` | Create a postmortem (summary, impact, action items, lessons learned) |

**`IncidentSeverity`**: `Low`, `Medium`, `High`, `Critical`
**`IncidentStatus`**: `Open`, `Investigating`, `Identified`, `Monitoring`, `Resolved`, `Closed`

**Example — Create incident**

```http
POST /api/incident
Authorization: Bearer <token>
Content-Type: application/json

{
  "title": "Checkout API elevated 5xx rate",
  "description": "Error rate above 5% on /api/checkout since 14:02 UTC",
  "severity": "High",
  "affectedService": "checkout-service",
  "assignedTo": "jane.doe",
  "labels": ["payments", "production"]
}
```

Every create/update/resolve/postmortem action on this controller is recorded through `IAuditService`, so it automatically shows up in [Audit Logs](#9-audit-logs).

---

## 9. Audit Logs

`AuditController` — `/api/audit` (Admin, PlatformEngineer only)

| Method | Endpoint | Description |
|---|---|---|
| GET | `/audit` | List audit log entries — filter by `userId`, `entityType`, `action`, `from`, `to`; paginated |
| GET | `/audit/{entityType}/{entityId}` | Get the audit trail for one specific entity |
| GET | `/audit/stats` | Summary stats over a period: totals, success/failure split, breakdown by action/entity/user |

Every audit entry captures: `Action`, `EntityType`, `EntityId`, `UserId`, `UserName`, `IpAddress`, `OldValues`, `NewValues`, `IsSuccess`, `ErrorMessage`, `CreatedAt`.

**Example — Stats**

```http
GET /api/audit/stats?from=2026-06-01&to=2026-06-24
```

```json
{
  "totalActions": 412,
  "successCount": 405,
  "failureCount": 7,
  "byAction": [{ "action": "UPDATE", "count": 180 }, { "action": "CREATE", "count": 140 }],
  "byEntityType": [{ "entityType": "Incident", "count": 60 }],
  "byUser": [{ "user": "jane.doe", "count": 95 }],
  "period": { "from": "2026-06-01T00:00:00Z", "to": "2026-06-24T00:00:00Z" }
}
```

---

## 10. Cost Management

`CostController` — `/api/cost`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/cost/reports` | List cost reports — filter by team, environment, date range |
| GET | `/cost/reports/{id}` | Get a single cost report's breakdown |
| POST | `/cost/reports/generate` | Generate a new cost report for a period |
| GET | `/cost/teams/{teamId}/summary` | Aggregate spend summary for a team |
| GET | `/cost/budget-alerts` | List configured budget alert thresholds |
| POST | `/cost/budget-alerts` | Create a budget alert (team, threshold, period) |
| PUT | `/cost/budget-alerts/{id}` | Update a budget alert threshold |
| DELETE | `/cost/budget-alerts/{id}` | Remove a budget alert |

---

## 11. Error Format

All error responses follow a consistent shape:

```json
{
  "message": "Human-readable error description",
  "errors": { "fieldName": ["Validation message"] },
  "traceId": "00-abc123...-01"
}
```

| Status | Meaning |
|---|---|
| 400 | Validation failure / malformed request |
| 401 | Missing or invalid JWT |
| 403 | Authenticated but role lacks permission |
| 404 | Resource not found |
| 409 | Conflict (e.g. duplicate service name) |
| 500 | Unhandled server error (logged with `traceId`) |

## 12. Pagination Convention

List endpoints accept `page` (default `1`) and `pageSize` (default varies by endpoint, typically `20`–`50`) and respond with:

```json
{
  "items": [ /* ... */ ],
  "totalCount": 134,
  "page": 1,
  "pageSize": 20,
  "totalPages": 7
}
```

---

*Full interactive request/response schemas are available via Swagger UI at `/swagger` when running the API locally, generated directly from the controllers' XML doc comments and `Contracts` DTOs.*

# Enterprise IDP — Architecture Documentation

## Overview

Enterprise Internal Developer Platform (IDP) is a cloud-native platform engineering solution built on Kubernetes, following GitOps principles, Domain-Driven Design (DDD), and CQRS patterns.

---

## Architecture Diagram
┌─────────────────────────────────────────────────────────────────┐

│                        ENTERPRISE IDP                           │

├─────────────────────────────────────────────────────────────────┤

│                                                                 │

│   ┌─────────────┐    ┌─────────────┐    ┌─────────────┐       │

│   │   React +   │    │  ASP.NET    │    │  PostgreSQL  │       │

│   │ TypeScript  │───▶│  Core 9     │───▶│  Database   │       │

│   │  Frontend   │    │   Backend   │    │             │       │

│   └─────────────┘    └──────┬──────┘    └─────────────┘       │

│                             │                                   │

│                    ┌────────▼────────┐                         │

│                    │   Redis Cache   │                         │

│                    └─────────────────┘                         │

│                                                                 │

├─────────────────────────────────────────────────────────────────┤

│                     INFRASTRUCTURE                              │

│                                                                 │

│   ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐    │

│   │  ArgoCD  │  │  Vault   │  │Prometheus│  │  Grafana │    │

│   │  GitOps  │  │ Secrets  │  │ Metrics  │  │Dashboard │    │

│   └──────────┘  └──────────┘  └──────────┘  └──────────┘    │

│                                                                 │

│   ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐    │

│   │   Loki   │  │  Jaeger  │  │  OTel    │  │  Alert   │    │

│   │ Logging  │  │ Tracing  │  │Collector │  │ Manager  │    │

│   └──────────┘  └──────────┘  └──────────┘  └──────────┘    │

│                                                                 │

├─────────────────────────────────────────────────────────────────┤

│                      KUBERNETES                                 │

│                                                                 │

│   ┌──────────────────────────────────────────────────────┐    │

│   │  enterprise-idp-dev | staging | production           │    │

│   │  ┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐       │    │

│   │  │Backend │ │Frontend│ │  DB    │ │ Redis  │       │    │

│   │  │  HPA   │ │  HPA   │ │StateSt │ │  Dep   │       │    │

│   │  └────────┘ └────────┘ └────────┘ └────────┘       │    │

│   └──────────────────────────────────────────────────────┘    │

│                                                                 │

│   NGINX Ingress ──▶ Cert-Manager ──▶ TLS Termination          │

└─────────────────────────────────────────────────────────────────┘

---

## Technology Stack

### Frontend
| Technology | Version | Purpose |
|-----------|---------|---------|
| React | 18.x | UI Framework |
| TypeScript | 5.x | Type Safety |
| Vite | 5.x | Build Tool |
| TailwindCSS | 3.x | Styling |
| React Query | 5.x | Data Fetching |
| Zustand | 4.x | State Management |

### Backend
| Technology | Version | Purpose |
|-----------|---------|---------|
| ASP.NET Core | 9.0 | Web API |
| Entity Framework Core | 9.0 | ORM |
| MediatR | 12.x | CQRS/Mediator |
| FluentValidation | 11.x | Validation |
| Serilog | 3.x | Logging |
| OpenTelemetry | 1.7.x | Observability |

### Infrastructure
| Technology | Version | Purpose |
|-----------|---------|---------|
| Kubernetes | 1.28+ | Container Orchestration |
| Helm | 3.13+ | Package Manager |
| ArgoCD | 2.9+ | GitOps |
| Terraform | 1.6+ | Infrastructure as Code |
| Docker | 24+ | Containerization |

### Observability
| Technology | Purpose |
|-----------|---------|
| Prometheus | Metrics Collection |
| Grafana | Dashboards & Visualization |
| Loki | Log Aggregation |
| Promtail | Log Shipping |
| Jaeger | Distributed Tracing |
| OpenTelemetry | Instrumentation |
| Alertmanager | Alert Routing |

---

## Domain-Driven Design

### Bounded Contexts
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐

│   Identity &    │  │    Service      │  │    GitOps &     │

│  Authorization  │  │    Catalog      │  │    CI/CD        │

│                 │  │                 │  │                 │

│ - Users         │  │ - Services      │  │ - Repositories  │

│ - Teams         │  │ - Ownership     │  │ - Pipelines     │

│ - Roles         │  │ - Dependencies  │  │ - Deployments   │

│ - Permissions   │  │ - Metadata      │  │ - Releases      │

└─────────────────┘  └─────────────────┘  └─────────────────┘
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐

│  Kubernetes &   │  │  Observability  │  │   Security &    │

│ Infrastructure  │  │                 │  │   Compliance    │

│                 │  │ - Metrics       │  │                 │

│ - Namespaces    │  │ - Logs          │  │ - Secrets       │

│ - Deployments   │  │ - Traces        │  │ - Audit Logs    │

│ - HPA/VPA       │  │ - Alerts        │  │ - Compliance    │

│ - Ingress       │  │ - Incidents     │  │ - Scanning      │

└─────────────────┘  └─────────────────┘  └─────────────────┘

---

## CQRS Pattern
Command Side (Write):

Request ──▶ Controller ──▶ MediatR ──▶ CommandHandler ──▶ Domain ──▶ Repository ──▶ PostgreSQL
Query Side (Read):

Request ──▶ Controller ──▶ MediatR ──▶ QueryHandler ──▶ Redis Cache / PostgreSQL ──▶ DTO
---

## GitOps Flow
Developer Push

│

▼

GitHub Repository

│

▼

GitHub Actions CI (Build + Test + Scan + Push Image)

│

▼

Update Helm Values (image tag)

│

▼

ArgoCD detects change

│

▼

ArgoCD syncs to Kubernetes

│

▼

Rolling Update → Health Check → Done
---

## Security Architecture
External Traffic

│

▼

NGINX Ingress (TLS Termination)

│

▼

Network Policy (Pod-to-Pod rules)

│

▼

RBAC (Admin | Platform Engineer | Developer)

│

▼

ServiceAccount + Vault Agent Injector

│

▼

Application (Secrets from Vault)
---

## Multi-Environment Strategy

| Feature | Dev | Staging | Production |
|---------|-----|---------|------------|
| Replicas | 1 | 2 | 3 |
| Resources | Low | Medium | High |
| TLS | No | Yes | Yes |
| Vault | Dev Mode | HA | HA |
| ArgoCD | Optional | Yes | Yes |
| Monitoring | Basic | Full | Full |
| Backup | No | Daily | Hourly |

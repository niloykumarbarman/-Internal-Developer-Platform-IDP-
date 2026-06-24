using Prometheus;

namespace EnterpriseIDP.Infrastructure.Observability.Metrics;

public static class IDPMetrics
{
    public static readonly Counter HttpRequestsTotal = Prometheus.Metrics
        .CreateCounter("idp_http_requests_total", "Total HTTP requests",
            new CounterConfiguration { LabelNames = new[] { "method", "endpoint", "status_code" } });

    public static readonly Histogram HttpRequestDuration = Prometheus.Metrics
        .CreateHistogram("idp_http_request_duration_seconds", "HTTP request duration",
            new HistogramConfiguration
            {
                LabelNames = new[] { "method", "endpoint" },
                Buckets = new[] { 0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1.0, 2.5, 5.0, 10.0 }
            });

    public static readonly Gauge RegisteredServicesTotal = Prometheus.Metrics
        .CreateGauge("idp_registered_services_total", "Total registered services in catalog");

    public static readonly Counter ServiceRegistrations = Prometheus.Metrics
        .CreateCounter("idp_service_registrations_total", "Service registration events",
            new CounterConfiguration { LabelNames = new[] { "team", "service_type" } });

    public static readonly Counter DeploymentsTotal = Prometheus.Metrics
        .CreateCounter("idp_deployments_total", "Total deployments triggered",
            new CounterConfiguration { LabelNames = new[] { "environment", "status", "team" } });

    public static readonly Gauge ActiveDeployments = Prometheus.Metrics
        .CreateGauge("idp_active_deployments", "Currently running deployments",
            new GaugeConfiguration { LabelNames = new[] { "environment" } });

    public static readonly Histogram DeploymentDuration = Prometheus.Metrics
        .CreateHistogram("idp_deployment_duration_seconds", "Deployment completion time",
            new HistogramConfiguration
            {
                LabelNames = new[] { "environment", "service" },
                Buckets = new[] { 10.0, 30, 60, 120, 300, 600, 1200 }
            });

    public static readonly Counter GitHubApiCallsTotal = Prometheus.Metrics
        .CreateCounter("idp_github_api_calls_total", "Total GitHub API calls",
            new CounterConfiguration { LabelNames = new[] { "operation", "status" } });

    public static readonly Counter RepositoriesCreated = Prometheus.Metrics
        .CreateCounter("idp_repositories_created_total", "GitHub repositories created via IDP");

    public static readonly Counter LoginAttempts = Prometheus.Metrics
        .CreateCounter("idp_login_attempts_total", "Total login attempts",
            new CounterConfiguration { LabelNames = new[] { "provider", "status" } });

    public static readonly Gauge ActiveUsers = Prometheus.Metrics
        .CreateGauge("idp_active_users_total", "Active users with JWT sessions");

    public static readonly Histogram DbQueryDuration = Prometheus.Metrics
        .CreateHistogram("idp_db_query_duration_seconds", "Database query execution time",
            new HistogramConfiguration
            {
                LabelNames = new[] { "operation", "table" },
                Buckets = new[] { 0.001, 0.005, 0.01, 0.05, 0.1, 0.5, 1.0, 5.0 }
            });

    public static readonly Counter DbErrors = Prometheus.Metrics
        .CreateCounter("idp_db_errors_total", "Total database errors",
            new CounterConfiguration { LabelNames = new[] { "operation" } });

    public static readonly Counter NamespacesProvisioned = Prometheus.Metrics
        .CreateCounter("idp_k8s_namespaces_provisioned_total", "Kubernetes namespaces provisioned",
            new CounterConfiguration { LabelNames = new[] { "environment", "team" } });

    public static readonly Gauge ManagedNamespaces = Prometheus.Metrics
        .CreateGauge("idp_k8s_managed_namespaces", "Total namespaces managed by IDP");
}

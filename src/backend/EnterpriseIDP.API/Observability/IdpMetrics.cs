using Prometheus;

namespace EnterpriseIDP.API.Observability;

public static class IdpMetrics
{
    private static readonly Counter DeploymentsTotal = Metrics
        .CreateCounter(
            "idp_deployments_total",
            "Total number of deployments triggered",
            new CounterConfiguration { LabelNames = ["environment", "status", "team"] });

    private static readonly Counter ServicesRegistered = Metrics
        .CreateCounter(
            "idp_services_registered_total",
            "Total services registered in catalog",
            new CounterConfiguration { LabelNames = ["type", "team"] });

    private static readonly Gauge ActiveDeployments = Metrics
        .CreateGauge(
            "idp_active_deployments",
            "Currently active deployments",
            new GaugeConfiguration { LabelNames = ["environment"] });

    private static readonly Histogram DeploymentDuration = Metrics
        .CreateHistogram(
            "idp_deployment_duration_seconds",
            "Deployment duration in seconds",
            new HistogramConfiguration
            {
                LabelNames = ["environment", "team"],
                Buckets = [30, 60, 120, 300, 600, 900, 1800]
            });

    private static readonly Counter PipelineRuns = Metrics
        .CreateCounter(
            "idp_pipeline_runs_total",
            "Total CI/CD pipeline runs",
            new CounterConfiguration { LabelNames = ["status", "team"] });

    private static readonly Gauge KubernetesNamespaces = Metrics
        .CreateGauge("idp_kubernetes_namespaces_total",
            "Total Kubernetes namespaces provisioned");

    public static void RecordDeployment(string environment, string status, string team)
        => DeploymentsTotal.WithLabels(environment, status, team).Inc();

    public static void RecordServiceRegistration(string type, string team)
        => ServicesRegistered.WithLabels(type, team).Inc();

    public static void SetActiveDeployments(string environment, double count)
        => ActiveDeployments.WithLabels(environment).Set(count);

    public static IDisposable TrackDeploymentDuration(string environment, string team)
        => DeploymentDuration.WithLabels(environment, team).NewTimer();

    public static void RecordPipelineRun(string status, string team)
        => PipelineRuns.WithLabels(status, team).Inc();

    public static void SetKubernetesNamespaces(double count)
        => KubernetesNamespaces.Set(count);
}

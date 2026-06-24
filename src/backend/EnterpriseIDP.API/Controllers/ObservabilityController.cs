using EnterpriseIDP.API.Observability;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EnterpriseIDP.API.Controllers;

[ApiController]
[Route("api/observability")]
public class ObservabilityController : ControllerBase
{
    private readonly HealthCheckService _healthCheck;
    private readonly ILogger<ObservabilityController> _logger;

    public ObservabilityController(
        HealthCheckService healthCheck,
        ILogger<ObservabilityController> logger)
    {
        _healthCheck = healthCheck;
        _logger = logger;
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHealth()
    {
        var report = await _healthCheck.CheckHealthAsync();
        var result = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds,
                tags = e.Value.Tags
            })
        };
        return report.Status == HealthStatus.Healthy ? Ok(result) : StatusCode(503, result);
    }

    [HttpPost("metrics/deployment")]
    [Authorize(Roles = "Admin,PlatformEngineer")]
    public IActionResult RecordDeploymentMetric(
        [FromQuery] string environment = "staging",
        [FromQuery] string status = "success",
        [FromQuery] string team = "platform")
    {
        IdpMetrics.RecordDeployment(environment, status, team);
        _logger.LogInformation(
            "Deployment metric recorded: {Environment} {Status} {Team}",
            environment, status, team);
        return Ok(new { message = "Metric recorded", environment, status, team });
    }
}

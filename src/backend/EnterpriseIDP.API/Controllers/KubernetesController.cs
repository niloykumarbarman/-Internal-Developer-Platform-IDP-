using EnterpriseIDP.Application.Features.Kubernetes.Commands.CreateNamespace;
using EnterpriseIDP.Application.Features.Kubernetes.Queries.GetDeployments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseIDP.API.Controllers;

[ApiController]
[Route("api/kubernetes")]
[Authorize]
public class KubernetesController : ControllerBase
{
    private readonly IMediator _mediator;

    public KubernetesController(IMediator mediator) => _mediator = mediator;

    [HttpGet("deployments")]
    public async Task<IActionResult> GetDeployments([FromQuery] GetDeploymentsQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpPost("namespaces")]
    public async Task<IActionResult> CreateNamespace(CreateNamespaceCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}

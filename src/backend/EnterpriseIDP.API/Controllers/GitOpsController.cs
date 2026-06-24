using EnterpriseIDP.Application.Features.GitOps.Commands.SyncApplication;
using EnterpriseIDP.Application.Features.GitOps.Commands.CreateRepository;
using EnterpriseIDP.Application.Features.GitOps.Queries.GetApplicationStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseIDP.API.Controllers;

[ApiController]
[Route("api/gitops")]
[Authorize]
public class GitOpsController : ControllerBase
{
    private readonly IMediator _mediator;

    public GitOpsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Sync an ArgoCD application
    /// </summary>
    [HttpPost("applications/{name}/sync")]
    [Authorize(Roles = "Admin,PlatformEngineer")]
    public async Task<IActionResult> SyncApplication(
        string name,
        [FromQuery] string @namespace = "enterprise-idp",
        [FromQuery] bool dryRun = false)
    {
        var result = await _mediator.Send(
            new SyncApplicationCommand(name, @namespace, dryRun));
        
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get ArgoCD application sync status
    /// </summary>
    [HttpGet("applications/{name}/status")]
    public async Task<IActionResult> GetApplicationStatus(string name)
    {
        var result = await _mediator.Send(new GetApplicationStatusQuery(name));
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Create a new GitHub repository
    /// </summary>
    [HttpPost("repositories")]
    [Authorize(Roles = "Admin,PlatformEngineer,Developer")]
    public async Task<IActionResult> CreateRepository(
        [FromBody] CreateRepositoryCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

using EnterpriseIDP.Application.Features.CICD.Commands.TriggerPipeline;
using EnterpriseIDP.Application.Features.CICD.Commands.CreatePipeline;
using EnterpriseIDP.Application.Features.CICD.Queries.GetPipelines;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseIDP.API.Controllers;

[ApiController]
[Route("api/pipelines")]
[Authorize]
public class CICDController : ControllerBase
{
    private readonly IMediator _mediator;

    public CICDController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetPipelines([FromQuery] GetPipelinesQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpPost("trigger")]
    public async Task<IActionResult> TriggerPipeline(TriggerPipelineCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePipeline(CreatePipelineCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}

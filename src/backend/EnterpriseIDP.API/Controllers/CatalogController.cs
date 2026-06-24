using EnterpriseIDP.Application.Features.Catalog.Commands.RegisterService;
using EnterpriseIDP.Application.Features.Catalog.Queries.GetServiceById;
using EnterpriseIDP.Application.Features.Catalog.Queries.GetServices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseIDP.API.Controllers;

[ApiController]
[Route("api/services")]
[Authorize]
public class CatalogController : ControllerBase
{
    private readonly IMediator _mediator;

    public CatalogController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetServices([FromQuery] GetServicesQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetServiceById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetServiceByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> RegisterService(RegisterServiceCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Promotions.Application.Dashboard.Queries;

namespace Promotions.Api.Controllers;

[ApiController]
[Route("api/promotions/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics()
    {
        var result = await _mediator.Send(new GetDashboardMetricsQuery());
        return Ok(result);
    }
}

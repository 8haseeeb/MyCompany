using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyCompany.ApiGateway.Controllers;

/// <summary>
/// Liveness for the gateway itself at GET /api/health (not proxied). Aggregate checks remain at GET /api/gateway/health.
/// </summary>
[ApiController]
[Route("api")]
public class GatewayPublicHealthController : ControllerBase
{
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult GetGatewayHealth()
    {
        return Ok(new
        {
            service = "ApiGateway",
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            note = "Downstream aggregate: GET /api/gateway/health"
        });
    }
}

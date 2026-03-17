using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Promotions.Api.Controllers
{
    [Route("api/v1/promotions")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize] 
    public class PromotionsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(new 
            { 
                message = "Promotions API working!",
                user = User.Identity?.Name,
                timestamp = DateTime.UtcNow
            });
        }
    }
}

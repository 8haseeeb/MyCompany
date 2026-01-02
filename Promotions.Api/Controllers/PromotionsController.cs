using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Promotions.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // JWT Token required
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

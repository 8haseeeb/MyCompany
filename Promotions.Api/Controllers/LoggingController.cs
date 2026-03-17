using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog.Core;
using Serilog.Events;

namespace Promotions.Api.Controllers
{
    [ApiController]
    [Route("api/v1/logging")]
    [ApiVersion("1.0")]
    [Authorize]
    public class LoggingController : ControllerBase
    {
        private readonly LoggingLevelSwitch _levelSwitch;

        public LoggingController(LoggingLevelSwitch levelSwitch)
        {
            _levelSwitch = levelSwitch;
        }

        /// <summary>
        /// Get current logging level
        /// </summary>
        [HttpGet("level")]
        public IActionResult GetCurrentLevel()
        {
            return Ok(new
            {
                currentLevel = _levelSwitch.MinimumLevel.ToString(),
                availableLevels = new[]
                {
                    "Verbose",
                    "Debug",
                    "Information",
                    "Warning",
                    "Error",
                    "Fatal"
                }
            });
        }

        /// <summary>
        /// Change logging level at runtime. Restricted to Admin role.
        /// </summary>
        /// <param name="level">New log level (Verbose, Debug, Information, Warning, Error, Fatal)</param>
        [HttpPost("level")]
        [Authorize(Roles = "Admin")]
        public IActionResult SetLevel([FromBody] SetLogLevelRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Level))
            {
                return BadRequest(new { message = "Level is required" });
            }

            if (!Enum.TryParse<LogEventLevel>(request.Level, true, out var logLevel))
            {
                return BadRequest(new
                {
                    message = $"Invalid log level: {request.Level}",
                    validLevels = new[] { "Verbose", "Debug", "Information", "Warning", "Error", "Fatal" }
                });
            }

            var previousLevel = _levelSwitch.MinimumLevel;
            _levelSwitch.MinimumLevel = logLevel;

            return Ok(new
            {
                message = "Logging level changed successfully",
                previousLevel = previousLevel.ToString(),
                newLevel = logLevel.ToString()
            });
        }
    }

    public class SetLogLevelRequest
    {
        public string Level { get; set; } = string.Empty;
    }
}

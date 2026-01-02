using MediatR;
using Microsoft.AspNetCore.Mvc;
using SSO.Application.Auth.Commands;
using SSO.Application.Dtos;

namespace SSO.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex) when (ex.Message == "Invalid credentials")
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var message = await _mediator.Send(new RegisterCommand(dto.UserName, dto.Email, dto.Password));

        return Ok(new
        {
            message = message
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto dto)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(dto.RefreshToken));
        return Ok(result);
    }


}

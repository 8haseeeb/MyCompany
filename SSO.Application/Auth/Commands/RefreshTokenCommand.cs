using MediatR;
using SSO.Application.Dtos;

namespace SSO.Application.Auth.Commands;

public class RefreshTokenCommand : IRequest<RefreshTokenResponseDto>
{
    public string RefreshToken { get; }

    public RefreshTokenCommand(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}

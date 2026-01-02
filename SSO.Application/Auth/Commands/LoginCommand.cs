using MediatR;
using SSO.Application.Dtos;

namespace SSO.Application.Auth.Commands;

public record LoginCommand(string Email, string Password)
    : IRequest<LoginResultDto>;

using MediatR;
using SSO.Application.Dtos;

namespace SSO.Application.Auth.Commands;

public class RegisterCommand : IRequest<RegisterResultDto>
{
    public string UserName { get; }
    public string Email { get; }
    public string Password { get; }

    public RegisterCommand(string userName, string email, string password)
    {
        UserName = userName;
        Email = email;
        Password = password;
    }
}

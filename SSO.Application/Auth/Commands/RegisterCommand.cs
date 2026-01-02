using MediatR;

namespace SSO.Application.Auth.Commands;

public class RegisterCommand : IRequest<string>
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

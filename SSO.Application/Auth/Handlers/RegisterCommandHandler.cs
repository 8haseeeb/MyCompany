using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SSO.Application.Auth.Commands;
using SSO.Application.Interfaces;
using SSO.Domain.Users;

namespace SSO.Application.Auth.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
{
    private readonly IIdentityDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IIdentityDbContext context,
        IPasswordHasher passwordHasher,
        ILogger<RegisterCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[Register] Attempting registration. Email: {Email}, UserName: {UserName}",
            request.Email, request.UserName);

        var exists = await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (exists)
        {
            _logger.LogWarning("[Register] User already exists. Email: {Email}", request.Email);
            throw new Exception("User already exists");
        }

        var hash = _passwordHasher.Hash(request.Password);

        var user = new User(request.UserName, request.Email, hash, request.Role);
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("[Register] User registered successfully. Email: {Email}", request.Email);
        return "User registered successfully";
    }
}

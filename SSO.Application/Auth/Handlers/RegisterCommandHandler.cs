using MediatR;
using Microsoft.EntityFrameworkCore;
using SSO.Application.Auth.Commands;
using SSO.Application.Interfaces;
using SSO.Domain.Users;

namespace SSO.Application.Auth.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
{
    private readonly IIdentityDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(
        IIdentityDbContext context,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (exists)
            throw new Exception("User already exists");

        var hash = _passwordHasher.Hash(request.Password);

        var user = new User(request.UserName, request.Email, hash);
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return "User registered successfully";
    }
}

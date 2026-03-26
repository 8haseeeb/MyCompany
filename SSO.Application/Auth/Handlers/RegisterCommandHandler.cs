using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SSO.Application.Auth;
using SSO.Application.Auth.Commands;
using SSO.Application.Common;
using SSO.Application.Dtos;
using SSO.Application.Interfaces;
using SSO.Domain.Users;

namespace SSO.Application.Auth.Handlers;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResultDto>
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

    public async Task<RegisterResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = AuthEmailNormalizer.Normalize(request.Email);

        _logger.LogInformation("[Register] Attempting registration. Email: {Email}, UserName: {UserName}",
            normalizedEmail, request.UserName);

        var exists = await _context.Users.AnyAsync(
            u => u.Email.ToLower() == normalizedEmail,
            cancellationToken);
        if (exists)
        {
            _logger.LogWarning("[Register] User already exists. Email: {Email}", normalizedEmail);
            throw new DuplicateUserException();
        }

        var hash = _passwordHasher.Hash(request.Password);
        // Public signup is always "User". Role elevation is not accepted via registration.
        const string role = "User";

        var user = new User(request.UserName.Trim(), normalizedEmail, hash, role);
        _context.Users.Add(user);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
        {
            _logger.LogWarning(ex, "[Register] Unique constraint violation. Email: {Email}", normalizedEmail);
            throw new DuplicateUserException();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "[Register] Database error on insert. Email: {Email}", normalizedEmail);
            throw;
        }

        _logger.LogInformation("[Register] User registered successfully. Email: {Email}", normalizedEmail);
        return new RegisterResultDto
        {
            Message = "Account created. Please log in with your email and password.",
            UserName = user.UserName,
            Email = user.Email
        };
    }
}

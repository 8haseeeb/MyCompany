using MediatR;
using SSO.Application.Auth.Commands;
using SSO.Application.Interfaces;

namespace SSO.Application.Auth.Handlers;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IIdentityDbContext _context;

    public LogoutCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user == null)
            return Unit.Value; // Idempotent: already logged out or invalid user

        user.UpdateSession(null);
        user.UpdateRefreshToken(null, null);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

using Microsoft.EntityFrameworkCore;
using SSO.Application.Auth;
using SSO.Application.Interfaces;
using SSO.Domain.Users;
using SSO.Infrastructure.Persistence;

namespace SSO.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityDbContext _context;

        public UserRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var normalized = AuthEmailNormalizer.Normalize(email);
            if (string.IsNullOrEmpty(normalized))
                return Task.FromResult<User?>(null);

            return _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalized, cancellationToken);
        }
    }
}

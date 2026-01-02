using Microsoft.EntityFrameworkCore;
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
            return _context.Users
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}

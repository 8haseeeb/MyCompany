using Microsoft.EntityFrameworkCore;
using SSO.Domain.RefreshTokens;
using SSO.Domain.Users;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Application.Interfaces
{
    public interface IIdentityDbContext
    {
        DbSet<User> Users { get; }

        DbSet<RefreshToken> RefreshTokens { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

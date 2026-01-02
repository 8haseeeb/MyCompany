using Microsoft.EntityFrameworkCore;
using SSO.Application.Interfaces;
using SSO.Domain.Users;
using SSO.Domain.RefreshTokens;
using SSO.Infrastructure.Persistence.Configurations;
using System.Threading;
using System.Threading.Tasks;

namespace SSO.Infrastructure.Persistence
{
    public class IdentityDbContext : DbContext, IIdentityDbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from Configurations folder
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        }
    }
}

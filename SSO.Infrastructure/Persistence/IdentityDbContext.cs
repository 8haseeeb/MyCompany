using SSO.Domain.Users;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SSO.Infrastructure.Persistence.Configurations; // Keep this as UserConfiguration is used

namespace SSO.Infrastructure.Persistence
{
    public class IdentityDbContext : DbContext, IIdentityDbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        // Removed: // public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from Configurations folder
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            // modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

        }
    }
}

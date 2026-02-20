using Microsoft.EntityFrameworkCore;
using SSO.Domain.Users;

namespace Promotions.Infrastructure.Persistence.External
{
    public class SsoDbContext : DbContext
    {
        public SsoDbContext(DbContextOptions<SsoDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Minimal configuration to map to the existing SSO database
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserName).HasColumnName("Name");
                entity.Ignore(e => e.CurrentSessionId);
                entity.Ignore(e => e.RefreshToken);
                entity.Ignore(e => e.RefreshTokenExpiry);
                entity.Ignore(e => e.Role);
            });
        }
    }
}

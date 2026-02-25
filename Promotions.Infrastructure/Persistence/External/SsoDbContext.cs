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
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);

                // Map the column name differences
                entity.Property(e => e.UserName).HasColumnName("Name");
                entity.Property(e => e.Email).HasColumnName("Email");
                entity.Property(e => e.PasswordHash).HasColumnName("PasswordHash");
                entity.Property(e => e.CurrentSessionId).HasColumnName("CurrentSessionId").HasMaxLength(100);
                entity.Property(e => e.RefreshToken).HasColumnName("RefreshToken");
                entity.Property(e => e.RefreshTokenExpiry).HasColumnName("RefreshTokenExpiry");

                // Ignore non-DB properties
                entity.Ignore(e => e.Role);
            });
        }
    }
}

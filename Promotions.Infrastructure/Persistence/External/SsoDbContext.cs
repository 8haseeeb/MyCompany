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
                entity.ToTable("Users"); // Ensure it matches SSO table name
                entity.HasKey(e => e.Id);
            });
        }
    }
}

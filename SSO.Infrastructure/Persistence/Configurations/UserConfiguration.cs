using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSO.Domain.Users;

namespace SSO.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).ValueGeneratedOnAdd();
            builder.Property(u => u.Email).IsRequired();
            builder.Property(u => u.UserName).HasColumnName("Name").IsRequired();
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.RefreshToken).HasColumnName("RefreshToken");
            builder.Property(u => u.RefreshTokenExpiry).HasColumnName("RefreshTokenExpiry");

            
            // Ignore columns and relationships that don't exist in the migrated database
            builder.Ignore(u => u.Role);
            builder.Ignore(u => u.CurrentSessionId);
            


        }
    }
}

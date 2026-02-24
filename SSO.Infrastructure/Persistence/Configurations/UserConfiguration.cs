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

            
            // Relationships and extra properties
            builder.Property(u => u.Role).HasColumnName("Role").IsRequired();
            builder.Property(u => u.CurrentSessionId).HasColumnName("CurrentSessionId").HasMaxLength(100);
            


        }
    }
}

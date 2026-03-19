using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SSO.Infrastructure.Persistence
{
    /// <summary>
    /// Factory used by EF Core CLI tools (dotnet ef migrations add / update) at design-time.
    /// Not used at runtime — the app registers IdentityDbContext via DI in Program.cs.
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=SSOIdentityDb;Trusted_Connection=True;TrustServerCertificate=True;");

            return new IdentityDbContext(optionsBuilder.Options);
        }
    }
}

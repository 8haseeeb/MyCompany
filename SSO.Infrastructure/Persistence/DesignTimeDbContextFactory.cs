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
            // Docker SQL on host port 1433 (dotnet ef from Windows/Mac against compose-mapped SQL)
            optionsBuilder.UseSqlServer(
                "Server=localhost,1433;Database=SSOServiceDb;User Id=sa;Password=MyPass@123;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true;");

            return new IdentityDbContext(optionsBuilder.Options);
        }
    }
}

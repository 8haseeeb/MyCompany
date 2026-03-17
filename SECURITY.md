# Security and Configuration

## Secrets Management

**Never commit real secrets to the repository.** The following must be set via environment variables, User Secrets (local development), or Azure Key Vault (production):

### Required secrets

| Key | Description | Local dev | Production |
|-----|-------------|-----------|------------|
| `JwtSettings:Secret` | JWT signing key (min 32 chars) | User Secrets or env | Env var or Key Vault |
| `ApplicationInsights:ConnectionString` | Azure Application Insights | User Secrets or env | Env var or Key Vault |
| `ConnectionStrings:DefaultConnection` | SQL Server (Promotions/SSO) | appsettings or User Secrets | Env var |
| `ConnectionStrings:SsoConnection` | SSO DB (Promotions.Api only) | appsettings or User Secrets | Env var |

### Local development

```bash
# From solution directory (MyCompany)
cd MyCompany.ApiGateway
dotnet user-secrets set "JwtSettings:Secret" "YourDevSecretKeyAtLeast32CharactersLong"

cd ../SSO.Api
dotnet user-secrets set "JwtSettings:Secret" "YourDevSecretKeyAtLeast32CharactersLong"
dotnet user-secrets set "ApplicationInsights:ConnectionString" "<your-appinsights-connection-string>"

cd ../Promotions.Api
dotnet user-secrets set "JwtSettings:Secret" "YourDevSecretKeyAtLeast32CharactersLong"
dotnet user-secrets set "ApplicationInsights:ConnectionString" "<your-appinsights-connection-string>"
```

Use the **same** JWT secret across Gateway, SSO.Api, and Promotions.Api so tokens issued by SSO are accepted by the Gateway and Promotions.

### Production (Azure Container Apps / GitHub Actions)

The deploy workflow (`.github/workflows/deploy.yml`) passes the following from **GitHub Secrets** into each Container App:

| GitHub Secret | Used as env var(s) | Services |
|---------------|--------------------|----------|
| `JWT_SECRET` | `JwtSettings__Secret` | SSO, Promotions, Gateway |
| `APPINSIGHTS_CONNECTION_STRING` | `ApplicationInsights__ConnectionString` | SSO, Promotions, Gateway |
| `SQL_CONNECTION_STRING_SSO` | `ConnectionStrings__DefaultConnection` | SSO |
| `SQL_CONNECTION_STRING_PROMOTIONS` | `ConnectionStrings__DefaultConnection`, `ConnectionStrings__SsoConnection` | Promotions |

Add `JWT_SECRET` and `APPINSIGHTS_CONNECTION_STRING` in the repo **Settings → Secrets and variables → Actions** so deployments succeed.

# Configuration & documentation changes

This document records **what changed** in the repo for local development, API Gateway routing, Azure-independent defaults, and database setup.

---

## 1. Local development & API Gateway (no APIM)

| Area | Change |
|------|--------|
| **Gateway downstream URLs** | `MyCompany.ApiGateway/appsettings.json` adds `Downstream:SsoUrl` and `Downstream:PromotionsUrl` (default `http://localhost:5253` and `http://localhost:5137`). `RouteResolver.Configure(IConfiguration)` runs at startup; env vars `SSO_URL` / `PROMOTIONS_URL` override config. |
| **RouteResolver** | `RewriteToVersionedPath` leaves `/api/health` unchanged (gateway-owned). Public properties `SsoBaseUrl` / `PromotionsBaseUrl` for health checks. |
| **Gateway health** | New `GatewayPublicHealthController`: `GET /api/health` returns gateway liveness. `GatewayHealthController` calls downstream **`/api/v1/health`** (not legacy `/api/health`). |
| **Application Insights** | Gateway, SSO, Promotions: `AddApplicationInsightsTelemetry` only when connection string is set and not `REPLACE_*`. OpenTelemetry uses the same guard. |
| **Serilog** | `BuildingBlocks/Common.Logging/Serilog/SerilogExtensions.cs`: Application Insights sink skipped for empty or `REPLACE_*` connection strings. |
| **Gateway appsettings** | `ApplicationInsights:ConnectionString` set to `""`; Serilog `WriteTo` no longer includes Application Insights (console + file). `Cors:AllowedOrigins` includes `http://localhost:5001` (Vite). |
| **Development JWT** | `MyCompany.ApiGateway/appsettings.Development.json`, `SSO.Api/appsettings.Development.json`, `Promotions.Api/appsettings.Development.json`: shared `JwtSettings:Secret` for local only. |
| **Redis (local)** | `SSO.Api` and `Promotions.Api` `appsettings.json`: `ConnectionStrings:Redis` is `""` → `DistributedMemoryCache` when Redis is not used. |
| **Frontend** | `MyCompany.WebApp/apps/host/src/services/api.js`: removed APIM path prefixes (`/sso`, `/promotion`, `VITE_APIM_PATH_PREFIX`). All calls use `/api/...` only. |
| **Vite proxy** | `apps/host/vite.config.js`: proxy target `http://localhost:5089` (Gateway HTTP port from `launchSettings`). |
| **Unit tests** | `RouteResolverTests`: added case for `/api/health` rewrite (unchanged). |

**Reference:** `docs/LOCAL_RUN_GATEWAY.md`

---

## 2. Database names & connection strings

| Change | Files |
|--------|--------|
| SSO database renamed from `SSOIdentityDb` to **`SSOServiceDb`** | `SSO.Api/appsettings.json` → `DefaultConnection` |
| Promotions **`SsoConnection`** points to **`SSOServiceDb`** (same as SSO; required for session validation) | `Promotions.Api/appsettings.json` |
| **`TrustServerCertificate=True`** added on SSO/Promotions connection strings | `SSO.Api/appsettings.json`, `Promotions.Api/appsettings.json` |
| **`PromotionsDb`** remains the promotions application database | `Promotions.Api/appsettings.json` → `DefaultConnection` |

**Reference:** `docs/DATABASE_SSO_PROMOTIONS_SETUP.md`

---

## 3. New files added

| File | Purpose |
|------|---------|
| `scripts/sql/01-list-tables-by-database.sql` | Lists all tables in `SSOServiceDb` and `PromotionsDb` for verification. |
| `docs/DATABASE_SSO_PROMOTIONS_SETUP.md` | Step-by-step: connect, list tables, apply EF migrations, query, troubleshooting. |
| `docs/LOCAL_RUN_GATEWAY.md` | Ports, start order, test URLs, frontend notes for local Gateway entry. |
| `MyCompany.ApiGateway/Controllers/GatewayPublicHealthController.cs` | `GET /api/health` on the gateway. |
| `docs/CHANGELOG_CONFIGURATION_AND_DOCS.md` | This file — summary of configuration and documentation changes. |

---

## 4. Previously documented work (cross-reference)

| Document | Contents |
|----------|----------|
| `docs/DEVELOPMENT-CHANGES-ANALYSIS.md` | Phases A/B/C, OpenTelemetry, logout + Redis, API versioning, deploy, CI. |
| `ENTERPRISE_IMPROVEMENTS.md` | Enterprise tracing, session cache invalidation, `/api/v1` routing. |
| `docs/AZURE_CONTAINER_APPS_DEPLOYMENT.md` | GitHub Secrets, ACA deploy, Redis, troubleshooting (e.g. 405 login). |

---

## 5. Quick checklist for a new machine

1. Set SQL connection strings if not using LocalDB / default DB names.  
2. Run `scripts/sql/01-list-tables-by-database.sql` or apply `dotnet ef database update` per `DATABASE_SSO_PROMOTIONS_SETUP.md`.  
3. Run Gateway → SSO → Promotions → Vite per `LOCAL_RUN_GATEWAY.md`.  
4. Optional: set `ConnectionStrings:Redis` on SSO and Promotions if you need shared session cache / logout invalidation across processes.

---

*Last updated: reflects SSOServiceDb/PromotionsDb naming, local Gateway defaults, and SQL verification script.*

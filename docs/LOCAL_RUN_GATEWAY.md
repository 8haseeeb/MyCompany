# Local run (API Gateway entry, no Azure / no APIM)

## Ports (from `Properties/launchSettings.json`)

| Service | HTTP | HTTPS (optional) |
|---------|------|------------------|
| **MyCompany.ApiGateway** | **5089** | 7211 |
| **SSO.Api** | **5253** | 7222 |
| **Promotions.Api** | **5137** | 7043 |
| **Host Vite** | **5001** | — |

Downstream URLs used by the gateway default to **HTTP** (`http://localhost:5253`, `http://localhost:5137`) so TLS certificate issues do not block local calls.

## Start order

1. **SQL Server** (LocalDB or your instance) — databases `SSOIdentityDb` and `PromotionsDb` must exist; apps apply EF migrations on startup.
2. **SSO.Api** — `dotnet run --project SSO.Api` (profile `http` or `https`).
3. **Promotions.Api** — `dotnet run --project Promotions.Api`.
4. **MyCompany.ApiGateway** — `dotnet run --project MyCompany.ApiGateway`.
5. **WebApp (optional)** — from `MyCompany.WebApp`: `npm run dev --workspace=apps/host` (proxy sends `/api` → `http://localhost:5089`).

## Configuration you already have

- **Gateway** `appsettings.json` → `Downstream:SsoUrl`, `Downstream:PromotionsUrl` (overridable with env `SSO_URL` / `PROMOTIONS_URL`).
- **Development JWT** — same `JwtSettings:Secret` in `appsettings.Development.json` for Gateway, SSO, and Promotions (ASP.NET Core `Development` only).
- **Redis** — `ConnectionStrings:Redis` empty → `IDistributedMemoryCache` (no Redis required).
- **Application Insights** — empty connection string → no AI registration, no Serilog AI sink (see `SerilogExtensions` + `Program.cs` guards).

## Quick tests

| Test | URL |
|------|-----|
| Gateway liveness | `GET http://localhost:5089/api/health` |
| Aggregate health | `GET http://localhost:5089/api/gateway/health` |
| Login (via gateway) | `POST http://localhost:5089/api/auth/login` → rewritten to `/api/v1/auth/login` on SSO |
| Promotions (JWT) | `GET http://localhost:5089/api/promotions/actions` with `Authorization: Bearer …` |
| Logout | `POST http://localhost:5089/api/auth/logout` with Bearer token |

## Frontend

- Use **only** paths like `/api/auth/login`, `/api/promotions/...` (no `/sso` or `/promotion` prefixes).
- Dev: leave `VITE_API_BASE_URL` empty; Vite proxies `/api` to the gateway (`vite.config.js` → `http://localhost:5089`).

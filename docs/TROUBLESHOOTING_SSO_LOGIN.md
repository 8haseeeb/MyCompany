# Troubleshooting SSO login (`POST /api/v1/auth/login`)

## Typical causes of HTTP 500 (fixed in code)

| Cause | What happened | Fix |
|--------|----------------|-----|
| **JWT secret missing/placeholder** | After password succeeds, `JwtTokenService` called `SymmetricSecurityKey` with null/short `JwtSettings:Secret` → exception → 500. | Set `JwtSettings:Secret` (≥ 32 UTF-8 bytes). In Development use `appsettings.Development.json`. In Production use `JwtSettings__Secret` env var. |
| **NULL `Role` in `Users`** | `new Claim(ClaimTypes.Role, user.Role)` threw if `Role` was null. | Code now uses `user.Role` fallback to `"User"`. Still recommended: keep `Role` non-null in SQL. |
| **Malformed password hash** | `BCrypt.Verify` threw on non-bcrypt strings. | `PasswordHasher.Verify` now catches and returns `false` → 401 instead of 500. |
| **Wrong credentials** | Previously thrown generic `Exception("Invalid credentials")`; controller only caught exact message. | Use `InvalidCredentialsException` → consistent **401** with JSON body. |

## Verify configuration

1. **Connection string** — `SSO.Api` `ConnectionStrings:DefaultConnection` → database `SSOServiceDb`, server correct.
2. **Migrations** — `dotnet ef database update` (see `DATABASE_SSO_PROMOTIONS_SETUP.md`) so `Users` matches EF (`Name`, `Email`, `PasswordHash`, `Role`, etc.).
3. **JWT secret** — Not empty, not `REPLACE_*`, length ≥ 32 characters for HS256.

## Verify bcrypt hash vs password

In a **test project** or temporary console (do not log real passwords in production):

```csharp
var ok = BCrypt.Net.BCrypt.Verify("haseeb123", "$2a$11$7Lw4yoUV9I4Y2RWJ3VahJuFX6vBjhT7a/jTmmCRxJZmEM7k5QSjAm");
// ok must be true for login to succeed after our fixes
```

If `Verify` returns `false`, the hash in the row does not match that plaintext (wrong password, wrong hash row, or truncated hash in DB).

## Expected HTTP results (after fixes)

| Situation | Status | Body |
|-----------|--------|------|
| Bad email/password | **401** | `{ "message": "Invalid email or password" }` |
| JWT not configured | **503** | `{ "message": "JwtSettings:Secret is missing..." }` |
| Success | **200** | `{ "accessToken", "refreshToken", "userName" }` |

## SQL quick checks

```sql
USE SSOServiceDb;
SELECT Id, Name, Email, LEFT(PasswordHash, 7) AS HashPrefix, Role
FROM dbo.Users
WHERE Email = 'haseeb@gmail.com';
-- HashPrefix should be '$2a$11$' or '$2b$' etc.
-- Role should not be NULL
```

## Enable detailed errors (Development only)

Run with `ASPNETCORE_ENVIRONMENT=Development` and check console logs / `Logs/sso-*.log` for stack traces.

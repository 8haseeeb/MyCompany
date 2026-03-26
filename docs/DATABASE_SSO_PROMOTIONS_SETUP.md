# Connect SSOServiceDb & PromotionsDb and prepare for queries

This solution expects:

| Database | Used by | EF context | Purpose |
|----------|---------|------------|---------|
| **SSOServiceDb** | `SSO.Api` (`DefaultConnection`) | `IdentityDbContext` | Users, sessions, refresh tokens |
| **SSOServiceDb** | `Promotions.Api` (`SsoConnection`) | `SsoDbContext` | Same `Users` table for session validation |
| **PromotionsDb** | `Promotions.Api` (`DefaultConnection`) | `PromotionsDbContext` | Promotion domain tables |

**Important:** `SsoConnection` must point to the **same** database as SSO (`SSOServiceDb`), not PromotionsDb.

---

## 1. Create empty databases (if they do not exist)

Run in SSMS or `sqlcmd` (adjust server):

```sql
IF DB_ID(N'SSOServiceDb') IS NULL
    CREATE DATABASE [SSOServiceDb];
GO
IF DB_ID(N'PromotionsDb') IS NULL
    CREATE DATABASE [PromotionsDb];
GO
```

---

## 2. Connection strings (local SQL Server example)

Use **your** server name (`localhost`, `(localdb)\MSSQLLocalDB`, or a named instance).

**SSO.Api** — `appsettings.json` or User Secrets / environment:

```text
ConnectionStrings__DefaultConnection=Server=localhost;Database=SSOServiceDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```

**Promotions.Api** — two connection strings:

```text
ConnectionStrings__DefaultConnection=Server=localhost;Database=PromotionsDb;Trusted_Connection=True;TrustServerCertificate=True;
ConnectionStrings__SsoConnection=Server=localhost;Database=SSOServiceDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true
```

---

## 3. List all tables (verify they exist)

Run the script:

`scripts/sql/01-list-tables-by-database.sql`

Or manually:

```sql
USE SSOServiceDb;
SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY 1,2;

USE PromotionsDb;
SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY 1,2;
```

---

## 4. Create missing tables (use EF migrations — not hand-written DDL)

The app’s schema is defined by **Entity Framework migrations** in the repo. Do **not** invent “default” tables manually; they will not match keys, columns, and FKs the APIs expect.

### Prerequisites

```bash
dotnet tool install --global dotnet-ef
```

### Apply SSO schema to SSOServiceDb

From the solution folder (`MyCompany`):

```bash
dotnet ef database update --project SSO.Infrastructure --startup-project SSO.Api --context IdentityDbContext --connection "Server=localhost;Database=SSOServiceDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

This creates (if missing):

- `dbo.Users`
- `dbo.__EFMigrationsHistory`

### Apply Promotions schema to PromotionsDb

```bash
dotnet ef database update --project Promotions.Infrastructure --startup-project Promotions.Api --context PromotionsDbContext --connection "Server=localhost;Database=PromotionsDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

This creates (if missing), in `dbo`:

- `TA500PROMOACTION`
- `TA501DELIVERYPOINTS`
- `TA5026PRODUCTDETAILS`
- `TA502PRODUCTS`
- `TA5118PROMOMEASUREFIELDS`
- `TA5150PROMOARTICLES`
- `TA8012PARTICIPANTS`
- `TB0042RELATIONS_CUST`
- `__EFMigrationsHistory`

---

## 5. Alternative: let the APIs migrate on startup

`SSO.Api` and `Promotions.Api` already run `Database.Migrate()` (or equivalent) on startup **when** `appsettings` point at `SSOServiceDb` and `PromotionsDb`. After updating connection strings, start:

1. `SSO.Api`
2. `Promotions.Api`

Then re-run the list-tables script to confirm objects exist.

---

## 6. Ready to run queries

- Open SSMS → connect to your server → `SSOServiceDb` / `PromotionsDb`.
- Example:

```sql
USE SSOServiceDb;
SELECT TOP 10 * FROM dbo.Users;

USE PromotionsDb;
SELECT TOP 10 * FROM dbo.TA500PROMOACTION;
```

---

## 7. Troubleshooting

| Issue | What to check |
|--------|----------------|
| Login works but Promotions returns session errors | `SsoConnection` must use **SSOServiceDb**, same server as SSO. |
| `dotnet ef` cannot find tools | `dotnet tool install --global dotnet-ef` and ensure `%USERPROFILE%\.dotnet\tools` is on PATH. |
| Migration fails “database does not exist” | Create databases (section 1) or fix server name in `--connection`. |
| Tables exist but wrong shape | Drop conflicting tables only in **dev** and re-run `database update`, or add a new migration (advanced). |

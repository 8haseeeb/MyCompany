# Azure Container Apps Deployment Guide

This guide covers configuring GitHub Secrets, production Redis, deploying Gateway, SSO, and Promotions to Azure Container Apps, and validating the deployment.

---

## 1. Summary of Findings

### What was correct
- **JWT_SECRET**: Passed as `JwtSettings__Secret` to all three apps. Code reads `JwtSettings:Secret` → matches.
- **APPINSIGHTS_CONNECTION_STRING**: Passed as `ApplicationInsights__ConnectionString`. Code uses `ApplicationInsights:ConnectionString` → matches.
- **Gateway**: Uses `SSO_URL` and `PROMOTIONS_URL` (set in workflow); no Redis needed.
- **SQL**: `ConnectionStrings__DefaultConnection` (SSO/Promotions) and `ConnectionStrings__SsoConnection` (Promotions) are set from secrets.

### What was fixed
- **Redis**: The workflow did **not** pass Redis to SSO or Promotions. Both apps use `ConnectionStrings:Redis` (env: `ConnectionStrings__Redis`). The workflow now:
  - Requires secret **REDIS_CONNECTION_STRING**.
  - Passes `ConnectionStrings__Redis="$REDIS"` to **SSO API** and **Promotions API**.
- **Validation**: Added checks so the workflow fails early if any required secret is empty (JWT, App Insights, Redis, SQL).

### No code changes required
- SSO and Promotions already read `GetConnectionString("Redis")` and use it when non-empty; no application code changes were needed.

---

## 2. Configure GitHub Secrets

GitHub secret names **cannot** contain colons. Use the exact names below.

| Secret Name | Description | Used By |
|-------------|-------------|---------|
| **ACR_PASSWORD** | Azure Container Registry admin password (or token) | All deploy steps (pull images) |
| **AZURE_CREDENTIALS** | JSON from `az ad sp create-for-rbac` (service principal for GitHub Actions) | Login step |
| **SQL_CONNECTION_STRING_SSO** | SQL Server connection string for SSO DB (e.g. `Server=...;Database=SSOIdentityDb;User Id=...;Password=...;Encrypt=True;TrustServerCertificate=False`) | SSO API, Promotions API (SsoConnection) |
| **SQL_CONNECTION_STRING_PROMOTIONS** | SQL Server connection string for Promotions DB | Promotions API |
| **JWT_SECRET** | Shared secret for JWT signing (same value for Gateway, SSO, Promotions) | Gateway, SSO, Promotions |
| **APPINSIGHTS_CONNECTION_STRING** | Application Insights connection string (from Azure portal) | Gateway, SSO, Promotions |
| **REDIS_CONNECTION_STRING** | Azure Cache for Redis connection string (see below) | SSO API, Promotions API |

**Steps:**

1. In your repo: **Settings → Secrets and variables → Actions**.
2. Click **New repository secret**.
3. Add each secret with the **exact** name in the table (e.g. `REDIS_CONNECTION_STRING`, not `ConnectionStrings:Redis`).
4. For **REDIS_CONNECTION_STRING**: use the full connection string from Azure (hostname, port, password, SSL). Example format:
   ```text
   <your-cache>.redis.cache.windows.net:6380,password=<access-key>,ssl=True,abortConnect=False
   ```

If you already use a different name for Redis (e.g. `REDIS`), either add a new secret `REDIS_CONNECTION_STRING` with the same value, or edit `.github/workflows/deploy.yml` and replace `secrets.REDIS_CONNECTION_STRING` with `secrets.REDIS` (and keep the env var name `REDIS` in the workflow).

---

## 3. Set Up Redis in Production

### 3.1 Create Azure Cache for Redis (if not done)

1. Azure Portal → **Create a resource** → **Azure Cache for Redis**.
2. Resource group: same as your Container Apps (e.g. `xtel-promo-rg`).
3. DNS name: e.g. `mycompany-promo-redis`.
4. Pricing tier: at least Basic for production; Standard/Premium for replication.
5. Create the resource.

### 3.2 Get connection details

1. Open the Redis resource → **Access keys** (or **Connection strings** in newer UX).
2. Copy the **Primary connection string** (StackExchange.Redis format). It looks like:
   ```text
   <hostname>:6380,password=<primary-access-key>,ssl=True,abortConnect=False
   ```
3. Store this value in GitHub Secret **REDIS_CONNECTION_STRING** (see section 2).

### 3.3 Network access (optional but recommended)

- **VNet integration**: If your Container Apps environment is in a VNet, place Redis in the same VNet (or peered) and use the private hostname/port in the connection string.
- **Firewall**: If you use Redis firewall rules, allow outbound traffic from your Container Apps environment (e.g. ACA subnet or egress IPs).

### 3.4 TLS

- Azure Redis uses port **6380** with SSL. Your connection string must include `ssl=True`. The apps use `StackExchangeRedis`; the default client uses TLS when the connection string specifies SSL.

---

## 4. Deploy All Three Apps to Azure Container Apps

### 4.1 Prerequisites

- Azure resource group, ACR, and Container Apps environment already created (or the first run of the workflow will create them via `az containerapp up`).
- GitHub repo connected to Azure (AZURE_CREDENTIALS and ACR_PASSWORD set).

### 4.2 Trigger deployment

- **Push to `main`**: The workflow runs automatically.
- **Manual run**: **Actions** → **Deploy to Azure Container Apps** → **Run workflow**.

### 4.3 Deploy order (in the workflow)

1. Build and push images (SSO, Promotions, Gateway, WebApp).
2. **SSO API** → **Promotions API** → **API Gateway** → **WebApp**.

The workflow uses the same image tag (`github.sha`) for all images so they stay in sync.

### 4.4 Important workflow env vars

- **DOMAIN**: e.g. `yellowplant-40f27ff8.southeastasia.azurecontainerapps.io`. Gateway gets:
  - `SSO_URL=https://sso-api.<DOMAIN>`
  - `PROMOTIONS_URL=https://promotions-api.<DOMAIN>`
- If you change the Container Apps environment or domain, update the `DOMAIN` value in `env:` in `.github/workflows/deploy.yml`.

---

## 5. Validate That Apps Start Without Errors

### 5.1 Check workflow run

1. **Actions** → latest **Deploy to Azure Container Apps** run.
2. Confirm all steps are green (build, push, deploy SSO, deploy Promotions, deploy Gateway, deploy WebApp).
3. If a step fails on secret validation (e.g. "REDIS_CONNECTION_STRING secret is empty"), add or fix that secret and re-run.

### 5.2 Check Container Apps in Azure

1. Azure Portal → your resource group → **Container Apps**.
2. Open **sso-api**, **promotions-api**, **api-gateway**. For each:
   - **Revision management**: Latest revision should be **Active** and **Running**.
   - **Log stream** (or **Logs**): No repeated startup exceptions (e.g. DB connection failures, Redis connection failures).

### 5.3 Health endpoints

- **SSO**: `https://sso-api.<DOMAIN>/api/v1/health` or `https://sso-api.<DOMAIN>/api/health` (if still mapped). Expect 200 and a health payload.
- **Promotions**: `https://promotions-api.<DOMAIN>/api/v1/health`. Expect 200.
- **Gateway**: If you have a gateway health route (e.g. `/api/gateway/health`), call it; otherwise confirm the gateway returns 401 for protected routes and 200 for auth routes (e.g. login).

Replace `<DOMAIN>` with your actual domain from the workflow (e.g. `yellowplant-40f27ff8.southeastasia.azurecontainerapps.io`).

### 5.4 End-to-end checks

1. **Login**: POST to `https://api-gateway.<DOMAIN>/api/auth/login` (or `/api/v1/auth/login`) with credentials → expect 200 and access/refresh tokens.
2. **Promotions with token**: GET `https://api-gateway.<DOMAIN>/api/promotions/actions` (or `/api/v1/promotions/actions`) with `Authorization: Bearer <access_token>` → expect 200 (or 401 if session validation fails).
3. **Logout**: POST `https://api-gateway.<DOMAIN>/api/auth/logout` with Bearer token → expect 200. Then repeat a Promotions call with the same token → expect 401 (session invalidated in Redis).

### 5.5 Application Insights

- In Application Insights (resource linked to your connection string), check **Transaction search** or **Failures** for the same time window as your tests. You should see traces from Gateway, SSO, and Promotions with the same operation/trace ID when calls flow through the gateway.

### 5.6 Redis

- If SSO or Promotions logs show Redis connection errors, verify:
  - **REDIS_CONNECTION_STRING** in GitHub matches the Azure Redis primary connection string (host, port 6380, password, `ssl=True`).
  - Redis firewall/VNet allows traffic from Container Apps.
  - No typo in the secret name in the workflow (`REDIS_CONNECTION_STRING`).

---

## 6. Quick Reference: Required GitHub Secrets

| Secret | Required for |
|--------|------------------|
| ACR_PASSWORD | All deploys |
| AZURE_CREDENTIALS | Azure login |
| SQL_CONNECTION_STRING_SSO | SSO API, Promotions API |
| SQL_CONNECTION_STRING_PROMOTIONS | Promotions API |
| JWT_SECRET | Gateway, SSO, Promotions |
| APPINSIGHTS_CONNECTION_STRING | Gateway, SSO, Promotions |
| REDIS_CONNECTION_STRING | SSO API, Promotions API |

---

## 7. Troubleshooting

- **"Error: REDIS_CONNECTION_STRING secret is empty"**  
  Add the secret in GitHub with the exact name `REDIS_CONNECTION_STRING` and re-run the workflow.

- **App starts but Redis errors in logs**  
  Check connection string format (host:6380, password=..., ssl=True). Ensure Redis is in the same region/VNet if applicable and firewall allows ACA.

- **Session still valid after logout**  
  Both SSO and Promotions must use the same Redis (same REDIS_CONNECTION_STRING). Confirm both Container Apps have `ConnectionStrings__Redis` set (check in Azure Portal → Container App → Environment variables).

- **502/503 from gateway to backend**  
  Confirm SSO_URL and PROMOTIONS_URL in the Gateway container point to the correct FQDNs and that the backend apps are running and returning 200 on their health endpoints.

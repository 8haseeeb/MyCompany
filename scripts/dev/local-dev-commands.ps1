# Copy each block into separate terminals (order matters for first-time startup).
# scripts/dev -> repo root is two levels up
$Root = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path

Write-Host "Using root: $Root" -ForegroundColor Cyan

Write-Host @"

=== 1) API Gateway (5089) ===
cd `"$Root\MyCompany.ApiGateway`"
dotnet run --launch-profile http

=== 2) SSO.Api (see launchSettings for HTTPS port) ===
cd `"$Root\SSO.Api`"
dotnet run

=== 3) Promotions.Api (see launchSettings — often 5137) ===
cd `"$Root\Promotions.Api`"
dotnet run

=== 4) Promotions MFE — vite build && vite preview on 5002 (remoteEntry.js required) ===
cd `"$Root\MyCompany.WebApp\apps\promotions`"
npm install
npm run dev

=== 5) Host React — port 5001 ===
cd `"$Root\MyCompany.WebApp\apps\host`"
npm install
npm run dev

Then open http://localhost:5001 and log in.
Login seed (if you ran scripts/sql/03-sso-users-clear-and-seed.sql):
  Email: dev@local.test
  Password: haseeb123

"@ -ForegroundColor Yellow


                    XTEL PROMO TOOL - COMPREHENSIVE SETUP GUIDE

This document serves as a complete manual for installing, configuring, and running
the XTEL Promo Tool solution.

--------------------------------------------------------------------------------
1. SOFTWARE REQUIREMENTS (PRE-REQUISITES)
--------------------------------------------------------------------------------
Before starting, please ensure the following software is installed on your machine.

1.  **Node.js (LTS Version)**
    - Purpose: Required to run the Frontend (React Application).
    - Version: v18.0.0 or higher.
    - Download: https://nodejs.org/

2.  **.NET 8.0 SDK**
    - Purpose: Required to build and run the Backend APIs and Gateway.
    - Version: .NET 8.0 (Compatible with .NET 9.0 SDK).
    - Download: https://dotnet.microsoft.com/download/dotnet/8.0

3.  **SQL Server (Express or Developer)**
    - Purpose: Database server for storing application data.
    - Download: https://www.microsoft.com/en-us/sql-server/sql-server-downloads

4.  **Visual Studio 2022** (Recommended)
    - Purpose: IDE for managing the project solution.

--------------------------------------------------------------------------------
2. DATABASE SETUP & CONFIGURATION (TWO DATABASES)
--------------------------------------------------------------------------------
The project requires TWO separate databases. You must update the connection strings
in TWO different locations.

Step 1: Configure "Promotions" Database
   - Navigate to the `Promotions.Api` project in Solution Explorer.
   - Open `appsettings.json`.
   - Update `DefaultConnection` string.
     *Recommended Database Name:* `PromotionsDb`
     Example: "Server=.\\SQLEXPRESS;Database=PromotionsDb;Trusted_Connection=True;TrustServerCertificate=True;"
                  \\write your server name if its not SQLEXPRESS.
Step 2: Configure "Identity" (SSO) Database
   - Navigate to the `SSO.Api` project in Solution Explorer.
   - Open `appsettings.json`.
   - Update `DefaultConnection` string.
     *Recommended Database Name:* `SSOIdentityDb`
     Example: "Server=.\\SQLEXPRESS;Database=SSOIdentityDb;Trusted_Connection=True;TrustServerCertificate=True;"
                  \\write your server name if its not SQLEXPRESS.

Step 3: Create Databases (Migrations)
   - Open **Package Manager Console** in Visual Studio.
   
   A. For Promotions DB:
      - Select Default Project: `Promotions.Infrastructure` (or `Promotions.Api`)
      - Run: `Update-Database`
   
   B. For Identity DB:
      - Select Default Project: `SSO.Infrastructure` (or `SSO.Api`)
      - Run: `Update-Database`

--------------------------------------------------------------------------------
3. BACKEND SETUP (MULTIPLE STARTUP PROJECTS)
--------------------------------------------------------------------------------
You must run THREE projects simultaneously for the backend to work.

Step 1: Configure Multiple Startup Projects
   - Right-click on the **Solution ('MyCompany')** in Solution Explorer.
   - Select **Properties**.
   - Go to **Common Properties** > **Startup Project**.
   - Select **"Multiple startup projects"**.
   - Set the "Action" to **Start** for the following three projects:
     
     1. SSO.Api
     2. Promotions.Api
     3. MyCompany.ApiGateway 
   - Click **Apply** and **OK**.

Step 2: Start the Backend
   - Click the green **Start** button (or press F5).
   - Verify that multiple terminal/browser windows open for each service.
   - **Gateway URL:** http://localhost:5089 (This is the main entry point).

--------------------------------------------------------------------------------
4. FRONTEND SETUP (MICRO-FRONTENDS)
--------------------------------------------------------------------------------

Step 1: Open Terminal
   - Open PowerShell or Command Prompt.
   - Navigate to the WebApp folder:
     cd Path\To\MyCompany\MyCompany.WebApp

Step 2: Install Packages (First Time Only)
   - Run: `npm install`

Step 3: Run the Frontend
   - Run: `npm run dev`

   * Host URL: http://localhost:5001 (Open this in Browser)
   * Remote URL: http://localhost:5002 (Runs in background)

--------------------------------------------------------------------------------
5. LOGIN CREDENTIALS
--------------------------------------------------------------------------------
Once the app is running at http://localhost:5001:

- Username: ali@gmail.com
- Password: ali123

//if u register your slef then its also fine. First register your self in which u have to given user name , email address and password. Then login.

If login fails, ensure `SSO.Api` is running and the `SSOIdentityDb` was created successfully.

================================================================================

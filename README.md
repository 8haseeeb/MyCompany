# XTEL PROMO TOOL - COMPREHENSIVE SETUP GUIDE

This document serves as a complete manual for installing, configuring, and running the XTEL Promo Tool solution.

---

## 1. SOFTWARE REQUIREMENTS (PRE-REQUISITES)

Before starting, please ensure the following software is installed on your machine.

1.  **Node.js (LTS Version)**
    - **Purpose:** Required to run the Frontend (React Application).
    - **Version:** v18.0.0 or higher.
    - **Download:** [https://nodejs.org/](https://nodejs.org/)

2.  **.NET 8.0 SDK**
    - **Purpose:** Required to build and run the Backend APIs and Gateway.
    - **Version:** .NET 8.0 (Compatible with .NET 9.0 SDK).
    - **Download:** [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

3.  **SQL Server (Express or Developer)**
    - **Purpose:** Database server for storing application data.
    - **Download:** [https://www.microsoft.com/en-us/sql-server/sql-server-downloads](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

4.  **Visual Studio 2022** (Recommended)
    - **Purpose:** IDE for managing the project solution.

---

## 2. DATABASE SETUP & CONFIGURATION (TWO DATABASES)

The project requires **TWO separate databases**. You must update the connection strings in **TWO different locations**.

### Step 1: Configure "Promotions" Database
1.  Navigate to the `Promotions.Api` project in Solution Explorer.
2.  Open `appsettings.json`.
3.  Update the `DefaultConnection` string.
    *   **Recommended Database Name:** `PromotionsDb`
    *   **Example:**
        ```json
        "DefaultConnection": "Server=.\\SQLEXPRESS;Database=PromotionsDb;Trusted_Connection=True;TrustServerCertificate=True;"
        ```
    *   *(Note: Write your server name if it's not `SQLEXPRESS`)*.

### Step 2: Configure "Identity" (SSO) Database
1.  Navigate to the `SSO.Api` project in Solution Explorer.
2.  Open `appsettings.json`.
3.  Update the `DefaultConnection` string.
    *   **Recommended Database Name:** `SSOIdentityDb`
    *   **Example:**
        ```json
        "DefaultConnection": "Server=.\\SQLEXPRESS;Database=SSOIdentityDb;Trusted_Connection=True;TrustServerCertificate=True;"
        ```
    *   *(Note: Write your server name if it's not `SQLEXPRESS`)*.

### Step 3: Create Databases (Migrations)
1.  Open **Package Manager Console** in Visual Studio.
2.  **For Promotions DB:**
    -   Select Default Project: `Promotions.Infrastructure` (or `Promotions.Api`).
    -   Run Command: `Update-Database`
3.  **For Identity DB:**
    -   Select Default Project: `SSO.Infrastructure` (or `SSO.Api`).
    -   Run Command: `Update-Database`

---

## 3. BACKEND SETUP (MULTIPLE STARTUP PROJECTS)

You must run **THREE projects simultaneously** for the backend to work.

### Step 1: Configure Multiple Startup Projects
1.  Right-click on the **Solution ('MyCompany')** in Solution Explorer.
2.  Select **Properties**.
3.  Go to **Common Properties** > **Startup Project**.
4.  Select **"Multiple startup projects"**.
5.  Set the "Action" to **Start** for the following three projects:
    1.  `SSO.Api`
    2.  `Promotions.Api`
    3.  `MyCompany.ApiGateway`
6.  Click **Apply** and **OK**.

### Step 2: Start the Backend
1.  Click the green **Start** button (or press `F5`).
2.  Verify that multiple terminal/browser windows open for each service.
3.  **Gateway URL:** `http://localhost:5089` (This is the main entry point).

---

## 4. FRONTEND SETUP (MICRO-FRONTENDS)

### Step 1: Open Terminal
1.  Open PowerShell or Command Prompt.
2.  Navigate to the WebApp folder:
    ```bash
    cd Path\To\MyCompany\MyCompany.WebApp
    ```

### Step 2: Install Packages (First Time Only)
Run the following command:
```bash
npm install
```

### Step 3: Run the Frontend
Run the development server:
```bash
npm run dev
```

*   **Host URL:** `http://localhost:5001` (Open this in Browser)
*   **Remote URL:** `http://localhost:5002` (Runs in background)

---

## 5. LOGIN CREDENTIALS

Once the app is running at `http://localhost:5001`:

*   **Username:** `ali@gmail.com`
*   **Password:** `ali123`

*(Note: If you want to register yourself, that is also fine. Simply go to the Register page, enter your User Name, Email, and Password, and then Login.)*

If login fails, ensure `SSO.Api` is running and the `SSOIdentityDb` was created successfully.

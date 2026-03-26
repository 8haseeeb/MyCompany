-- Run in SQL Server Management Studio or sqlcmd.
-- Replace server name if needed. Uses your database names: SSOServiceDb and PromotionsDb.

-- ========== SSOServiceDb ==========
USE [SSOServiceDb];
GO

SELECT
    s.name AS [Schema],
    t.name AS [TableName]
FROM sys.tables t
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
ORDER BY s.name, t.name;
GO

-- Expected application tables (EF): dbo.Users, dbo.__EFMigrationsHistory
-- If Users is missing, apply SSO migrations (see docs/DATABASE_SSO_PROMOTIONS_SETUP.md).

-- ========== PromotionsDb ==========
USE [PromotionsDb];
GO

SELECT
    s.name AS [Schema],
    t.name AS [TableName]
FROM sys.tables t
INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
ORDER BY s.name, t.name;
GO

-- Expected application tables (EF), all dbo:
--   TA500PROMOACTION, TA501DELIVERYPOINTS, TA5026PRODUCTDETAILS, TA502PRODUCTS,
--   TA5118PROMOMEASUREFIELDS, TA5150PROMOARTICLES, TA8012PARTICIPANTS,
--   TB0042RELATIONS_CUST, __EFMigrationsHistory

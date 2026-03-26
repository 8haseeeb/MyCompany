-- Dev reference data so atomic promotion create passes CodDiv validation
-- (CodDiv must exist on TB0042RELATIONS_CUST and/or TA501DELIVERYPOINTS).
-- Run against PromotionsDb (adjust database name if needed).
-- Idempotent: skips if DIV_1 / H1 / NODE_1 row already exists.

USE PromotionsDb;
GO

IF NOT EXISTS (
    SELECT 1
    FROM dbo.TB0042RELATIONS_CUST
    WHERE CODDIV = N'DIV_1'
      AND CODHER = N'H1'
      AND CODNODE = N'NODE_1'
      AND IDLEVEL = 1
      AND DTESTART = '2020-01-01'
)
BEGIN
    INSERT INTO dbo.TB0042RELATIONS_CUST (
        CODHER, CODDIV, CODNODE, IDLEVEL, DTESTART, COOPAREANTNODE, DTEEND
    )
    VALUES (N'H1', N'DIV_1', N'NODE_1', 1, '2020-01-01', NULL, NULL);
    PRINT 'Inserted TB0042RELATIONS_CUST seed row for DIV_1.';
END
ELSE
    PRINT 'TB0042RELATIONS_CUST seed for DIV_1 already present.';
GO

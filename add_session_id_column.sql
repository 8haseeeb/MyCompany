-- SQL Script to add CurrentSessionId column to Users table
-- Run this on the SSOIdentityDb database

IF NOT EXISTS (
    SELECT * 
    FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Users]') 
    AND name = 'CurrentSessionId'
)
BEGIN
    ALTER TABLE [dbo].[Users]
    ADD [CurrentSessionId] NVARCHAR(100) NULL;
    
    PRINT 'Column CurrentSessionId added successfully.';
END
ELSE
BEGIN
    PRINT 'Column CurrentSessionId already exists.';
END
GO

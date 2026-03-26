-- Run in SSMS against SSOServiceDb ONLY when:
--   - Table dbo.Users already exists (and matches the InitialCreate migration), and
--   - dbo.__EFMigrationsHistory is missing or empty,
--   so EF keeps failing with error 2714 ("object already exists").
-- Adjust ProductVersion if your Microsoft.EntityFrameworkCore package differs.

IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory] (
        [MigrationId]    nvarchar(150) NOT NULL CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY,
        [ProductVersion] nvarchar(32)  NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20260319071704_InitialCreate')
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260319071704_InitialCreate', N'8.0.0');
GO

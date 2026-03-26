-- Run in SSMS against PromotionsDb ONLY when:
--   - All tables from migrations already exist, and
--   - dbo.__EFMigrationsHistory is missing or out of sync,
--   so the app logs migration errors (e.g. 1801 loops or missing history).
-- Order must match chronological migration order. ProductVersion = EF Core package version.

IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory] (
        [MigrationId]    nvarchar(150) NOT NULL CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY,
        [ProductVersion] nvarchar(32)  NOT NULL
    );
END
GO

MERGE INTO [dbo].[__EFMigrationsHistory] AS T
USING (VALUES
    (N'20260107102207_InitialDecoupledModel', N'8.0.0'),
    (N'20260112092935_RemoveIdActionFromMeasureFields', N'8.0.0'),
    (N'20260223084411_FixCodDivSchemaV2', N'8.0.0'),
    (N'20260223084453_FixCodDivSchemaFinal', N'8.0.0'),
    (N'20260223100000_FixBigIntColumns', N'8.0.0'),
    (N'20260223110000_DropArticleForeignKey', N'8.0.0'),
    (N'20260325064636_ProductDetailFlgInclusionAndOptionalArticle', N'8.0.0')
) AS S ([MigrationId], [ProductVersion])
ON T.[MigrationId] = S.[MigrationId]
WHEN NOT MATCHED THEN INSERT ([MigrationId], [ProductVersion]) VALUES (S.[MigrationId], S.[ProductVersion]);
GO

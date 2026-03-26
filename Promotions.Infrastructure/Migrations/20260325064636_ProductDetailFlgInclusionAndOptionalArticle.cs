using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    /// <summary>
    /// Aligns DB with model: optional product-detail → article (FK stays dropped per DropArticleForeignKey),
    /// nullable FLGINCLUSION handled in EF via value converter, CodNodeN as string, CODMEASURE width.
    /// </summary>
    public partial class ProductDetailFlgInclusionAndOptionalArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Do not re-create FK to TA5150PROMOARTICLES — catalog rows may be missing.
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_TA5026PRODUCTDETAILS_TA5150PROMOARTICLES_CODDIV_CODNODEO')
                    ALTER TABLE [dbo].[TA5026PRODUCTDETAILS] DROP CONSTRAINT [FK_TA5026PRODUCTDETAILS_TA5150PROMOARTICLES_CODDIV_CODNODEO];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'TA502PRODUCTS' AND COLUMN_NAME = N'CODMEASURE'
                      AND DATA_TYPE = N'nvarchar' AND CHARACTER_MAXIMUM_LENGTH > 0 AND CHARACTER_MAXIMUM_LENGTH < 50)
                BEGIN
                    ALTER TABLE [dbo].[TA502PRODUCTS] ALTER COLUMN [CODMEASURE] NVARCHAR(50) NULL;
                END
            ");

            // SQL Server parses the whole batch before executing: referencing FROMNODEFIN_NEW in the same
            // batch as ALTER TABLE ADD fails (error 207). Use separate Sql() calls so each runs as its own batch.
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'TA5150PROMOARTICLES' AND COLUMN_NAME = N'FROMNODEFIN' AND DATA_TYPE = N'bit')
                AND NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'TA5150PROMOARTICLES' AND COLUMN_NAME = N'FROMNODEFIN_NEW')
                BEGIN
                    ALTER TABLE [dbo].[TA5150PROMOARTICLES] ADD [FROMNODEFIN_NEW] NVARCHAR(50) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'TA5150PROMOARTICLES' AND COLUMN_NAME = N'FROMNODEFIN' AND DATA_TYPE = N'bit')
                AND EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'TA5150PROMOARTICLES' AND COLUMN_NAME = N'FROMNODEFIN_NEW')
                BEGIN
                    UPDATE [dbo].[TA5150PROMOARTICLES]
                        SET [FROMNODEFIN_NEW] = CASE WHEN [FROMNODEFIN] = CAST(1 AS bit) THEN N'1' ELSE N'0' END
                        WHERE [FROMNODEFIN] IS NOT NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'TA5150PROMOARTICLES' AND COLUMN_NAME = N'FROMNODEFIN' AND DATA_TYPE = N'bit')
                AND EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'TA5150PROMOARTICLES' AND COLUMN_NAME = N'FROMNODEFIN_NEW')
                BEGIN
                    ALTER TABLE [dbo].[TA5150PROMOARTICLES] DROP COLUMN [FROMNODEFIN];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'TA5150PROMOARTICLES' AND COLUMN_NAME = N'FROMNODEFIN_NEW')
                BEGIN
                    EXEC sp_rename N'dbo.TA5150PROMOARTICLES.FROMNODEFIN_NEW', N'FROMNODEFIN', 'COLUMN';
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Non-reversible data/type changes; leave empty.
        }
    }
}

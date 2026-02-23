using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropArticleForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the FK from TA5026PRODUCTDETAILS -> TA5150PROMOARTICLES.
            // PromoArticles are master/catalog data managed externally.
            // The handler does not create articles, so this FK causes an INSERT
            // violation when creating a promotion with product details.

            migrationBuilder.Sql(@"
                -- Drop the index created for the FK
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_TA5026PRODUCTDETAILS_CODDIV_CODNODEO' AND object_id = OBJECT_ID('dbo.TA5026PRODUCTDETAILS'))
                    DROP INDEX [IX_TA5026PRODUCTDETAILS_CODDIV_CODNODEO] ON [dbo].[TA5026PRODUCTDETAILS];

                -- Drop the FK constraint
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TA5026PRODUCTDETAILS_TA5150PROMOARTICLES_CODDIV_CODNODEO')
                    ALTER TABLE [dbo].[TA5026PRODUCTDETAILS] DROP CONSTRAINT [FK_TA5026PRODUCTDETAILS_TA5150PROMOARTICLES_CODDIV_CODNODEO];

                -- Also drop any variant names
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TA5026PRODUCTDETAILS_TA5150PROMOARTICLES')
                    ALTER TABLE [dbo].[TA5026PRODUCTDETAILS] DROP CONSTRAINT [FK_TA5026PRODUCTDETAILS_TA5150PROMOARTICLES];
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Re-create the FK if needed (optional rollback path)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TA5026PRODUCTDETAILS_TA5150PROMOARTICLES_CODDIV_CODNODEO')
                    ALTER TABLE [dbo].[TA5026PRODUCTDETAILS]
                        ADD CONSTRAINT [FK_TA5026PRODUCTDETAILS_TA5150PROMOARTICLES_CODDIV_CODNODEO]
                        FOREIGN KEY ([CODDIV], [CODNODEO])
                        REFERENCES [dbo].[TA5150PROMOARTICLES] ([CODDIV], [CODNODEO])
                        ON DELETE RESTRICT;
            ");
        }
    }
}

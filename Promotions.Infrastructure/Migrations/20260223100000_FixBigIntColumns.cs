using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBigIntColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =====================================================================
            // This migration fixes columns that were BIGINT in the original Azure DB
            // but are mapped as string in the EF model.
            // We use raw SQL to handle the constraint drops/re-creates safely.
            // =====================================================================

            // --- TA502PRODUCTS: CODPRODUCT (BIGINT -> NVARCHAR) ---
            migrationBuilder.Sql(@"
                -- Drop Unique Constraint
                IF EXISTS (SELECT 1 FROM sys.objects WHERE name = 'UQ_TA502PRODUCTS_IDACTION_CODPRODUCT' AND type = 'UQ')
                    ALTER TABLE [dbo].[TA502PRODUCTS] DROP CONSTRAINT [UQ_TA502PRODUCTS_IDACTION_CODPRODUCT];

                -- Drop FK from TA5026PRODUCTDETAILS -> TA502PRODUCTS
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TA5026PRODUCTDETAILS_TA502PRODUCTS_IDACTION_CODPRODUCT_LEVPRODUCT_CODDISPLAY')
                    ALTER TABLE [dbo].[TA5026PRODUCTDETAILS] DROP CONSTRAINT [FK_TA5026PRODUCTDETAILS_TA502PRODUCTS_IDACTION_CODPRODUCT_LEVPRODUCT_CODDISPLAY];

                -- Drop PK on TA5026PRODUCTDETAILS
                IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'PK_TA5026PRODUCTDETAILS')
                    ALTER TABLE [dbo].[TA5026PRODUCTDETAILS] DROP CONSTRAINT [PK_TA5026PRODUCTDETAILS];

                -- Drop PK on TA502PRODUCTS
                IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'PK_TA502PRODUCTS')
                    ALTER TABLE [dbo].[TA502PRODUCTS] DROP CONSTRAINT [PK_TA502PRODUCTS];

                -- Alter columns  
                ALTER TABLE [dbo].[TA502PRODUCTS] ALTER COLUMN [CODPRODUCT] NVARCHAR(50) NOT NULL;
                ALTER TABLE [dbo].[TA502PRODUCTS] ALTER COLUMN [CODDISPLAY] NVARCHAR(50) NOT NULL;

                -- Re-create PK
                ALTER TABLE [dbo].[TA502PRODUCTS] ADD CONSTRAINT [PK_TA502PRODUCTS] PRIMARY KEY CLUSTERED ([IDACTION] ASC, [CODPRODUCT] ASC, [LEVPRODUCT] ASC, [CODDISPLAY] ASC);
            ");

            // --- TA5026PRODUCTDETAILS: CODPRODUCT (BIGINT -> NVARCHAR) ---
            migrationBuilder.Sql(@"
                ALTER TABLE [dbo].[TA5026PRODUCTDETAILS] ALTER COLUMN [CODPRODUCT] NVARCHAR(50) NOT NULL;
                ALTER TABLE [dbo].[TA5026PRODUCTDETAILS] ALTER COLUMN [CODDISPLAY] NVARCHAR(50) NOT NULL;

                -- Re-create PK
                IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'PK_TA5026PRODUCTDETAILS')
                    ALTER TABLE [dbo].[TA5026PRODUCTDETAILS] ADD CONSTRAINT [PK_TA5026PRODUCTDETAILS] PRIMARY KEY CLUSTERED ([IDACTION] ASC, [CODPRODUCT] ASC, [LEVPRODUCT] ASC, [CODDISPLAY] ASC, [CODDIV] ASC, [CODNODEO] ASC);

                -- Re-create FK from TA5026PRODUCTDETAILS -> TA502PRODUCTS
                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_TA5026PRODUCTDETAILS_TA502PRODUCTS_IDACTION_CODPRODUCT_LEVPRODUCT_CODDISPLAY')
                    ALTER TABLE [dbo].[TA5026PRODUCTDETAILS] ADD CONSTRAINT [FK_TA5026PRODUCTDETAILS_TA502PRODUCTS_IDACTION_CODPRODUCT_LEVPRODUCT_CODDISPLAY]
                        FOREIGN KEY ([IDACTION], [CODPRODUCT], [LEVPRODUCT], [CODDISPLAY])
                        REFERENCES [dbo].[TA502PRODUCTS] ([IDACTION], [CODPRODUCT], [LEVPRODUCT], [CODDISPLAY])
                        ON DELETE CASCADE;
            ");

            // --- TA8012PARTICIPANTS: CODPARTICIPANT (BIGINT -> NVARCHAR) ---
            migrationBuilder.Sql(@"
                -- Drop PK on TA8012PARTICIPANTS
                IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'PK_TA8012PARTICIPANTS')
                    ALTER TABLE [dbo].[TA8012PARTICIPANTS] DROP CONSTRAINT [PK_TA8012PARTICIPANTS];

                -- Also handle legacy table name
                IF EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'PK_TA5012PARTICIPANTS')
                    ALTER TABLE [dbo].[TA5012PARTICIPANTS] DROP CONSTRAINT [PK_TA5012PARTICIPANTS];

                -- Alter column
                IF OBJECT_ID('dbo.TA8012PARTICIPANTS') IS NOT NULL
                    ALTER TABLE [dbo].[TA8012PARTICIPANTS] ALTER COLUMN [CODPARTICIPANT] NVARCHAR(450) NOT NULL;

                IF OBJECT_ID('dbo.TA5012PARTICIPANTS') IS NOT NULL
                    ALTER TABLE [dbo].[TA5012PARTICIPANTS] ALTER COLUMN [CODPARTICIPANT] NVARCHAR(450) NOT NULL;

                -- Re-create PK
                IF OBJECT_ID('dbo.TA8012PARTICIPANTS') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'PK_TA8012PARTICIPANTS')
                    ALTER TABLE [dbo].[TA8012PARTICIPANTS] ADD CONSTRAINT [PK_TA8012PARTICIPANTS] PRIMARY KEY CLUSTERED ([IDACTION] ASC, [CODPARTICIPANT] ASC);

                IF OBJECT_ID('dbo.TA5012PARTICIPANTS') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'PK_TA5012PARTICIPANTS')
                    ALTER TABLE [dbo].[TA5012PARTICIPANTS] ADD CONSTRAINT [PK_TA5012PARTICIPANTS] PRIMARY KEY CLUSTERED ([IDACTION] ASC, [CODPARTICIPANT] ASC);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverting these would require knowing old data, which we cannot guarantee.
            // These columns have always been intended as strings; this is a data fix only.
        }
    }
}

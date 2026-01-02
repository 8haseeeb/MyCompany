using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TA5020PRODUCTS",
                columns: table => new
                {
                    IdAction = table.Column<int>(type: "int", nullable: false),
                    CodProduct = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LevProduct = table.Column<int>(type: "int", nullable: false),
                    CodDisplay = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodDiv = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    QtyEstimated = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PerceDiscount1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PerceDiscount2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NumMeasure = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CodMeasure = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TA5020PRODUCTS", x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TA5020PRODUCTS");
        }
    }
}

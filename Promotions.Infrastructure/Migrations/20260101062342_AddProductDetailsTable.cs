using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductDetailsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TA5026PRODUCTDETAILS",
                columns: table => new
                {
                    IdAction = table.Column<int>(type: "int", nullable: false),
                    CodProduct = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LevProduct = table.Column<int>(type: "int", nullable: false),
                    CodDisplay = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodNode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodDiv = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FlgInclusion = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TA5026PRODUCTDETAILS", x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay, x.CodNode, x.CodDiv });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TA5026PRODUCTDETAILS");
        }
    }
}

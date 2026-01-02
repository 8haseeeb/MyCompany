using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPromoArticlesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TA5150PROMOARTICLE",
                columns: table => new
                {
                    CODDIV = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CODNODE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CODNODE1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CODNODE2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CODNODE_N = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TA5150PROMOARTICLE", x => new { x.CODDIV, x.CODNODE });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TA5150PROMOARTICLE");
        }
    }
}

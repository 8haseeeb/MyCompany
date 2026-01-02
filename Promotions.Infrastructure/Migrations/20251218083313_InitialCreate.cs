using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TA5118PROMOMEASUREFIELDS",
                columns: table => new
                {
                    CODDIV = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CODMEASURE = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FIELDNAME = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FORMULA = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TA5118PROMOMEASUREFIELDS", x => new { x.CODDIV, x.CODMEASURE, x.FIELDNAME });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TA5118PROMOMEASUREFIELDS");
        }
    }
}

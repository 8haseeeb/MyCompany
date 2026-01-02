using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPromoActionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TA500PROMOACTION",
                columns: table => new
                {
                    ID_ACTION = table.Column<int>(type: "int", nullable: false),
                    DESACTION = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CODDIV = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CODCONTRACTOR = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DTESTARTSELLIN = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DTEENDSELLIN = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DTESTARTSELLOUT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DTEENDSELLOUT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DOCUMENTKEY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DTETOSHOST = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LEVPARTICIPANTS = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TA500PROMOACTION", x => x.ID_ACTION);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TA500PROMOACTION");
        }
    }
}

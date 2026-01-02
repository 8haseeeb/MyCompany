using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryPointsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TA5014DELIVERYPOINTS",
                columns: table => new
                {
                    ID_ACTION = table.Column<int>(type: "int", nullable: false),
                    CODDELIVERYPOINT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FLGINCLUSION = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TA5014DELIVERYPOINTS", x => new { x.ID_ACTION, x.CODDELIVERYPOINT });
                });

            migrationBuilder.CreateIndex(
                name: "IX_TA5014DELIVERYPOINTS_ID_ACTION",
                table: "TA5014DELIVERYPOINTS",
                column: "ID_ACTION");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TA5014DELIVERYPOINTS");
        }
    }
}

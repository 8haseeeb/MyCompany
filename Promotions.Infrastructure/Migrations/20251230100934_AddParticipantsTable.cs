using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddParticipantsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TA5012PARTICIPANTS",
                columns: table => new
                {
                    ID_ACTION = table.Column<int>(type: "int", nullable: false),
                    CODPARTICIPANT = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FLGINCLUSION = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TA5012PARTICIPANTS", x => new { x.ID_ACTION, x.CODPARTICIPANT });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TA5012PARTICIPANTS");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerRelationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB0042RELATIONS_CUST",
                columns: table => new
                {
                    CodHier = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CodDiv = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CodNode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IdLevel = table.Column<int>(type: "int", nullable: false),
                    DteStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CodParentNode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DteEnd = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB0042RELATIONS_CUST", x => new { x.CodHier, x.CodDiv, x.CodNode, x.IdLevel, x.DteStart });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB0042RELATIONS_CUST");
        }
    }
}

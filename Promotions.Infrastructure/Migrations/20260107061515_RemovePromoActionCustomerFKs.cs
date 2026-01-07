using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePromoActionCustomerFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TA500PROMOACTION_TB0042RELATIONS_CUST_CONTRACTORCODHIER_CODDIV_CODCONTRACTOR_CONTRACTORIDLEVEL_CONTRACTORDTESTART",
                table: "TA500PROMOACTION");

            migrationBuilder.DropIndex(
                name: "IX_TA500PROMOACTION_CONTRACTORCODHIER_CODDIV_CODCONTRACTOR_CONTRACTORIDLEVEL_CONTRACTORDTESTART",
                table: "TA500PROMOACTION");

            migrationBuilder.DropColumn(
                name: "CONTRACTORCODHIER",
                table: "TA500PROMOACTION");

            migrationBuilder.DropColumn(
                name: "CONTRACTORDTESTART",
                table: "TA500PROMOACTION");

            migrationBuilder.DropColumn(
                name: "CONTRACTORIDLEVEL",
                table: "TA500PROMOACTION");

            migrationBuilder.AlterColumn<string>(
                name: "CODDIV",
                table: "TA500PROMOACTION",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "CODCONTRACTOR",
                table: "TA500PROMOACTION",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CODDIV",
                table: "TA500PROMOACTION",
                type: "nvarchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CODCONTRACTOR",
                table: "TA500PROMOACTION",
                type: "nvarchar(30)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "CONTRACTORCODHIER",
                table: "TA500PROMOACTION",
                type: "nvarchar(10)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CONTRACTORDTESTART",
                table: "TA500PROMOACTION",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CONTRACTORIDLEVEL",
                table: "TA500PROMOACTION",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TA500PROMOACTION_CONTRACTORCODHIER_CODDIV_CODCONTRACTOR_CONTRACTORIDLEVEL_CONTRACTORDTESTART",
                table: "TA500PROMOACTION",
                columns: new[] { "CONTRACTORCODHIER", "CODDIV", "CODCONTRACTOR", "CONTRACTORIDLEVEL", "CONTRACTORDTESTART" });

            migrationBuilder.AddForeignKey(
                name: "FK_TA500PROMOACTION_TB0042RELATIONS_CUST_CONTRACTORCODHIER_CODDIV_CODCONTRACTOR_CONTRACTORIDLEVEL_CONTRACTORDTESTART",
                table: "TA500PROMOACTION",
                columns: new[] { "CONTRACTORCODHIER", "CODDIV", "CODCONTRACTOR", "CONTRACTORIDLEVEL", "CONTRACTORDTESTART" },
                principalTable: "TB0042RELATIONS_CUST",
                principalColumns: new[] { "CodHier", "CodDiv", "CodNode", "IdLevel", "DteStart" });
        }
    }
}

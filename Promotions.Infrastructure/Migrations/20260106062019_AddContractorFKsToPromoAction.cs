using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContractorFKsToPromoAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TA500PROMOACTION_TB0042RELATIONS_CUST_ContractorCodHier_ContractorCodDiv_ContractorCodNode_ContractorIdLevel_ContractorDteSt~",
                table: "TA500PROMOACTION");

            migrationBuilder.DropIndex(
                name: "IX_TA500PROMOACTION_ContractorCodHier_ContractorCodDiv_ContractorCodNode_ContractorIdLevel_ContractorDteStart",
                table: "TA500PROMOACTION");

            migrationBuilder.DropColumn(
                name: "ContractorCodDiv",
                table: "TA500PROMOACTION");

            migrationBuilder.DropColumn(
                name: "ContractorCodNode",
                table: "TA500PROMOACTION");

            migrationBuilder.RenameColumn(
                name: "ContractorIdLevel",
                table: "TA500PROMOACTION",
                newName: "CONTRACTORIDLEVEL");

            migrationBuilder.RenameColumn(
                name: "ContractorDteStart",
                table: "TA500PROMOACTION",
                newName: "CONTRACTORDTESTART");

            migrationBuilder.RenameColumn(
                name: "ContractorCodHier",
                table: "TA500PROMOACTION",
                newName: "CONTRACTORCODHIER");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TA500PROMOACTION_TB0042RELATIONS_CUST_CONTRACTORCODHIER_CODDIV_CODCONTRACTOR_CONTRACTORIDLEVEL_CONTRACTORDTESTART",
                table: "TA500PROMOACTION");

            migrationBuilder.DropIndex(
                name: "IX_TA500PROMOACTION_CONTRACTORCODHIER_CODDIV_CODCONTRACTOR_CONTRACTORIDLEVEL_CONTRACTORDTESTART",
                table: "TA500PROMOACTION");

            migrationBuilder.RenameColumn(
                name: "CONTRACTORIDLEVEL",
                table: "TA500PROMOACTION",
                newName: "ContractorIdLevel");

            migrationBuilder.RenameColumn(
                name: "CONTRACTORDTESTART",
                table: "TA500PROMOACTION",
                newName: "ContractorDteStart");

            migrationBuilder.RenameColumn(
                name: "CONTRACTORCODHIER",
                table: "TA500PROMOACTION",
                newName: "ContractorCodHier");

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

            migrationBuilder.AddColumn<string>(
                name: "ContractorCodDiv",
                table: "TA500PROMOACTION",
                type: "nvarchar(10)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContractorCodNode",
                table: "TA500PROMOACTION",
                type: "nvarchar(30)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TA500PROMOACTION_ContractorCodHier_ContractorCodDiv_ContractorCodNode_ContractorIdLevel_ContractorDteStart",
                table: "TA500PROMOACTION",
                columns: new[] { "ContractorCodHier", "ContractorCodDiv", "ContractorCodNode", "ContractorIdLevel", "ContractorDteStart" });

            migrationBuilder.AddForeignKey(
                name: "FK_TA500PROMOACTION_TB0042RELATIONS_CUST_ContractorCodHier_ContractorCodDiv_ContractorCodNode_ContractorIdLevel_ContractorDteSt~",
                table: "TA500PROMOACTION",
                columns: new[] { "ContractorCodHier", "ContractorCodDiv", "ContractorCodNode", "ContractorIdLevel", "ContractorDteStart" },
                principalTable: "TB0042RELATIONS_CUST",
                principalColumns: new[] { "CodHier", "CodDiv", "CodNode", "IdLevel", "DteStart" });
        }
    }
}

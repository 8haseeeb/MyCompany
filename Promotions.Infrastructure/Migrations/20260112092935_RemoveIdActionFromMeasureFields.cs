using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIdActionFromMeasureFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TA5118PROMOMEASUREFIELDS_TA5020PRODUCTS_IdAction_CodProduct_LevProduct_CodDisplay",
                table: "TA5118PROMOMEASUREFIELDS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TA5118PROMOMEASUREFIELDS",
                table: "TA5118PROMOMEASUREFIELDS");

            migrationBuilder.DropColumn(
                name: "IdAction",
                table: "TA5118PROMOMEASUREFIELDS");

            migrationBuilder.DropColumn(
                name: "CodProduct",
                table: "TA5118PROMOMEASUREFIELDS");

            migrationBuilder.DropColumn(
                name: "LevProduct",
                table: "TA5118PROMOMEASUREFIELDS");

            migrationBuilder.DropColumn(
                name: "CodDisplay",
                table: "TA5118PROMOMEASUREFIELDS");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TA5118PROMOMEASUREFIELDS",
                table: "TA5118PROMOMEASUREFIELDS",
                columns: new[] { "CODDIV", "CODMEASURE", "FIELDNAME" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TA5118PROMOMEASUREFIELDS",
                table: "TA5118PROMOMEASUREFIELDS");

            migrationBuilder.AddColumn<int>(
                name: "IdAction",
                table: "TA5118PROMOMEASUREFIELDS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CodProduct",
                table: "TA5118PROMOMEASUREFIELDS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LevProduct",
                table: "TA5118PROMOMEASUREFIELDS",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CodDisplay",
                table: "TA5118PROMOMEASUREFIELDS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TA5118PROMOMEASUREFIELDS",
                table: "TA5118PROMOMEASUREFIELDS",
                columns: new[] { "IdAction", "CodProduct", "LevProduct", "CodDisplay", "CODDIV", "CODMEASURE", "FIELDNAME" });

            migrationBuilder.AddForeignKey(
                name: "FK_TA5118PROMOMEASUREFIELDS_TA5020PRODUCTS_IdAction_CodProduct_LevProduct_CodDisplay",
                table: "TA5118PROMOMEASUREFIELDS",
                columns: new[] { "IdAction", "CodProduct", "LevProduct", "CodDisplay" },
                principalTable: "TA5020PRODUCTS",
                principalColumns: new[] { "IdAction", "CodProduct", "LevProduct", "CodDisplay" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}

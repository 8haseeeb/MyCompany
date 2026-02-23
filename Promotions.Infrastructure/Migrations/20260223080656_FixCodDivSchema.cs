using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCodDivSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TA5012PARTICIPANTS_TA500PROMOACTION_ID_ACTION",
                table: "TA5012PARTICIPANTS");

            migrationBuilder.DropForeignKey(
                name: "FK_TA5012PARTICIPANTS_TB0042RELATIONS_CUST_CodHier_CodDiv_CodNode_IdLevel_DteStart",
                table: "TA5012PARTICIPANTS");

            migrationBuilder.DropForeignKey(
                name: "FK_TA5014DELIVERYPOINTS_TA500PROMOACTION_ID_ACTION",
                table: "TA5014DELIVERYPOINTS");

            migrationBuilder.DropForeignKey(
                name: "FK_TA5014DELIVERYPOINTS_TB0042RELATIONS_CUST_CodHier_CodDiv_CodNode_IdLevel_DteStart",
                table: "TA5014DELIVERYPOINTS");

            migrationBuilder.DropForeignKey(
                name: "FK_TA5020PRODUCTS_TA500PROMOACTION_IdAction",
                table: "TA5020PRODUCTS");

            migrationBuilder.DropForeignKey(
                name: "FK_TA5026PRODUCTDETAILS_TA5020PRODUCTS_IdAction_CodProduct_LevProduct_CodDisplay",
                table: "TA5026PRODUCTDETAILS");

            migrationBuilder.DropForeignKey(
                name: "FK_TA5150PROMOARTICLE_TA5026PRODUCTDETAILS_IdAction_CodProduct_LevProduct_CodDisplay_CODNODE_CODDIV",
                table: "TA5150PROMOARTICLE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TA5150PROMOARTICLE",
                table: "TA5150PROMOARTICLE");

            migrationBuilder.DropIndex(
                name: "IX_TA5150PROMOARTICLE_IdAction_CodProduct_LevProduct_CodDisplay_CODNODE_CODDIV",
                table: "TA5150PROMOARTICLE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TA5020PRODUCTS",
                table: "TA5020PRODUCTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TA5014DELIVERYPOINTS",
                table: "TA5014DELIVERYPOINTS");

            migrationBuilder.DropIndex(
                name: "IX_TA5014DELIVERYPOINTS_ID_ACTION",
                table: "TA5014DELIVERYPOINTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TA5012PARTICIPANTS",
                table: "TA5012PARTICIPANTS");

            migrationBuilder.DropColumn(
                name: "IdAction",
                table: "TA5150PROMOARTICLE");

            migrationBuilder.DropColumn(
                name: "CodProduct",
                table: "TA5150PROMOARTICLE");

            migrationBuilder.DropColumn(
                name: "LevProduct",
                table: "TA5150PROMOARTICLE");

            migrationBuilder.DropColumn(
                name: "CodDisplay",
                table: "TA5150PROMOARTICLE");

            migrationBuilder.RenameTable(
                name: "TA5150PROMOARTICLE",
                newName: "TA5150PROMOARTICLES");

            migrationBuilder.RenameTable(
                name: "TA5020PRODUCTS",
                newName: "TA502PRODUCTS");

            migrationBuilder.RenameTable(
                name: "TA5014DELIVERYPOINTS",
                newName: "TA501DELIVERYPOINTS");

            migrationBuilder.RenameTable(
                name: "TA5012PARTICIPANTS",
                newName: "TA8012PARTICIPANTS");

            migrationBuilder.RenameColumn(
                name: "DteEnd",
                table: "TB0042RELATIONS_CUST",
                newName: "DTEEND");

            migrationBuilder.RenameColumn(
                name: "DteStart",
                table: "TB0042RELATIONS_CUST",
                newName: "DTESTART");

            migrationBuilder.RenameColumn(
                name: "IdLevel",
                table: "TB0042RELATIONS_CUST",
                newName: "IDLEVEL");

            migrationBuilder.RenameColumn(
                name: "CodNode",
                table: "TB0042RELATIONS_CUST",
                newName: "CODNODE");

            migrationBuilder.RenameColumn(
                name: "CodDiv",
                table: "TB0042RELATIONS_CUST",
                newName: "CODDIV");

            migrationBuilder.RenameColumn(
                name: "CodParentNode",
                table: "TB0042RELATIONS_CUST",
                newName: "COOPAREANTNODE");

            migrationBuilder.RenameColumn(
                name: "CodHier",
                table: "TB0042RELATIONS_CUST",
                newName: "CODHER");

            migrationBuilder.RenameColumn(
                name: "FlgInclusion",
                table: "TA5026PRODUCTDETAILS",
                newName: "FLGINCLUSION");

            migrationBuilder.RenameColumn(
                name: "CodDiv",
                table: "TA5026PRODUCTDETAILS",
                newName: "CODDIV");

            migrationBuilder.RenameColumn(
                name: "CodDisplay",
                table: "TA5026PRODUCTDETAILS",
                newName: "CODDISPLAY");

            migrationBuilder.RenameColumn(
                name: "LevProduct",
                table: "TA5026PRODUCTDETAILS",
                newName: "LEVPRODUCT");

            migrationBuilder.RenameColumn(
                name: "CodProduct",
                table: "TA5026PRODUCTDETAILS",
                newName: "CODPRODUCT");

            migrationBuilder.RenameColumn(
                name: "IdAction",
                table: "TA5026PRODUCTDETAILS",
                newName: "IDACTION");

            migrationBuilder.RenameColumn(
                name: "CodNode",
                table: "TA5026PRODUCTDETAILS",
                newName: "CODNODEO");

            migrationBuilder.RenameColumn(
                name: "DTETOSHOST",
                table: "TA500PROMOACTION",
                newName: "DTEHOST");

            migrationBuilder.RenameColumn(
                name: "ID_ACTION",
                table: "TA500PROMOACTION",
                newName: "IDACTION");

            migrationBuilder.RenameColumn(
                name: "CODNODE_N",
                table: "TA5150PROMOARTICLES",
                newName: "FROMNODEFIN");

            migrationBuilder.RenameColumn(
                name: "CODNODE",
                table: "TA5150PROMOARTICLES",
                newName: "CODNODEO");

            migrationBuilder.RenameColumn(
                name: "QtyEstimated",
                table: "TA502PRODUCTS",
                newName: "QTYESTIMATED");

            migrationBuilder.RenameColumn(
                name: "CodMeasure",
                table: "TA502PRODUCTS",
                newName: "CODMEASURE");

            migrationBuilder.RenameColumn(
                name: "CodDiv",
                table: "TA502PRODUCTS",
                newName: "CODDIV");

            migrationBuilder.RenameColumn(
                name: "CodDisplay",
                table: "TA502PRODUCTS",
                newName: "CODDISPLAY");

            migrationBuilder.RenameColumn(
                name: "LevProduct",
                table: "TA502PRODUCTS",
                newName: "LEVPRODUCT");

            migrationBuilder.RenameColumn(
                name: "CodProduct",
                table: "TA502PRODUCTS",
                newName: "CODPRODUCT");

            migrationBuilder.RenameColumn(
                name: "IdAction",
                table: "TA502PRODUCTS",
                newName: "IDACTION");

            migrationBuilder.RenameColumn(
                name: "PerceDiscount2",
                table: "TA502PRODUCTS",
                newName: "PERCDISCOUNT2");

            migrationBuilder.RenameColumn(
                name: "PerceDiscount1",
                table: "TA502PRODUCTS",
                newName: "PERCDISCOUNT1");

            migrationBuilder.RenameColumn(
                name: "NumMeasure",
                table: "TA502PRODUCTS",
                newName: "NUMMEASUREA");

            migrationBuilder.RenameColumn(
                name: "IdLevel",
                table: "TA501DELIVERYPOINTS",
                newName: "IDLEVEL");

            migrationBuilder.RenameColumn(
                name: "DteStart",
                table: "TA501DELIVERYPOINTS",
                newName: "DTESTART");

            migrationBuilder.RenameColumn(
                name: "CodNode",
                table: "TA501DELIVERYPOINTS",
                newName: "CODNODE");

            migrationBuilder.RenameColumn(
                name: "CodDiv",
                table: "TA501DELIVERYPOINTS",
                newName: "CODDIV");

            migrationBuilder.RenameColumn(
                name: "CodHier",
                table: "TA501DELIVERYPOINTS",
                newName: "CODHER");

            migrationBuilder.RenameColumn(
                name: "ID_ACTION",
                table: "TA501DELIVERYPOINTS",
                newName: "IDACTION");

            migrationBuilder.RenameIndex(
                name: "IX_TA5014DELIVERYPOINTS_CodHier_CodDiv_CodNode_IdLevel_DteStart",
                table: "TA501DELIVERYPOINTS",
                newName: "IX_TA501DELIVERYPOINTS_CODHER_CODDIV_CODNODE_IDLEVEL_DTESTART");

            migrationBuilder.RenameColumn(
                name: "IdLevel",
                table: "TA8012PARTICIPANTS",
                newName: "IDLEVEL");

            migrationBuilder.RenameColumn(
                name: "DteStart",
                table: "TA8012PARTICIPANTS",
                newName: "DTESTART");

            migrationBuilder.RenameColumn(
                name: "CodNode",
                table: "TA8012PARTICIPANTS",
                newName: "CODNODE");

            migrationBuilder.RenameColumn(
                name: "CodDiv",
                table: "TA8012PARTICIPANTS",
                newName: "CODDIV");

            migrationBuilder.RenameColumn(
                name: "CodHier",
                table: "TA8012PARTICIPANTS",
                newName: "CODHER");

            migrationBuilder.RenameColumn(
                name: "ID_ACTION",
                table: "TA8012PARTICIPANTS",
                newName: "IDACTION");

            migrationBuilder.RenameIndex(
                name: "IX_TA5012PARTICIPANTS_CodHier_CodDiv_CodNode_IdLevel_DteStart",
                table: "TA8012PARTICIPANTS",
                newName: "IX_TA8012PARTICIPANTS_CODHER_CODDIV_CODNODE_IDLEVEL_DTESTART");

            migrationBuilder.AlterColumn<string>(
                name: "COOPAREANTNODE",
                table: "TB0042RELATIONS_CUST",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "CODDIV",
                table: "TA5118PROMOMEASUREFIELDS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "CODNODEO",
                table: "TA5026PRODUCTDETAILS",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CODDIV",
                table: "TA500PROMOACTION",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "FROMNODEFIN",
                table: "TA5150PROMOARTICLES",
                type: "bit",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "QTYESTIMATED",
                table: "TA502PRODUCTS",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "CODDIV",
                table: "TA502PRODUCTS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "PERCDISCOUNT2",
                table: "TA502PRODUCTS",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PERCDISCOUNT1",
                table: "TA502PRODUCTS",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NUMMEASUREA",
                table: "TA502PRODUCTS",
                type: "decimal(18,3)",
                precision: 18,
                scale: 3,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CODDELIVERYPOINT",
                table: "TA501DELIVERYPOINTS",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CODNODE",
                table: "TA8012PARTICIPANTS",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "CODDIV",
                table: "TA8012PARTICIPANTS",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "CODHER",
                table: "TA8012PARTICIPANTS",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TA5150PROMOARTICLES",
                table: "TA5150PROMOARTICLES",
                columns: new[] { "CODDIV", "CODNODEO" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TA502PRODUCTS",
                table: "TA502PRODUCTS",
                columns: new[] { "IDACTION", "CODPRODUCT", "LEVPRODUCT", "CODDISPLAY" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TA501DELIVERYPOINTS",
                table: "TA501DELIVERYPOINTS",
                columns: new[] { "IDACTION", "CODDELIVERYPOINT" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TA8012PARTICIPANTS",
                table: "TA8012PARTICIPANTS",
                columns: new[] { "IDACTION", "CODPARTICIPANT" });

            migrationBuilder.CreateIndex(
                name: "IX_TA5026PRODUCTDETAILS_CODDIV_CODNODEO",
                table: "TA5026PRODUCTDETAILS",
                columns: new[] { "CODDIV", "CODNODEO" });

            migrationBuilder.AddForeignKey(
                name: "FK_TA501DELIVERYPOINTS_TA500PROMOACTION_IDACTION",
                table: "TA501DELIVERYPOINTS",
                column: "IDACTION",
                principalTable: "TA500PROMOACTION",
                principalColumn: "IDACTION",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TA501DELIVERYPOINTS_TB0042RELATIONS_CUST_CODHER_CODDIV_CODNODE_IDLEVEL_DTESTART",
                table: "TA501DELIVERYPOINTS",
                columns: new[] { "CODHER", "CODDIV", "CODNODE", "IDLEVEL", "DTESTART" },
                principalTable: "TB0042RELATIONS_CUST",
                principalColumns: new[] { "CODHER", "CODDIV", "CODNODE", "IDLEVEL", "DTESTART" });

            migrationBuilder.AddForeignKey(
                name: "FK_TA5026PRODUCTDETAILS_TA502PRODUCTS_IDACTION_CODPRODUCT_LEVPRODUCT_CODDISPLAY",
                table: "TA5026PRODUCTDETAILS",
                columns: new[] { "IDACTION", "CODPRODUCT", "LEVPRODUCT", "CODDISPLAY" },
                principalTable: "TA502PRODUCTS",
                principalColumns: new[] { "IDACTION", "CODPRODUCT", "LEVPRODUCT", "CODDISPLAY" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TA5026PRODUCTDETAILS_TA5150PROMOARTICLES_CODDIV_CODNODEO",
                table: "TA5026PRODUCTDETAILS",
                columns: new[] { "CODDIV", "CODNODEO" },
                principalTable: "TA5150PROMOARTICLES",
                principalColumns: new[] { "CODDIV", "CODNODEO" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TA502PRODUCTS_TA500PROMOACTION_IDACTION",
                table: "TA502PRODUCTS",
                column: "IDACTION",
                principalTable: "TA500PROMOACTION",
                principalColumn: "IDACTION",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TA8012PARTICIPANTS_TA500PROMOACTION_IDACTION",
                table: "TA8012PARTICIPANTS",
                column: "IDACTION",
                principalTable: "TA500PROMOACTION",
                principalColumn: "IDACTION",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TA8012PARTICIPANTS_TB0042RELATIONS_CUST_CODHER_CODDIV_CODNODE_IDLEVEL_DTESTART",
                table: "TA8012PARTICIPANTS",
                columns: new[] { "CODHER", "CODDIV", "CODNODE", "IDLEVEL", "DTESTART" },
                principalTable: "TB0042RELATIONS_CUST",
                principalColumns: new[] { "CODHER", "CODDIV", "CODNODE", "IDLEVEL", "DTESTART" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TA501DELIVERYPOINTS_TA500PROMOACTION_IDACTION",
                table: "TA501DELIVERYPOINTS");

            migrationBuilder.DropForeignKey(
                name: "FK_TA501DELIVERYPOINTS_TB0042RELATIONS_CUST_CODHER_CODDIV_CODNODE_IDLEVEL_DTESTART",
                table: "TA501DELIVERYPOINTS");

            migrationBuilder.DropForeignKey(
                name: "FK_TA5026PRODUCTDETAILS_TA502PRODUCTS_IDACTION_CODPRODUCT_LEVPRODUCT_CODDISPLAY",
                table: "TA5026PRODUCTDETAILS");

            migrationBuilder.DropForeignKey(
                name: "FK_TA5026PRODUCTDETAILS_TA5150PROMOARTICLES_CODDIV_CODNODEO",
                table: "TA5026PRODUCTDETAILS");

            migrationBuilder.DropForeignKey(
                name: "FK_TA502PRODUCTS_TA500PROMOACTION_IDACTION",
                table: "TA502PRODUCTS");

            migrationBuilder.DropForeignKey(
                name: "FK_TA8012PARTICIPANTS_TA500PROMOACTION_IDACTION",
                table: "TA8012PARTICIPANTS");

            migrationBuilder.DropForeignKey(
                name: "FK_TA8012PARTICIPANTS_TB0042RELATIONS_CUST_CODHER_CODDIV_CODNODE_IDLEVEL_DTESTART",
                table: "TA8012PARTICIPANTS");

            migrationBuilder.DropIndex(
                name: "IX_TA5026PRODUCTDETAILS_CODDIV_CODNODEO",
                table: "TA5026PRODUCTDETAILS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TA8012PARTICIPANTS",
                table: "TA8012PARTICIPANTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TA5150PROMOARTICLES",
                table: "TA5150PROMOARTICLES");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TA502PRODUCTS",
                table: "TA502PRODUCTS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TA501DELIVERYPOINTS",
                table: "TA501DELIVERYPOINTS");

            migrationBuilder.RenameTable(
                name: "TA8012PARTICIPANTS",
                newName: "TA5012PARTICIPANTS");

            migrationBuilder.RenameTable(
                name: "TA5150PROMOARTICLES",
                newName: "TA5150PROMOARTICLE");

            migrationBuilder.RenameTable(
                name: "TA502PRODUCTS",
                newName: "TA5020PRODUCTS");

            migrationBuilder.RenameTable(
                name: "TA501DELIVERYPOINTS",
                newName: "TA5014DELIVERYPOINTS");

            migrationBuilder.RenameColumn(
                name: "DTEEND",
                table: "TB0042RELATIONS_CUST",
                newName: "DteEnd");

            migrationBuilder.RenameColumn(
                name: "DTESTART",
                table: "TB0042RELATIONS_CUST",
                newName: "DteStart");

            migrationBuilder.RenameColumn(
                name: "IDLEVEL",
                table: "TB0042RELATIONS_CUST",
                newName: "IdLevel");

            migrationBuilder.RenameColumn(
                name: "CODNODE",
                table: "TB0042RELATIONS_CUST",
                newName: "CodNode");

            migrationBuilder.RenameColumn(
                name: "CODDIV",
                table: "TB0042RELATIONS_CUST",
                newName: "CodDiv");

            migrationBuilder.RenameColumn(
                name: "COOPAREANTNODE",
                table: "TB0042RELATIONS_CUST",
                newName: "CodParentNode");

            migrationBuilder.RenameColumn(
                name: "CODHER",
                table: "TB0042RELATIONS_CUST",
                newName: "CodHier");

            migrationBuilder.RenameColumn(
                name: "FLGINCLUSION",
                table: "TA5026PRODUCTDETAILS",
                newName: "FlgInclusion");

            migrationBuilder.RenameColumn(
                name: "CODDIV",
                table: "TA5026PRODUCTDETAILS",
                newName: "CodDiv");

            migrationBuilder.RenameColumn(
                name: "CODDISPLAY",
                table: "TA5026PRODUCTDETAILS",
                newName: "CodDisplay");

            migrationBuilder.RenameColumn(
                name: "LEVPRODUCT",
                table: "TA5026PRODUCTDETAILS",
                newName: "LevProduct");

            migrationBuilder.RenameColumn(
                name: "CODPRODUCT",
                table: "TA5026PRODUCTDETAILS",
                newName: "CodProduct");

            migrationBuilder.RenameColumn(
                name: "IDACTION",
                table: "TA5026PRODUCTDETAILS",
                newName: "IdAction");

            migrationBuilder.RenameColumn(
                name: "CODNODEO",
                table: "TA5026PRODUCTDETAILS",
                newName: "CodNode");

            migrationBuilder.RenameColumn(
                name: "DTEHOST",
                table: "TA500PROMOACTION",
                newName: "DTETOSHOST");

            migrationBuilder.RenameColumn(
                name: "IDACTION",
                table: "TA500PROMOACTION",
                newName: "ID_ACTION");

            migrationBuilder.RenameColumn(
                name: "IDLEVEL",
                table: "TA5012PARTICIPANTS",
                newName: "IdLevel");

            migrationBuilder.RenameColumn(
                name: "DTESTART",
                table: "TA5012PARTICIPANTS",
                newName: "DteStart");

            migrationBuilder.RenameColumn(
                name: "CODNODE",
                table: "TA5012PARTICIPANTS",
                newName: "CodNode");

            migrationBuilder.RenameColumn(
                name: "CODDIV",
                table: "TA5012PARTICIPANTS",
                newName: "CodDiv");

            migrationBuilder.RenameColumn(
                name: "CODHER",
                table: "TA5012PARTICIPANTS",
                newName: "CodHier");

            migrationBuilder.RenameColumn(
                name: "IDACTION",
                table: "TA5012PARTICIPANTS",
                newName: "ID_ACTION");

            migrationBuilder.RenameIndex(
                name: "IX_TA8012PARTICIPANTS_CODHER_CODDIV_CODNODE_IDLEVEL_DTESTART",
                table: "TA5012PARTICIPANTS",
                newName: "IX_TA5012PARTICIPANTS_CodHier_CodDiv_CodNode_IdLevel_DteStart");

            migrationBuilder.RenameColumn(
                name: "FROMNODEFIN",
                table: "TA5150PROMOARTICLE",
                newName: "CODNODE_N");

            migrationBuilder.RenameColumn(
                name: "CODNODEO",
                table: "TA5150PROMOARTICLE",
                newName: "CODNODE");

            migrationBuilder.RenameColumn(
                name: "QTYESTIMATED",
                table: "TA5020PRODUCTS",
                newName: "QtyEstimated");

            migrationBuilder.RenameColumn(
                name: "CODMEASURE",
                table: "TA5020PRODUCTS",
                newName: "CodMeasure");

            migrationBuilder.RenameColumn(
                name: "CODDIV",
                table: "TA5020PRODUCTS",
                newName: "CodDiv");

            migrationBuilder.RenameColumn(
                name: "CODDISPLAY",
                table: "TA5020PRODUCTS",
                newName: "CodDisplay");

            migrationBuilder.RenameColumn(
                name: "LEVPRODUCT",
                table: "TA5020PRODUCTS",
                newName: "LevProduct");

            migrationBuilder.RenameColumn(
                name: "CODPRODUCT",
                table: "TA5020PRODUCTS",
                newName: "CodProduct");

            migrationBuilder.RenameColumn(
                name: "IDACTION",
                table: "TA5020PRODUCTS",
                newName: "IdAction");

            migrationBuilder.RenameColumn(
                name: "PERCDISCOUNT2",
                table: "TA5020PRODUCTS",
                newName: "PerceDiscount2");

            migrationBuilder.RenameColumn(
                name: "PERCDISCOUNT1",
                table: "TA5020PRODUCTS",
                newName: "PerceDiscount1");

            migrationBuilder.RenameColumn(
                name: "NUMMEASUREA",
                table: "TA5020PRODUCTS",
                newName: "NumMeasure");

            migrationBuilder.RenameColumn(
                name: "IDLEVEL",
                table: "TA5014DELIVERYPOINTS",
                newName: "IdLevel");

            migrationBuilder.RenameColumn(
                name: "DTESTART",
                table: "TA5014DELIVERYPOINTS",
                newName: "DteStart");

            migrationBuilder.RenameColumn(
                name: "CODNODE",
                table: "TA5014DELIVERYPOINTS",
                newName: "CodNode");

            migrationBuilder.RenameColumn(
                name: "CODDIV",
                table: "TA5014DELIVERYPOINTS",
                newName: "CodDiv");

            migrationBuilder.RenameColumn(
                name: "CODHER",
                table: "TA5014DELIVERYPOINTS",
                newName: "CodHier");

            migrationBuilder.RenameColumn(
                name: "IDACTION",
                table: "TA5014DELIVERYPOINTS",
                newName: "ID_ACTION");

            migrationBuilder.RenameIndex(
                name: "IX_TA501DELIVERYPOINTS_CODHER_CODDIV_CODNODE_IDLEVEL_DTESTART",
                table: "TA5014DELIVERYPOINTS",
                newName: "IX_TA5014DELIVERYPOINTS_CodHier_CodDiv_CodNode_IdLevel_DteStart");

            migrationBuilder.AlterColumn<string>(
                name: "CodParentNode",
                table: "TB0042RELATIONS_CUST",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CODDIV",
                table: "TA5118PROMOMEASUREFIELDS",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CodNode",
                table: "TA5026PRODUCTDETAILS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "CODDIV",
                table: "TA500PROMOACTION",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodNode",
                table: "TA5012PARTICIPANTS",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodDiv",
                table: "TA5012PARTICIPANTS",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodHier",
                table: "TA5012PARTICIPANTS",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CODNODE_N",
                table: "TA5150PROMOARTICLE",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdAction",
                table: "TA5150PROMOARTICLE",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CodProduct",
                table: "TA5150PROMOARTICLE",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LevProduct",
                table: "TA5150PROMOARTICLE",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CodDisplay",
                table: "TA5150PROMOARTICLE",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "QtyEstimated",
                table: "TA5020PRODUCTS",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3);

            migrationBuilder.AlterColumn<string>(
                name: "CodDiv",
                table: "TA5020PRODUCTS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PerceDiscount2",
                table: "TA5020PRODUCTS",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PerceDiscount1",
                table: "TA5020PRODUCTS",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "NumMeasure",
                table: "TA5020PRODUCTS",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)",
                oldPrecision: 18,
                oldScale: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CODDELIVERYPOINT",
                table: "TA5014DELIVERYPOINTS",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TA5012PARTICIPANTS",
                table: "TA5012PARTICIPANTS",
                columns: new[] { "ID_ACTION", "CODPARTICIPANT" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TA5150PROMOARTICLE",
                table: "TA5150PROMOARTICLE",
                columns: new[] { "IdAction", "CodProduct", "LevProduct", "CodDisplay", "CODDIV", "CODNODE" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TA5020PRODUCTS",
                table: "TA5020PRODUCTS",
                columns: new[] { "IdAction", "CodProduct", "LevProduct", "CodDisplay" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TA5014DELIVERYPOINTS",
                table: "TA5014DELIVERYPOINTS",
                columns: new[] { "ID_ACTION", "CODDELIVERYPOINT" });

            migrationBuilder.CreateIndex(
                name: "IX_TA5150PROMOARTICLE_IdAction_CodProduct_LevProduct_CodDisplay_CODNODE_CODDIV",
                table: "TA5150PROMOARTICLE",
                columns: new[] { "IdAction", "CodProduct", "LevProduct", "CodDisplay", "CODNODE", "CODDIV" });

            migrationBuilder.CreateIndex(
                name: "IX_TA5014DELIVERYPOINTS_ID_ACTION",
                table: "TA5014DELIVERYPOINTS",
                column: "ID_ACTION");

            migrationBuilder.AddForeignKey(
                name: "FK_TA5012PARTICIPANTS_TA500PROMOACTION_ID_ACTION",
                table: "TA5012PARTICIPANTS",
                column: "ID_ACTION",
                principalTable: "TA500PROMOACTION",
                principalColumn: "ID_ACTION",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TA5012PARTICIPANTS_TB0042RELATIONS_CUST_CodHier_CodDiv_CodNode_IdLevel_DteStart",
                table: "TA5012PARTICIPANTS",
                columns: new[] { "CodHier", "CodDiv", "CodNode", "IdLevel", "DteStart" },
                principalTable: "TB0042RELATIONS_CUST",
                principalColumns: new[] { "CodHier", "CodDiv", "CodNode", "IdLevel", "DteStart" });

            migrationBuilder.AddForeignKey(
                name: "FK_TA5014DELIVERYPOINTS_TA500PROMOACTION_ID_ACTION",
                table: "TA5014DELIVERYPOINTS",
                column: "ID_ACTION",
                principalTable: "TA500PROMOACTION",
                principalColumn: "ID_ACTION",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TA5014DELIVERYPOINTS_TB0042RELATIONS_CUST_CodHier_CodDiv_CodNode_IdLevel_DteStart",
                table: "TA5014DELIVERYPOINTS",
                columns: new[] { "CodHier", "CodDiv", "CodNode", "IdLevel", "DteStart" },
                principalTable: "TB0042RELATIONS_CUST",
                principalColumns: new[] { "CodHier", "CodDiv", "CodNode", "IdLevel", "DteStart" });

            migrationBuilder.AddForeignKey(
                name: "FK_TA5020PRODUCTS_TA500PROMOACTION_IdAction",
                table: "TA5020PRODUCTS",
                column: "IdAction",
                principalTable: "TA500PROMOACTION",
                principalColumn: "ID_ACTION",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TA5026PRODUCTDETAILS_TA5020PRODUCTS_IdAction_CodProduct_LevProduct_CodDisplay",
                table: "TA5026PRODUCTDETAILS",
                columns: new[] { "IdAction", "CodProduct", "LevProduct", "CodDisplay" },
                principalTable: "TA5020PRODUCTS",
                principalColumns: new[] { "IdAction", "CodProduct", "LevProduct", "CodDisplay" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TA5150PROMOARTICLE_TA5026PRODUCTDETAILS_IdAction_CodProduct_LevProduct_CodDisplay_CODNODE_CODDIV",
                table: "TA5150PROMOARTICLE",
                columns: new[] { "IdAction", "CodProduct", "LevProduct", "CodDisplay", "CODNODE", "CODDIV" },
                principalTable: "TA5026PRODUCTDETAILS",
                principalColumns: new[] { "IdAction", "CodProduct", "LevProduct", "CodDisplay", "CodNode", "CodDiv" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}

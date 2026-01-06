using System;
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
                    LEVPARTICIPANTS = table.Column<int>(type: "int", nullable: true),
                    ContractorCodHier = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    ContractorCodDiv = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    ContractorCodNode = table.Column<string>(type: "nvarchar(30)", nullable: true),
                    ContractorIdLevel = table.Column<int>(type: "int", nullable: true),
                    ContractorDteStart = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TA500PROMOACTION", x => x.ID_ACTION);
                    table.ForeignKey(
                        name: "FK_TA500PROMOACTION_TB0042RELATIONS_CUST_ContractorCodHier_ContractorCodDiv_ContractorCodNode_ContractorIdLevel_ContractorDteSt~",
                        columns: x => new { x.ContractorCodHier, x.ContractorCodDiv, x.ContractorCodNode, x.ContractorIdLevel, x.ContractorDteStart },
                        principalTable: "TB0042RELATIONS_CUST",
                        principalColumns: new[] { "CodHier", "CodDiv", "CodNode", "IdLevel", "DteStart" });
                });

            migrationBuilder.CreateTable(
                name: "TA5012PARTICIPANTS",
                columns: table => new
                {
                    ID_ACTION = table.Column<int>(type: "int", nullable: false),
                    CODPARTICIPANT = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FLGINCLUSION = table.Column<bool>(type: "bit", nullable: false),
                    CodHier = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CodDiv = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CodNode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IdLevel = table.Column<int>(type: "int", nullable: false),
                    DteStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TA5012PARTICIPANTS", x => new { x.ID_ACTION, x.CODPARTICIPANT });
                    table.ForeignKey(
                        name: "FK_TA5012PARTICIPANTS_TA500PROMOACTION_ID_ACTION",
                        column: x => x.ID_ACTION,
                        principalTable: "TA500PROMOACTION",
                        principalColumn: "ID_ACTION",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TA5012PARTICIPANTS_TB0042RELATIONS_CUST_CodHier_CodDiv_CodNode_IdLevel_DteStart",
                        columns: x => new { x.CodHier, x.CodDiv, x.CodNode, x.IdLevel, x.DteStart },
                        principalTable: "TB0042RELATIONS_CUST",
                        principalColumns: new[] { "CodHier", "CodDiv", "CodNode", "IdLevel", "DteStart" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TA5014DELIVERYPOINTS",
                columns: table => new
                {
                    ID_ACTION = table.Column<int>(type: "int", nullable: false),
                    CODDELIVERYPOINT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FLGINCLUSION = table.Column<bool>(type: "bit", nullable: false),
                    CodHier = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CodDiv = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CodNode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IdLevel = table.Column<int>(type: "int", nullable: false),
                    DteStart = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TA5014DELIVERYPOINTS", x => new { x.ID_ACTION, x.CODDELIVERYPOINT });
                    table.ForeignKey(
                        name: "FK_TA5014DELIVERYPOINTS_TA500PROMOACTION_ID_ACTION",
                        column: x => x.ID_ACTION,
                        principalTable: "TA500PROMOACTION",
                        principalColumn: "ID_ACTION",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TA5014DELIVERYPOINTS_TB0042RELATIONS_CUST_CodHier_CodDiv_CodNode_IdLevel_DteStart",
                        columns: x => new { x.CodHier, x.CodDiv, x.CodNode, x.IdLevel, x.DteStart },
                        principalTable: "TB0042RELATIONS_CUST",
                        principalColumns: new[] { "CodHier", "CodDiv", "CodNode", "IdLevel", "DteStart" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TA5020PRODUCTS",
                columns: table => new
                {
                    IdAction = table.Column<int>(type: "int", nullable: false),
                    CodProduct = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LevProduct = table.Column<int>(type: "int", nullable: false),
                    CodDisplay = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodDiv = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    QtyEstimated = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PerceDiscount1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PerceDiscount2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NumMeasure = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CodMeasure = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TA5020PRODUCTS", x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay });
                    table.ForeignKey(
                        name: "FK_TA5020PRODUCTS_TA500PROMOACTION_IdAction",
                        column: x => x.IdAction,
                        principalTable: "TA500PROMOACTION",
                        principalColumn: "ID_ACTION",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TA5026PRODUCTDETAILS",
                columns: table => new
                {
                    IdAction = table.Column<int>(type: "int", nullable: false),
                    CodProduct = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LevProduct = table.Column<int>(type: "int", nullable: false),
                    CodDisplay = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodNode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodDiv = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FlgInclusion = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TA5026PRODUCTDETAILS", x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay, x.CodNode, x.CodDiv });
                    table.ForeignKey(
                        name: "FK_TA5026PRODUCTDETAILS_TA5020PRODUCTS_IdAction_CodProduct_LevProduct_CodDisplay",
                        columns: x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay },
                        principalTable: "TA5020PRODUCTS",
                        principalColumns: new[] { "IdAction", "CodProduct", "LevProduct", "CodDisplay" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TA5118PROMOMEASUREFIELDS",
                columns: table => new
                {
                    IdAction = table.Column<int>(type: "int", nullable: false),
                    CodProduct = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LevProduct = table.Column<int>(type: "int", nullable: false),
                    CodDisplay = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CODDIV = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CODMEASURE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FIELDNAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FORMULA = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TA5118PROMOMEASUREFIELDS", x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay, x.CODDIV, x.CODMEASURE, x.FIELDNAME });
                    table.ForeignKey(
                        name: "FK_TA5118PROMOMEASUREFIELDS_TA5020PRODUCTS_IdAction_CodProduct_LevProduct_CodDisplay",
                        columns: x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay },
                        principalTable: "TA5020PRODUCTS",
                        principalColumns: new[] { "IdAction", "CodProduct", "LevProduct", "CodDisplay" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TA5150PROMOARTICLE",
                columns: table => new
                {
                    IdAction = table.Column<int>(type: "int", nullable: false),
                    CodProduct = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LevProduct = table.Column<int>(type: "int", nullable: false),
                    CodDisplay = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CODDIV = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CODNODE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CODNODE1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CODNODE2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CODNODE_N = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TA5150PROMOARTICLE", x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay, x.CODDIV, x.CODNODE });
                    table.ForeignKey(
                        name: "FK_TA5150PROMOARTICLE_TA5026PRODUCTDETAILS_IdAction_CodProduct_LevProduct_CodDisplay_CODNODE_CODDIV",
                        columns: x => new { x.IdAction, x.CodProduct, x.LevProduct, x.CodDisplay, x.CODNODE, x.CODDIV },
                        principalTable: "TA5026PRODUCTDETAILS",
                        principalColumns: new[] { "IdAction", "CodProduct", "LevProduct", "CodDisplay", "CodNode", "CodDiv" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TA500PROMOACTION_ContractorCodHier_ContractorCodDiv_ContractorCodNode_ContractorIdLevel_ContractorDteStart",
                table: "TA500PROMOACTION",
                columns: new[] { "ContractorCodHier", "ContractorCodDiv", "ContractorCodNode", "ContractorIdLevel", "ContractorDteStart" });

            migrationBuilder.CreateIndex(
                name: "IX_TA5012PARTICIPANTS_CodHier_CodDiv_CodNode_IdLevel_DteStart",
                table: "TA5012PARTICIPANTS",
                columns: new[] { "CodHier", "CodDiv", "CodNode", "IdLevel", "DteStart" });

            migrationBuilder.CreateIndex(
                name: "IX_TA5014DELIVERYPOINTS_CodHier_CodDiv_CodNode_IdLevel_DteStart",
                table: "TA5014DELIVERYPOINTS",
                columns: new[] { "CodHier", "CodDiv", "CodNode", "IdLevel", "DteStart" });

            migrationBuilder.CreateIndex(
                name: "IX_TA5014DELIVERYPOINTS_ID_ACTION",
                table: "TA5014DELIVERYPOINTS",
                column: "ID_ACTION");

            migrationBuilder.CreateIndex(
                name: "IX_TA5150PROMOARTICLE_IdAction_CodProduct_LevProduct_CodDisplay_CODNODE_CODDIV",
                table: "TA5150PROMOARTICLE",
                columns: new[] { "IdAction", "CodProduct", "LevProduct", "CodDisplay", "CODNODE", "CODDIV" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TA5012PARTICIPANTS");

            migrationBuilder.DropTable(
                name: "TA5014DELIVERYPOINTS");

            migrationBuilder.DropTable(
                name: "TA5118PROMOMEASUREFIELDS");

            migrationBuilder.DropTable(
                name: "TA5150PROMOARTICLE");

            migrationBuilder.DropTable(
                name: "TA5026PRODUCTDETAILS");

            migrationBuilder.DropTable(
                name: "TA5020PRODUCTS");

            migrationBuilder.DropTable(
                name: "TA500PROMOACTION");

            migrationBuilder.DropTable(
                name: "TB0042RELATIONS_CUST");
        }
    }
}

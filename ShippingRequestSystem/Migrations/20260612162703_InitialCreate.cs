using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "prcPurchaseShippingRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RequestorEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BusinessUnit = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    BuyerBusinessName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    BuyerAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BuyerTaxId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RecipientBusinessName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    RecipientAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RecipientTaxId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RecipientAttentionTo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RecipientPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RecipientEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ShippingWay = table.Column<int>(type: "int", nullable: false),
                    ServiceType = table.Column<int>(type: "int", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ShipmentPurpose = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    InvoiceType = table.Column<int>(type: "int", nullable: false),
                    TransferReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Incoterm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IgiPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HasCOO = table.Column<bool>(type: "bit", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OperationType = table.Column<int>(type: "int", nullable: false),
                    IsStackable = table.Column<bool>(type: "bit", nullable: false),
                    PaymentResponsibility = table.Column<int>(type: "int", nullable: false),
                    ParcelAccount = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HasInsurance = table.Column<bool>(type: "bit", nullable: false),
                    RequestorObservations = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MaterialBajaFolio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LogisticsStaffObservations = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ApproximateCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserManagerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserManagerEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prcPurchaseShippingRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "prcPurchaseApprovalStep",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingRequestId = table.Column<int>(type: "int", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApproverName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ApproverEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prcPurchaseApprovalStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prcPurchaseApprovalStep_prcPurchaseShippingRequest_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "prcPurchaseShippingRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prcPurchasePremiumFreightInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingRequestId = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    Transport = table.Column<int>(type: "int", nullable: false),
                    Payment = table.Column<int>(type: "int", nullable: false),
                    PickUpName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PickUpAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PickUpContact = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DeliveryName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeliveryContact = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CauseOfShipment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Countermeasures = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Forwarder = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prcPurchasePremiumFreightInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prcPurchasePremiumFreightInfo_prcPurchaseShippingRequest_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "prcPurchaseShippingRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prcPurchaseShippingPackage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingRequestId = table.Column<int>(type: "int", nullable: false),
                    PackageType = table.Column<int>(type: "int", nullable: false),
                    ParentPalletId = table.Column<int>(type: "int", nullable: true),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prcPurchaseShippingPackage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prcPurchaseShippingPackage_prcPurchaseShippingPackage_ParentPalletId",
                        column: x => x.ParentPalletId,
                        principalTable: "prcPurchaseShippingPackage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_prcPurchaseShippingPackage_prcPurchaseShippingRequest_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "prcPurchaseShippingRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "prcPurchaseShippingItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingPackageId = table.Column<int>(type: "int", nullable: false),
                    PartNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Class = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UOM = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NetWeight = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    GrossWeight = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Width = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prcPurchaseShippingItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prcPurchaseShippingItem_prcPurchaseShippingPackage_ShippingPackageId",
                        column: x => x.ShippingPackageId,
                        principalTable: "prcPurchaseShippingPackage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_prcPurchaseApprovalStep_ShippingRequestId",
                table: "prcPurchaseApprovalStep",
                column: "ShippingRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_prcPurchasePremiumFreightInfo_ShippingRequestId",
                table: "prcPurchasePremiumFreightInfo",
                column: "ShippingRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prcPurchaseShippingItem_ShippingPackageId",
                table: "prcPurchaseShippingItem",
                column: "ShippingPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_prcPurchaseShippingPackage_ParentPalletId",
                table: "prcPurchaseShippingPackage",
                column: "ParentPalletId");

            migrationBuilder.CreateIndex(
                name: "IX_prcPurchaseShippingPackage_ShippingRequestId",
                table: "prcPurchaseShippingPackage",
                column: "ShippingRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prcPurchaseApprovalStep");

            migrationBuilder.DropTable(
                name: "prcPurchasePremiumFreightInfo");

            migrationBuilder.DropTable(
                name: "prcPurchaseShippingItem");

            migrationBuilder.DropTable(
                name: "prcPurchaseShippingPackage");

            migrationBuilder.DropTable(
                name: "prcPurchaseShippingRequest");
        }
    }
}

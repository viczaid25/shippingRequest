using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddFleteEvidenciaCatalogoBorrador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransferReason",
                table: "prcPurchaseShippingRequest");

            migrationBuilder.AddColumn<int>(
                name: "Carrier",
                table: "prcPurchaseShippingRequest",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DraftStep",
                table: "prcPurchaseShippingRequest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EvidenceDimensionsPhotoPath",
                table: "prcPurchaseShippingRequest",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenceFinalPackagingPhotoPath",
                table: "prcPurchaseShippingRequest",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenceGrossWeightPhotoPath",
                table: "prcPurchaseShippingRequest",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenceNetWeightPhotoPath",
                table: "prcPurchaseShippingRequest",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresInternationalFreight",
                table: "prcPurchaseShippingRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresLocalDestinationFreight",
                table: "prcPurchaseShippingRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresLocalOriginFreight",
                table: "prcPurchaseShippingRequest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "GrossWeight",
                table: "prcPurchaseShippingPackage",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                table: "prcPurchaseShippingPackage",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "prcPurchaseShippingPackage",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                table: "prcPurchaseShippingPackage",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "prcPurchasePartCatalog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Class = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prcPurchasePartCatalog", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_prcPurchasePartCatalog_PartNumber",
                table: "prcPurchasePartCatalog",
                column: "PartNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prcPurchasePartCatalog");

            migrationBuilder.DropColumn(
                name: "Carrier",
                table: "prcPurchaseShippingRequest");

            migrationBuilder.DropColumn(
                name: "DraftStep",
                table: "prcPurchaseShippingRequest");

            migrationBuilder.DropColumn(
                name: "EvidenceDimensionsPhotoPath",
                table: "prcPurchaseShippingRequest");

            migrationBuilder.DropColumn(
                name: "EvidenceFinalPackagingPhotoPath",
                table: "prcPurchaseShippingRequest");

            migrationBuilder.DropColumn(
                name: "EvidenceGrossWeightPhotoPath",
                table: "prcPurchaseShippingRequest");

            migrationBuilder.DropColumn(
                name: "EvidenceNetWeightPhotoPath",
                table: "prcPurchaseShippingRequest");

            migrationBuilder.DropColumn(
                name: "RequiresInternationalFreight",
                table: "prcPurchaseShippingRequest");

            migrationBuilder.DropColumn(
                name: "RequiresLocalDestinationFreight",
                table: "prcPurchaseShippingRequest");

            migrationBuilder.DropColumn(
                name: "RequiresLocalOriginFreight",
                table: "prcPurchaseShippingRequest");

            migrationBuilder.DropColumn(
                name: "GrossWeight",
                table: "prcPurchaseShippingPackage");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "prcPurchaseShippingPackage");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "prcPurchaseShippingPackage");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "prcPurchaseShippingPackage");

            migrationBuilder.AddColumn<string>(
                name: "TransferReason",
                table: "prcPurchaseShippingRequest",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}

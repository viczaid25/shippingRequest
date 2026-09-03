using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class RemoveItemGrossWeightDims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrossWeight",
                table: "prcPurchaseShippingItem");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "prcPurchaseShippingItem");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "prcPurchaseShippingItem");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "prcPurchaseShippingItem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "GrossWeight",
                table: "prcPurchaseShippingItem",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                table: "prcPurchaseShippingItem",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "prcPurchaseShippingItem",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                table: "prcPurchaseShippingItem",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}

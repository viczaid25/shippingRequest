using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddNationalCategoryAndCooDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CooDocumentPath",
                table: "prcPurchaseShippingRequest",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NationalCategory",
                table: "prcPurchaseShippingRequest",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CooDocumentPath",
                table: "prcPurchaseShippingRequest");

            migrationBuilder.DropColumn(
                name: "NationalCategory",
                table: "prcPurchaseShippingRequest");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddQuotesAndStaffFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "IgiPercentageStaff",
                table: "prcPurchaseShippingRequest",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "IgiPercentageStaff",
                table: "prcPurchaseShippingRequest",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldNullable: true);
        }
    }
}

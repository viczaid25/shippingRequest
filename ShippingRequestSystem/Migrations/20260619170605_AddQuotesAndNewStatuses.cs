using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddQuotesAndNewStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "IgiPercentage",
                table: "prcPurchaseShippingRequest",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "IgiPercentageStaff",
                table: "prcPurchaseShippingRequest",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelectedQuoteId",
                table: "prcPurchaseShippingRequest",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "prcPurchaseShippingQuote",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShippingRequestId = table.Column<int>(type: "int", nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsSelected = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prcPurchaseShippingQuote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prcPurchaseShippingQuote_prcPurchaseShippingRequest_ShippingRequestId",
                        column: x => x.ShippingRequestId,
                        principalTable: "prcPurchaseShippingRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_prcPurchaseShippingQuote_ShippingRequestId",
                table: "prcPurchaseShippingQuote",
                column: "ShippingRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prcPurchaseShippingQuote");

            migrationBuilder.DropColumn(
                name: "IgiPercentageStaff",
                table: "prcPurchaseShippingRequest");

            migrationBuilder.DropColumn(
                name: "SelectedQuoteId",
                table: "prcPurchaseShippingRequest");

            migrationBuilder.AlterColumn<decimal>(
                name: "IgiPercentage",
                table: "prcPurchaseShippingRequest",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);
        }
    }
}

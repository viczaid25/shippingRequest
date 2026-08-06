using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddQuoteFilePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileUrl",
                table: "prcPurchaseShippingQuote",
                newName: "FilePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "prcPurchaseShippingQuote",
                newName: "FileUrl");
        }
    }
}

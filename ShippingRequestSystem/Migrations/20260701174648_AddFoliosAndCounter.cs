using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddFoliosAndCounter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Folio",
                table: "prcPurchaseShippingRequest",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Folio",
                table: "prcPurchasePremiumFreightInfo",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "prcPurchaseFolioCounter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prefix = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    LastNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prcPurchaseFolioCounter", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prcPurchaseFolioCounter");

            migrationBuilder.DropColumn(
                name: "Folio",
                table: "prcPurchaseShippingRequest");

            migrationBuilder.DropColumn(
                name: "Folio",
                table: "prcPurchasePremiumFreightInfo");
        }
    }
}

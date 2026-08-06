using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionsAndAdmins2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PcLoginId",
                table: "prcPurchaseSystemAdmin",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PcLoginId",
                table: "prcPurchaseApprovalRolePermission",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PcLoginId",
                table: "prcPurchaseSystemAdmin");

            migrationBuilder.DropColumn(
                name: "PcLoginId",
                table: "prcPurchaseApprovalRolePermission");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionsAndAdmins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "prcPurchaseApprovalRolePermission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MeaxUserId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UserDepartment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserPosition = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prcPurchaseApprovalRolePermission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "prcPurchaseSystemAdmin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeaxUserId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UserDepartment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prcPurchaseSystemAdmin", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_prcPurchaseApprovalRolePermission_RoleName_MeaxUserId",
                table: "prcPurchaseApprovalRolePermission",
                columns: new[] { "RoleName", "MeaxUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_prcPurchaseSystemAdmin_MeaxUserId",
                table: "prcPurchaseSystemAdmin",
                column: "MeaxUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prcPurchaseApprovalRolePermission");

            migrationBuilder.DropTable(
                name: "prcPurchaseSystemAdmin");
        }
    }
}

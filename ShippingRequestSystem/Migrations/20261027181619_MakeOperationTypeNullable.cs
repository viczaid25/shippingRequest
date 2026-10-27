using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class MakeOperationTypeNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OperationType",
                table: "prcPurchaseShippingRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // Las solicitudes que aún esperan a Trade Staff (Borrador / PendingTradeStaffQuotes)
            // no tienen Tipo de Operación real: el A1 guardado era solo el valor por defecto.
            migrationBuilder.Sql(
                "UPDATE prcPurchaseShippingRequest SET OperationType = NULL WHERE Status IN (0, 1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OperationType",
                table: "prcPurchaseShippingRequest",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}

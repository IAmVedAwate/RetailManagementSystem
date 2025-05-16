using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddingWarehouseIdForMsgs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "RetailMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RetailMessages_WarehouseId",
                table: "RetailMessages",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_RetailMessages_Warehouses_WarehouseId",
                table: "RetailMessages",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RetailMessages_Warehouses_WarehouseId",
                table: "RetailMessages");

            migrationBuilder.DropIndex(
                name: "IX_RetailMessages_WarehouseId",
                table: "RetailMessages");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "RetailMessages");
        }
    }
}

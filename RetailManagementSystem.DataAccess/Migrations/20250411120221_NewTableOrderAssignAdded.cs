using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NewTableOrderAssignAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderAssigned",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryUserId = table.Column<int>(type: "int", nullable: false),
                    DeliveryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderAssigned", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderAssigned_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderAssigned_DeliveryUsers_DeliveryUserId",
                        column: x => x.DeliveryUserId,
                        principalTable: "DeliveryUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssigned_DeliveryId",
                table: "OrderAssigned",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderAssigned_DeliveryUserId",
                table: "OrderAssigned",
                column: "DeliveryUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderAssigned");
        }
    }
}

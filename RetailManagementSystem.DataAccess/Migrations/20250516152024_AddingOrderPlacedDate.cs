using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddingOrderPlacedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderPlacedDate",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderPlacedDate",
                table: "Deliveries");
        }
    }
}

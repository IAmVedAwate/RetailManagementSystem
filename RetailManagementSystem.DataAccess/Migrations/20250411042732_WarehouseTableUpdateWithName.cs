﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailManagementSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class WarehouseTableUpdateWithName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<string>(
                name: "WarehouseName",
                table: "Warehouses",
                type: "nvarchar(max)",
                nullable: true);

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "WarehouseName",
                table: "Warehouses");

            
        }
    }
}

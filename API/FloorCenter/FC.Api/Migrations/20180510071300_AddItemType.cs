using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddItemType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Vendor",
                table: "Warehouses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "Sizes",
                nullable: true);


            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "Items",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Vendor",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "Items");

        }
    }
}

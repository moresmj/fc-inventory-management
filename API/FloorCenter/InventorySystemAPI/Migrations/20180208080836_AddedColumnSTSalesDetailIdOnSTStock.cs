using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class AddedColumnSTSalesDetailIdOnSTStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "STSalesDetailId",
                table: "STStock",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Item_SizeId",
                table: "Item",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_Size_SizeId",
                table: "Item",
                column: "SizeId",
                principalTable: "Size",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_Size_SizeId",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_SizeId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "STSalesDetailId",
                table: "STStock");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class AddedDeliveryStatusColumnOnSTInventoryDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryStatus",
                table: "STInventoryDetail",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_STDelivery_STInventoryId",
                table: "STDelivery",
                column: "STInventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_STDelivery_STInventory_STInventoryId",
                table: "STDelivery",
                column: "STInventoryId",
                principalTable: "STInventory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STDelivery_STInventory_STInventoryId",
                table: "STDelivery");

            migrationBuilder.DropIndex(
                name: "IX_STDelivery_STInventoryId",
                table: "STDelivery");

            migrationBuilder.DropColumn(
                name: "DeliveryStatus",
                table: "STInventoryDetail");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class RenameTransactionIdToTransactionNoOnWHInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "WHInventory",
                newName: "TransactionNo");

            migrationBuilder.CreateIndex(
                name: "IX_STInventory_WarehouseId",
                table: "STInventory",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_STDeliveryDetail_ItemId",
                table: "STDeliveryDetail",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_STDeliveryDetail_Item_ItemId",
                table: "STDeliveryDetail",
                column: "ItemId",
                principalTable: "Item",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_STInventory_Warehouse_WarehouseId",
                table: "STInventory",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STDeliveryDetail_Item_ItemId",
                table: "STDeliveryDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_STInventory_Warehouse_WarehouseId",
                table: "STInventory");

            migrationBuilder.DropIndex(
                name: "IX_STInventory_WarehouseId",
                table: "STInventory");

            migrationBuilder.DropIndex(
                name: "IX_STDeliveryDetail_ItemId",
                table: "STDeliveryDetail");

            migrationBuilder.RenameColumn(
                name: "TransactionNo",
                table: "WHInventory",
                newName: "TransactionId");
        }
    }
}

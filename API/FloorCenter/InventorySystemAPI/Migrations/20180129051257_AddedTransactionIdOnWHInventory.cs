using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class AddedTransactionIdOnWHInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "WHInventory",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WHInventoryDetail_WHInventoryId",
                table: "WHInventoryDetail",
                column: "WHInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_CompanyId",
                table: "Store",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Store_WarehouseId",
                table: "Store",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Store_Company_CompanyId",
                table: "Store",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Store_Warehouse_WarehouseId",
                table: "Store",
                column: "WarehouseId",
                principalTable: "Warehouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WHInventoryDetail_WHInventory_WHInventoryId",
                table: "WHInventoryDetail",
                column: "WHInventoryId",
                principalTable: "WHInventory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Store_Company_CompanyId",
                table: "Store");

            migrationBuilder.DropForeignKey(
                name: "FK_Store_Warehouse_WarehouseId",
                table: "Store");

            migrationBuilder.DropForeignKey(
                name: "FK_WHInventoryDetail_WHInventory_WHInventoryId",
                table: "WHInventoryDetail");

            migrationBuilder.DropIndex(
                name: "IX_WHInventoryDetail_WHInventoryId",
                table: "WHInventoryDetail");

            migrationBuilder.DropIndex(
                name: "IX_Store_CompanyId",
                table: "Store");

            migrationBuilder.DropIndex(
                name: "IX_Store_WarehouseId",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "WHInventory");
        }
    }
}

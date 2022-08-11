using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedTransactionNoColumnOnSTReturnTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionNo",
                table: "STReturns",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_STReturns_StoreId",
                table: "STReturns",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_STReturns_WarehouseId",
                table: "STReturns",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_STPurchaseReturns_ItemId",
                table: "STPurchaseReturns",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_STPurchaseReturns_Items_ItemId",
                table: "STPurchaseReturns",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_STReturns_Stores_StoreId",
                table: "STReturns",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_STReturns_Warehouses_WarehouseId",
                table: "STReturns",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STPurchaseReturns_Items_ItemId",
                table: "STPurchaseReturns");

            migrationBuilder.DropForeignKey(
                name: "FK_STReturns_Stores_StoreId",
                table: "STReturns");

            migrationBuilder.DropForeignKey(
                name: "FK_STReturns_Warehouses_WarehouseId",
                table: "STReturns");

            migrationBuilder.DropIndex(
                name: "IX_STReturns_StoreId",
                table: "STReturns");

            migrationBuilder.DropIndex(
                name: "IX_STReturns_WarehouseId",
                table: "STReturns");

            migrationBuilder.DropIndex(
                name: "IX_STPurchaseReturns_ItemId",
                table: "STPurchaseReturns");

            migrationBuilder.DropColumn(
                name: "TransactionNo",
                table: "STReturns");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class UpdatedItemSerialNumberToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "WHStockSummary",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "STStockSummary",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SerialNumber",
                table: "Items",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(int),
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WHStockSummary_ItemId",
                table: "WHStockSummary",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WHStockSummary_WarehouseId",
                table: "WHStockSummary",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_STStockSummary_ItemId",
                table: "STStockSummary",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_STStockSummary_StoreId",
                table: "STStockSummary",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_STStockSummary_Items_ItemId",
                table: "STStockSummary",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_STStockSummary_Stores_StoreId",
                table: "STStockSummary",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WHStockSummary_Items_ItemId",
                table: "WHStockSummary",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WHStockSummary_Warehouses_WarehouseId",
                table: "WHStockSummary",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STStockSummary_Items_ItemId",
                table: "STStockSummary");

            migrationBuilder.DropForeignKey(
                name: "FK_STStockSummary_Stores_StoreId",
                table: "STStockSummary");

            migrationBuilder.DropForeignKey(
                name: "FK_WHStockSummary_Items_ItemId",
                table: "WHStockSummary");

            migrationBuilder.DropForeignKey(
                name: "FK_WHStockSummary_Warehouses_WarehouseId",
                table: "WHStockSummary");

            migrationBuilder.DropIndex(
                name: "IX_WHStockSummary_ItemId",
                table: "WHStockSummary");

            migrationBuilder.DropIndex(
                name: "IX_WHStockSummary_WarehouseId",
                table: "WHStockSummary");

            migrationBuilder.DropIndex(
                name: "IX_STStockSummary_ItemId",
                table: "STStockSummary");

            migrationBuilder.DropIndex(
                name: "IX_STStockSummary_StoreId",
                table: "STStockSummary");

            migrationBuilder.AlterColumn<int>(
                name: "SerialNumber",
                table: "WHStockSummary",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SerialNumber",
                table: "STStockSummary",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SerialNumber",
                table: "Items",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 30,
                oldNullable: true);
        }
    }
}

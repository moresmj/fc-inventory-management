using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class RenamedColumnsForWarehouseImportAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "STImportId",
                table: "WHImportDetails");

            migrationBuilder.AddColumn<int>(
                name: "WHImportDetailId",
                table: "WHStocks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WHStocks_WHImportDetailId",
                table: "WHStocks",
                column: "WHImportDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_WHImports_WarehouseId",
                table: "WHImports",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_WHImportDetails_ItemId",
                table: "WHImportDetails",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_WHImportDetails_Items_ItemId",
                table: "WHImportDetails",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WHImports_Warehouses_WarehouseId",
                table: "WHImports",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WHStocks_WHImportDetails_WHImportDetailId",
                table: "WHStocks",
                column: "WHImportDetailId",
                principalTable: "WHImportDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WHImportDetails_Items_ItemId",
                table: "WHImportDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_WHImports_Warehouses_WarehouseId",
                table: "WHImports");

            migrationBuilder.DropForeignKey(
                name: "FK_WHStocks_WHImportDetails_WHImportDetailId",
                table: "WHStocks");

            migrationBuilder.DropIndex(
                name: "IX_WHStocks_WHImportDetailId",
                table: "WHStocks");

            migrationBuilder.DropIndex(
                name: "IX_WHImports_WarehouseId",
                table: "WHImports");

            migrationBuilder.DropIndex(
                name: "IX_WHImportDetails_ItemId",
                table: "WHImportDetails");

            migrationBuilder.DropColumn(
                name: "WHImportDetailId",
                table: "WHStocks");

            migrationBuilder.AddColumn<int>(
                name: "STImportId",
                table: "WHImportDetails",
                nullable: true);
        }
    }
}

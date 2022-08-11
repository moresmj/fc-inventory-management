using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedNewColumnsOnSTSalesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "STSales",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesAgent",
                table: "STSales",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_StoreId",
                table: "Users",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_WarehouseId",
                table: "Users",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_STStocks_ItemId",
                table: "STStocks",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_STStocks_Items_ItemId",
                table: "STStocks",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Stores_StoreId",
                table: "Users",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Warehouses_WarehouseId",
                table: "Users",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STStocks_Items_ItemId",
                table: "STStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Stores_StoreId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Warehouses_WarehouseId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_StoreId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_WarehouseId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_STStocks_ItemId",
                table: "STStocks");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "STSales");

            migrationBuilder.DropColumn(
                name: "SalesAgent",
                table: "STSales");
        }
    }
}

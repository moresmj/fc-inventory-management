using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedStoreIdSTSalesDetailIdColumnsOnSTStockTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "STSalesDetailId",
                table: "STStocks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "STStocks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_STSales_STOrderId",
                table: "STSales",
                column: "STOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_STSales_STOrders_STOrderId",
                table: "STSales",
                column: "STOrderId",
                principalTable: "STOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STSales_STOrders_STOrderId",
                table: "STSales");

            migrationBuilder.DropIndex(
                name: "IX_STSales_STOrderId",
                table: "STSales");

            migrationBuilder.DropColumn(
                name: "STSalesDetailId",
                table: "STStocks");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "STStocks");
        }
    }
}

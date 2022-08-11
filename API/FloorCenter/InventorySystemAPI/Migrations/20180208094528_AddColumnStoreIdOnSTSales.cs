using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class AddColumnStoreIdOnSTSales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "STSales",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_STStock_STDeliveryDetailId",
                table: "STStock",
                column: "STDeliveryDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_STStock_STDeliveryDetail_STDeliveryDetailId",
                table: "STStock",
                column: "STDeliveryDetailId",
                principalTable: "STDeliveryDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STStock_STDeliveryDetail_STDeliveryDetailId",
                table: "STStock");

            migrationBuilder.DropIndex(
                name: "IX_STStock_STDeliveryDetailId",
                table: "STStock");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "STSales");
        }
    }
}

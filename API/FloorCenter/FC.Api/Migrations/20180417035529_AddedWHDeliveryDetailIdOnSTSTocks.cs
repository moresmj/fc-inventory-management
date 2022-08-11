using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedWHDeliveryDetailIdOnSTSTocks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WHDeliveryDetailId",
                table: "STStocks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_STStocks_StoreId",
                table: "STStocks",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_STStocks_Stores_StoreId",
                table: "STStocks",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STStocks_Stores_StoreId",
                table: "STStocks");

            migrationBuilder.DropIndex(
                name: "IX_STStocks_StoreId",
                table: "STStocks");

            migrationBuilder.DropColumn(
                name: "WHDeliveryDetailId",
                table: "STStocks");
        }
    }
}

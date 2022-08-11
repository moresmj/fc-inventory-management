using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class RenameSTInventoryDetailIdToSTInventoryId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "STInventoryDetailId",
                table: "STDelivery",
                newName: "STInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_STDeliveryDetail_STDeliveryId",
                table: "STDeliveryDetail",
                column: "STDeliveryId");

            migrationBuilder.AddForeignKey(
                name: "FK_STDeliveryDetail_STDelivery_STDeliveryId",
                table: "STDeliveryDetail",
                column: "STDeliveryId",
                principalTable: "STDelivery",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STDeliveryDetail_STDelivery_STDeliveryId",
                table: "STDeliveryDetail");

            migrationBuilder.DropIndex(
                name: "IX_STDeliveryDetail_STDeliveryId",
                table: "STDeliveryDetail");

            migrationBuilder.RenameColumn(
                name: "STInventoryId",
                table: "STDelivery",
                newName: "STInventoryDetailId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedNavigationPropertiesOnWHStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WHStocks_STClientDeliveryId",
                table: "WHStocks",
                column: "STClientDeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_WHStocks_STOrderDetailId",
                table: "WHStocks",
                column: "STOrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_WHStocks_STShowroomDeliveryId",
                table: "WHStocks",
                column: "STShowroomDeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_WHStocks_WHDeliveryDetailId",
                table: "WHStocks",
                column: "WHDeliveryDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_WHStocks_WHReceiveDetailId",
                table: "WHStocks",
                column: "WHReceiveDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_WHDeliveries_StoreId",
                table: "WHDeliveries",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_STDeliveries_StoreId",
                table: "STDeliveries",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_STDeliveries_Stores_StoreId",
                table: "STDeliveries",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WHDeliveries_Stores_StoreId",
                table: "WHDeliveries",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WHStocks_STClientDeliveries_STClientDeliveryId",
                table: "WHStocks",
                column: "STClientDeliveryId",
                principalTable: "STClientDeliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WHStocks_STOrderDetails_STOrderDetailId",
                table: "WHStocks",
                column: "STOrderDetailId",
                principalTable: "STOrderDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WHStocks_STShowroomDeliveries_STShowroomDeliveryId",
                table: "WHStocks",
                column: "STShowroomDeliveryId",
                principalTable: "STShowroomDeliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WHStocks_WHDeliveryDetails_WHDeliveryDetailId",
                table: "WHStocks",
                column: "WHDeliveryDetailId",
                principalTable: "WHDeliveryDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WHStocks_WHReceiveDetails_WHReceiveDetailId",
                table: "WHStocks",
                column: "WHReceiveDetailId",
                principalTable: "WHReceiveDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STDeliveries_Stores_StoreId",
                table: "STDeliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_WHDeliveries_Stores_StoreId",
                table: "WHDeliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_WHStocks_STClientDeliveries_STClientDeliveryId",
                table: "WHStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_WHStocks_STOrderDetails_STOrderDetailId",
                table: "WHStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_WHStocks_STShowroomDeliveries_STShowroomDeliveryId",
                table: "WHStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_WHStocks_WHDeliveryDetails_WHDeliveryDetailId",
                table: "WHStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_WHStocks_WHReceiveDetails_WHReceiveDetailId",
                table: "WHStocks");

            migrationBuilder.DropIndex(
                name: "IX_WHStocks_STClientDeliveryId",
                table: "WHStocks");

            migrationBuilder.DropIndex(
                name: "IX_WHStocks_STOrderDetailId",
                table: "WHStocks");

            migrationBuilder.DropIndex(
                name: "IX_WHStocks_STShowroomDeliveryId",
                table: "WHStocks");

            migrationBuilder.DropIndex(
                name: "IX_WHStocks_WHDeliveryDetailId",
                table: "WHStocks");

            migrationBuilder.DropIndex(
                name: "IX_WHStocks_WHReceiveDetailId",
                table: "WHStocks");

            migrationBuilder.DropIndex(
                name: "IX_WHDeliveries_StoreId",
                table: "WHDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_STDeliveries_StoreId",
                table: "STDeliveries");
        }
    }
}

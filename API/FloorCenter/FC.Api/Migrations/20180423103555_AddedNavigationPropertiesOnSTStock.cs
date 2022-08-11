using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedNavigationPropertiesOnSTStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_STStocks_STClientDeliveryId",
                table: "STStocks",
                column: "STClientDeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_STStocks_STClientReturnId",
                table: "STStocks",
                column: "STClientReturnId");

            migrationBuilder.CreateIndex(
                name: "IX_STStocks_STOrderDetailId",
                table: "STStocks",
                column: "STOrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_STStocks_STSalesDetailId",
                table: "STStocks",
                column: "STSalesDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_STStocks_STShowroomDeliveryId",
                table: "STStocks",
                column: "STShowroomDeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_STStocks_WHDeliveryDetailId",
                table: "STStocks",
                column: "WHDeliveryDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_STStocks_STClientDeliveries_STClientDeliveryId",
                table: "STStocks",
                column: "STClientDeliveryId",
                principalTable: "STClientDeliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_STStocks_STClientReturns_STClientReturnId",
                table: "STStocks",
                column: "STClientReturnId",
                principalTable: "STClientReturns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_STStocks_STOrderDetails_STOrderDetailId",
                table: "STStocks",
                column: "STOrderDetailId",
                principalTable: "STOrderDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_STStocks_STSalesDetails_STSalesDetailId",
                table: "STStocks",
                column: "STSalesDetailId",
                principalTable: "STSalesDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_STStocks_STShowroomDeliveries_STShowroomDeliveryId",
                table: "STStocks",
                column: "STShowroomDeliveryId",
                principalTable: "STShowroomDeliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_STStocks_WHDeliveryDetails_WHDeliveryDetailId",
                table: "STStocks",
                column: "WHDeliveryDetailId",
                principalTable: "WHDeliveryDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STStocks_STClientDeliveries_STClientDeliveryId",
                table: "STStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_STStocks_STClientReturns_STClientReturnId",
                table: "STStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_STStocks_STOrderDetails_STOrderDetailId",
                table: "STStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_STStocks_STSalesDetails_STSalesDetailId",
                table: "STStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_STStocks_STShowroomDeliveries_STShowroomDeliveryId",
                table: "STStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_STStocks_WHDeliveryDetails_WHDeliveryDetailId",
                table: "STStocks");

            migrationBuilder.DropIndex(
                name: "IX_STStocks_STClientDeliveryId",
                table: "STStocks");

            migrationBuilder.DropIndex(
                name: "IX_STStocks_STClientReturnId",
                table: "STStocks");

            migrationBuilder.DropIndex(
                name: "IX_STStocks_STOrderDetailId",
                table: "STStocks");

            migrationBuilder.DropIndex(
                name: "IX_STStocks_STSalesDetailId",
                table: "STStocks");

            migrationBuilder.DropIndex(
                name: "IX_STStocks_STShowroomDeliveryId",
                table: "STStocks");

            migrationBuilder.DropIndex(
                name: "IX_STStocks_WHDeliveryDetailId",
                table: "STStocks");
        }
    }
}

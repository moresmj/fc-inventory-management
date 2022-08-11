using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedSTSalesDetailIdOnSTClientDeliveryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryStatus",
                table: "STSalesDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "STSalesDetailId",
                table: "STClientDeliveries",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_STDeliveries_STSalesId",
                table: "STDeliveries",
                column: "STSalesId");

            migrationBuilder.AddForeignKey(
                name: "FK_STDeliveries_STSales_STSalesId",
                table: "STDeliveries",
                column: "STSalesId",
                principalTable: "STSales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STDeliveries_STSales_STSalesId",
                table: "STDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_STDeliveries_STSalesId",
                table: "STDeliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryStatus",
                table: "STSalesDetails");

            migrationBuilder.DropColumn(
                name: "STSalesDetailId",
                table: "STClientDeliveries");
        }
    }
}

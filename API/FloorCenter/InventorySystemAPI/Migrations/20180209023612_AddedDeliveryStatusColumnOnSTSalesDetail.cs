using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class AddedDeliveryStatusColumnOnSTSalesDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STReleaseDetail_STSalesDetail_STSalesDetailId",
                table: "STReleaseDetail");

            migrationBuilder.DropIndex(
                name: "IX_STReleaseDetail_STSalesDetailId",
                table: "STReleaseDetail");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryStatus",
                table: "STSalesDetail",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryStatus",
                table: "STSalesDetail");

            migrationBuilder.CreateIndex(
                name: "IX_STReleaseDetail_STSalesDetailId",
                table: "STReleaseDetail",
                column: "STSalesDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_STReleaseDetail_STSalesDetail_STSalesDetailId",
                table: "STReleaseDetail",
                column: "STSalesDetailId",
                principalTable: "STSalesDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

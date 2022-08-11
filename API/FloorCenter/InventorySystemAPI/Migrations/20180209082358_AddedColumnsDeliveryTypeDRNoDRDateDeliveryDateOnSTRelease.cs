using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class AddedColumnsDeliveryTypeDRNoDRDateDeliveryDateOnSTRelease : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DRDate",
                table: "STRelease",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DRNo",
                table: "STRelease",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "STRelease",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryType",
                table: "STRelease",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STReleaseDetail_STSalesDetail_STSalesDetailId",
                table: "STReleaseDetail");

            migrationBuilder.DropIndex(
                name: "IX_STReleaseDetail_STSalesDetailId",
                table: "STReleaseDetail");

            migrationBuilder.DropColumn(
                name: "DRDate",
                table: "STRelease");

            migrationBuilder.DropColumn(
                name: "DRNo",
                table: "STRelease");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "STRelease");

            migrationBuilder.DropColumn(
                name: "DeliveryType",
                table: "STRelease");
        }
    }
}

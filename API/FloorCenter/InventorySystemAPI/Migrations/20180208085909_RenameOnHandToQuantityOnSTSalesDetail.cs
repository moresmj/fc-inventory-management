using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class RenameOnHandToQuantityOnSTSalesDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OnHand",
                table: "STSalesDetail",
                newName: "Quantity");

            migrationBuilder.CreateIndex(
                name: "IX_STSalesDetail_STSalesId",
                table: "STSalesDetail",
                column: "STSalesId");

            migrationBuilder.AddForeignKey(
                name: "FK_STSalesDetail_STSales_STSalesId",
                table: "STSalesDetail",
                column: "STSalesId",
                principalTable: "STSales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STSalesDetail_STSales_STSalesId",
                table: "STSalesDetail");

            migrationBuilder.DropIndex(
                name: "IX_STSalesDetail_STSalesId",
                table: "STSalesDetail");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "STSalesDetail",
                newName: "OnHand");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddAllocationDetailIdFieldOnWhStocks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AllocatedAdvanceOrderId",
                table: "WHAllocateAdvanceOrderDetail",
                newName: "WHAllocatedAdvanceOrderId");

            migrationBuilder.AddColumn<int>(
                name: "WHAllocateAdvanceOrderDetailId",
                table: "WHStocks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WHStocks_WHAllocateAdvanceOrderDetailId",
                table: "WHStocks",
                column: "WHAllocateAdvanceOrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_WHStocks_WHAllocateAdvanceOrderDetail_WHAllocateAdvanceOrderDetailId",
                table: "WHStocks",
                column: "WHAllocateAdvanceOrderDetailId",
                principalTable: "WHAllocateAdvanceOrderDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WHStocks_WHAllocateAdvanceOrderDetail_WHAllocateAdvanceOrderDetailId",
                table: "WHStocks");

            migrationBuilder.DropIndex(
                name: "IX_WHStocks_WHAllocateAdvanceOrderDetailId",
                table: "WHStocks");

            migrationBuilder.DropColumn(
                name: "WHAllocateAdvanceOrderDetailId",
                table: "WHStocks");

            migrationBuilder.RenameColumn(
                name: "WHAllocatedAdvanceOrderId",
                table: "WHAllocateAdvanceOrderDetail",
                newName: "AllocatedAdvanceOrderId");
        }
    }
}

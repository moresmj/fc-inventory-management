using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedSTImportDetailIdOnSTStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "STImportDetailId",
                table: "STStocks",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_STStocks_STImportDetailId",
                table: "STStocks",
                column: "STImportDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_STStocks_STImportDetails_STImportDetailId",
                table: "STStocks",
                column: "STImportDetailId",
                principalTable: "STImportDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STStocks_STImportDetails_STImportDetailId",
                table: "STStocks");

            migrationBuilder.DropIndex(
                name: "IX_STStocks_STImportDetailId",
                table: "STStocks");

            migrationBuilder.DropColumn(
                name: "STImportDetailId",
                table: "STStocks");
        }
    }
}

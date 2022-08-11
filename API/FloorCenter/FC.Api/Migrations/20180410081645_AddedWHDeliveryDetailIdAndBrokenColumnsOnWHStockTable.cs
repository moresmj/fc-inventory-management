using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedWHDeliveryDetailIdAndBrokenColumnsOnWHStockTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Broken",
                table: "WHStocks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "WHDeliveryDetailId",
                table: "WHStocks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Broken",
                table: "WHStocks");

            migrationBuilder.DropColumn(
                name: "WHDeliveryDetailId",
                table: "WHStocks");
        }
    }
}

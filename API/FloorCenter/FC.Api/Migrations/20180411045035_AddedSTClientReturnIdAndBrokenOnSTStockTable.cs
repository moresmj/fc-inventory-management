using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedSTClientReturnIdAndBrokenOnSTStockTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Broken",
                table: "STStocks",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "STClientReturnId",
                table: "STStocks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Broken",
                table: "STStocks");

            migrationBuilder.DropColumn(
                name: "STClientReturnId",
                table: "STStocks");
        }
    }
}

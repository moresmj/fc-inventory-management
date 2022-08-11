using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class addReservedColumnWhStockReceiveDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Reserved",
                table: "WHStocks",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReservedQuantity",
                table: "WHReceiveDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reserved",
                table: "WHStocks");

            migrationBuilder.DropColumn(
                name: "ReservedQuantity",
                table: "WHReceiveDetails");
        }
    }
}

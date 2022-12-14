using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedSTShowroomDeliveryIdOnWHStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "STShowroomDeliveryId",
                table: "WHStocks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "STShowroomDeliveryId",
                table: "WHStocks");
        }
    }
}

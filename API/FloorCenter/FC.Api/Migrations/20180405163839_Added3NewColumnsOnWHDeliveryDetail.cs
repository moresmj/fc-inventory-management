using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class Added3NewColumnsOnWHDeliveryDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReceivedBrokenQuantity",
                table: "WHDeliveryDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedGoodQuantity",
                table: "WHDeliveryDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceivedRemarks",
                table: "WHDeliveryDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceivedBrokenQuantity",
                table: "WHDeliveryDetails");

            migrationBuilder.DropColumn(
                name: "ReceivedGoodQuantity",
                table: "WHDeliveryDetails");

            migrationBuilder.DropColumn(
                name: "ReceivedRemarks",
                table: "WHDeliveryDetails");
        }
    }
}

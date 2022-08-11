using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class MovedDeliveryTypeFromSTDeliveryToSTOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryType",
                table: "STDeliveries");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryType",
                table: "STOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryType",
                table: "STOrders");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryType",
                table: "STDeliveries",
                nullable: true);
        }
    }
}

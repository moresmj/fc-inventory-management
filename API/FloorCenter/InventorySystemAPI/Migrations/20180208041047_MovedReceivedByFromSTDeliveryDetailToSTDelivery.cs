using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class MovedReceivedByFromSTDeliveryDetailToSTDelivery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "STDeliveryDetail");

            migrationBuilder.AddColumn<int>(
                name: "ReceivedBy",
                table: "STDelivery",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "STDelivery");

            migrationBuilder.AddColumn<int>(
                name: "ReceivedBy",
                table: "STDeliveryDetail",
                nullable: true);
        }
    }
}

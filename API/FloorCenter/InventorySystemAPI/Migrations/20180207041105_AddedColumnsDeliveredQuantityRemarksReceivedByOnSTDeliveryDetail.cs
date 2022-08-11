using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class AddedColumnsDeliveredQuantityRemarksReceivedByOnSTDeliveryDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveredQuantity",
                table: "STDeliveryDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedBy",
                table: "STDeliveryDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "STDeliveryDetail",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveredQuantity",
                table: "STDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "ReceivedBy",
                table: "STDeliveryDetail");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "STDeliveryDetail");
        }
    }
}

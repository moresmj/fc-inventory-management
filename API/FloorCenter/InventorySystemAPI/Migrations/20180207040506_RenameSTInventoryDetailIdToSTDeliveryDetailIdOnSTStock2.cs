using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class RenameSTInventoryDetailIdToSTDeliveryDetailIdOnSTStock2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "STInventoryDetailId",
                table: "STStock",
                newName: "STDeliveryDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "STDeliveryDetailId",
                table: "STStock",
                newName: "STInventoryDetailId");
        }
    }
}

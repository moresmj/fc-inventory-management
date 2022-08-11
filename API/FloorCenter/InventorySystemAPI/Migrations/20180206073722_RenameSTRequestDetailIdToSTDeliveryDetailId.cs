using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class RenameSTRequestDetailIdToSTDeliveryDetailId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "STRequestDetailId",
                table: "WHStock",
                newName: "STDeliveryDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "STDeliveryDetailId",
                table: "WHStock",
                newName: "STRequestDetailId");
        }
    }
}

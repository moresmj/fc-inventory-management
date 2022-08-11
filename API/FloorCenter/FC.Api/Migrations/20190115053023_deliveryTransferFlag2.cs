using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class deliveryTransferFlag2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isDeliveryTransfer",
                table: "STStocks",
                newName: "IsDeliveryTransfer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeliveryTransfer",
                table: "STStocks",
                newName: "isDeliveryTransfer");
        }
    }
}

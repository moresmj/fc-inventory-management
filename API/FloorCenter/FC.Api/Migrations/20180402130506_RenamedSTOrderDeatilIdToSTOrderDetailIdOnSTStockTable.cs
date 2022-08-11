using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class RenamedSTOrderDeatilIdToSTOrderDetailIdOnSTStockTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "STOrderDeatilId",
                table: "STStocks",
                newName: "STOrderDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "STOrderDetailId",
                table: "STStocks",
                newName: "STOrderDeatilId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class ChangedSTOrderDetailIdToNullableOnSTStocksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "STOrderDeatilId",
                table: "STStocks",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "STOrderDeatilId",
                table: "STStocks",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}

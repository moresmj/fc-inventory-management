using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedColumnsOnSTOrderDetailTAble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateReleased",
                table: "STOrderDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReleaseStatus",
                table: "STOrderDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateReleased",
                table: "STOrderDetails");

            migrationBuilder.DropColumn(
                name: "ReleaseStatus",
                table: "STOrderDetails");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class addisTonalityAny2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isTonalityAny",
                table: "STOrders");

            migrationBuilder.AddColumn<bool>(
                name: "isTonalityAny",
                table: "STOrderDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isTonalityAny",
                table: "STOrderDetails");

            migrationBuilder.AddColumn<bool>(
                name: "isTonalityAny",
                table: "STOrders",
                nullable: true);
        }
    }
}

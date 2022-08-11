using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class PhysicalCountType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhysicalCountType",
                table: "WHImports",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMode",
                table: "STOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesAgent",
                table: "STOrders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhysicalCountType",
                table: "STImports",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhysicalCountType",
                table: "WHImports");

            migrationBuilder.DropColumn(
                name: "PaymentMode",
                table: "STOrders");

            migrationBuilder.DropColumn(
                name: "SalesAgent",
                table: "STOrders");

            migrationBuilder.DropColumn(
                name: "PhysicalCountType",
                table: "STImports");
        }
    }
}

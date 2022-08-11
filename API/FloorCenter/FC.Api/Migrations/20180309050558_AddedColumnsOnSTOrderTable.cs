using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedColumnsOnSTOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address1",
                table: "STOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "STOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address3",
                table: "STOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "STOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "STOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address1",
                table: "STOrders");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "STOrders");

            migrationBuilder.DropColumn(
                name: "Address3",
                table: "STOrders");

            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "STOrders");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "STOrders");
        }
    }
}

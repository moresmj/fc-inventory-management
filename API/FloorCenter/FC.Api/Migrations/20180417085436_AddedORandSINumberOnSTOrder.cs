using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedORandSINumberOnSTOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ORNumber",
                table: "STOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SINumber",
                table: "STOrders",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "ORNumber",
                table: "STOrders");

            migrationBuilder.DropColumn(
                name: "SINumber",
                table: "STOrders");
        }
    }
}

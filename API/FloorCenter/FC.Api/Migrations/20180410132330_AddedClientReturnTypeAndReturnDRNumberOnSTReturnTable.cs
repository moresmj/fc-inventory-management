using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedClientReturnTypeAndReturnDRNumberOnSTReturnTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientReturnType",
                table: "STReturns",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnDRNumber",
                table: "STReturns",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientReturnType",
                table: "STReturns");

            migrationBuilder.DropColumn(
                name: "ReturnDRNumber",
                table: "STReturns");
        }
    }
}

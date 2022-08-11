using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddRequestStatusAdvanceOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "STAdvanceOrder");

            migrationBuilder.AddColumn<int>(
                name: "RequestStatus",
                table: "STAdvanceOrder",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestStatus",
                table: "STAdvanceOrder");

            migrationBuilder.AddColumn<int>(
                name: "OrderStatus",
                table: "STAdvanceOrder",
                nullable: true);
        }
    }
}

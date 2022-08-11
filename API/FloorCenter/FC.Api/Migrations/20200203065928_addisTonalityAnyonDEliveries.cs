using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class addisTonalityAnyonDEliveries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isTonalityAny",
                table: "STShowroomDeliveries",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isTonalityAny",
                table: "STClientDeliveries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isTonalityAny",
                table: "STShowroomDeliveries");

            migrationBuilder.DropColumn(
                name: "isTonalityAny",
                table: "STClientDeliveries");
        }
    }
}

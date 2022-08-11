using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedApprovedDeliveryDateDriverNameAndPlateNoOnSTReturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedDeliveryDate",
                table: "STReturns",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverName",
                table: "STReturns",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlateNumber",
                table: "STReturns",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedDeliveryDate",
                table: "STReturns");

            migrationBuilder.DropColumn(
                name: "DriverName",
                table: "STReturns");

            migrationBuilder.DropColumn(
                name: "PlateNumber",
                table: "STReturns");
        }
    }
}

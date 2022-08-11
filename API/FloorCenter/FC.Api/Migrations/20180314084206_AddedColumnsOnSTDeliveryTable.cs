using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedColumnsOnSTDeliveryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address1",
                table: "STDeliveries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "STDeliveries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address3",
                table: "STDeliveries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientName",
                table: "STDeliveries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "STDeliveries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ORNumber",
                table: "STDeliveries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "STDeliveries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SINumber",
                table: "STDeliveries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address1",
                table: "STDeliveries");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "STDeliveries");

            migrationBuilder.DropColumn(
                name: "Address3",
                table: "STDeliveries");

            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "STDeliveries");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "STDeliveries");

            migrationBuilder.DropColumn(
                name: "ORNumber",
                table: "STDeliveries");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "STDeliveries");

            migrationBuilder.DropColumn(
                name: "SINumber",
                table: "STDeliveries");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedRemarksForPhysicalCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "WHImportDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AdjustmentCount",
                table: "STImportDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "STImportDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "WHImportDetails");

            migrationBuilder.DropColumn(
                name: "AdjustmentCount",
                table: "STImportDetails");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "STImportDetails");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class addDeliveryidonMOdifyTonalityDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StClientDeliveryId",
                table: "WHModifyItemTonalityDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StShowroomDeliveryId",
                table: "WHModifyItemTonalityDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StClientDeliveryId",
                table: "WHModifyItemTonalityDetails");

            migrationBuilder.DropColumn(
                name: "StShowroomDeliveryId",
                table: "WHModifyItemTonalityDetails");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class stPurchaseReturnIsTonalityAny : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isTonalityAny",
                table: "WHDeliveryDetails",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isTonalityAny",
                table: "STPurchaseReturns",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isTonalityAny",
                table: "WHDeliveryDetails");

            migrationBuilder.DropColumn(
                name: "isTonalityAny",
                table: "STPurchaseReturns");
        }
    }
}

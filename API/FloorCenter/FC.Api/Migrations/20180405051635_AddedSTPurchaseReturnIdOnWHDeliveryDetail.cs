using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedSTPurchaseReturnIdOnWHDeliveryDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "STPurchaseReturnId",
                table: "WHDeliveryDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WHDeliveries_STReturnId",
                table: "WHDeliveries",
                column: "STReturnId");

            migrationBuilder.AddForeignKey(
                name: "FK_WHDeliveries_STReturns_STReturnId",
                table: "WHDeliveries",
                column: "STReturnId",
                principalTable: "STReturns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WHDeliveries_STReturns_STReturnId",
                table: "WHDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_WHDeliveries_STReturnId",
                table: "WHDeliveries");

            migrationBuilder.DropColumn(
                name: "STPurchaseReturnId",
                table: "WHDeliveryDetails");
        }
    }
}

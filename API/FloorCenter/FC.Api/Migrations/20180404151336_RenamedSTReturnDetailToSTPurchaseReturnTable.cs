using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class RenamedSTReturnDetailToSTPurchaseReturnTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STReturnDetails_STReturns_STReturnId",
                table: "STReturnDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_STReturnDetails",
                table: "STReturnDetails");

            migrationBuilder.RenameTable(
                name: "STReturnDetails",
                newName: "STPurchaseReturns");

            migrationBuilder.RenameIndex(
                name: "IX_STReturnDetails_STReturnId",
                table: "STPurchaseReturns",
                newName: "IX_STPurchaseReturns_STReturnId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_STPurchaseReturns",
                table: "STPurchaseReturns",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_STPurchaseReturns_STReturns_STReturnId",
                table: "STPurchaseReturns",
                column: "STReturnId",
                principalTable: "STReturns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STPurchaseReturns_STReturns_STReturnId",
                table: "STPurchaseReturns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_STPurchaseReturns",
                table: "STPurchaseReturns");

            migrationBuilder.RenameTable(
                name: "STPurchaseReturns",
                newName: "STReturnDetails");

            migrationBuilder.RenameIndex(
                name: "IX_STPurchaseReturns_STReturnId",
                table: "STReturnDetails",
                newName: "IX_STReturnDetails_STReturnId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_STReturnDetails",
                table: "STReturnDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_STReturnDetails_STReturns_STReturnId",
                table: "STReturnDetails",
                column: "STReturnId",
                principalTable: "STReturns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

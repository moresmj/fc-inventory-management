using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class CustomItemForSTAdvanceOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WHAllocatedAdvanceOrderId",
                table: "WHAllocateAdvanceOrderDetail");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "STAdvanceOrderDetails",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isCustom",
                table: "STAdvanceOrderDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sizeId",
                table: "STAdvanceOrderDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tonality",
                table: "STAdvanceOrderDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "STAdvanceOrderDetails");

            migrationBuilder.DropColumn(
                name: "isCustom",
                table: "STAdvanceOrderDetails");

            migrationBuilder.DropColumn(
                name: "sizeId",
                table: "STAdvanceOrderDetails");

            migrationBuilder.DropColumn(
                name: "tonality",
                table: "STAdvanceOrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "WHAllocatedAdvanceOrderId",
                table: "WHAllocateAdvanceOrderDetail",
                nullable: true);
        }
    }
}

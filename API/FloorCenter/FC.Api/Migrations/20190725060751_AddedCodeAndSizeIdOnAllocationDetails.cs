using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedCodeAndSizeIdOnAllocationDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "WHAllocateAdvanceOrderDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "WHAllocateAdvanceOrderDetail",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "WHAllocateAdvanceOrderDetail");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "WHAllocateAdvanceOrderDetail");
        }
    }
}

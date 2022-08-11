using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class RemovedDecoratorForDRNoOnSTRelease : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DRNo",
                table: "STRelease",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "date",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DRNo",
                table: "STRelease",
                type: "date",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

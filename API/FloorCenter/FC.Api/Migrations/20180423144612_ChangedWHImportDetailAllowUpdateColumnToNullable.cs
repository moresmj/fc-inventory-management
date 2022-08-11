using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class ChangedWHImportDetailAllowUpdateColumnToNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "AllowUpdate",
                table: "WHImportDetails",
                nullable: true,
                oldClrType: typeof(bool));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "AllowUpdate",
                table: "WHImportDetails",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}

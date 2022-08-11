using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedBrokenOnWHImportDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Broken",
                table: "WHImportDetails",
                nullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "QtyPerBox",
            //    table: "Items",
            //    nullable: true,
            //    oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Broken",
                table: "WHImportDetails");

            //migrationBuilder.AlterColumn<int>(
            //    name: "QtyPerBox",
            //    table: "Items",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldNullable: true);
        }
    }
}

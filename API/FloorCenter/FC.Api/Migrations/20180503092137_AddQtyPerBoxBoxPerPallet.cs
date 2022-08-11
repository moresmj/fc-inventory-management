using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddQtyPerBoxBoxPerPallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
            name: "QtyPerBox",
            table: "Items",
            nullable: true);

            migrationBuilder.AddColumn<string>(
            name: "BoxPerPallet",
            table: "Items",
            nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
               name: "QtyPerBox",
               table: "Items");


            migrationBuilder.DropColumn(
               name: "BoxPerPallet",
               table: "Items");
        }
    }
}

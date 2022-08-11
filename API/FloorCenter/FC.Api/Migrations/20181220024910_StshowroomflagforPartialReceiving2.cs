using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class StshowroomflagforPartialReceiving2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsRemainingForReceiving",
                table: "STShowroomDeliveries",
                nullable: true,
                oldClrType: typeof(bool));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsRemainingForReceiving",
                table: "STShowroomDeliveries",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}

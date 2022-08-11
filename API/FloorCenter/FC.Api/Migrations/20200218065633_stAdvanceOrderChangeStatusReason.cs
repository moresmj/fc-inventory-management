using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class stAdvanceOrderChangeStatusReason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "changeStatusReasons",
            //    table: "STAdvanceOrderDetails",
            //    nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "changeStatusReasons",
                table: "STAdvanceOrder",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "changeStatusReasons",
                table: "STAdvanceOrderDetails");

            migrationBuilder.DropColumn(
                name: "changeStatusReasons",
                table: "STAdvanceOrder");
        }
    }
}

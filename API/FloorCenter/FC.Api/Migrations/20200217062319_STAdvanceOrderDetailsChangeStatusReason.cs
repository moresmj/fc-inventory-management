using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class STAdvanceOrderDetailsChangeStatusReason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "ChangeStatusReason",
            //    table: "STAdvanceOrderDetails",
            //    nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeStatusReason",
                table: "STAdvanceOrderDetails");
        }
    }
}

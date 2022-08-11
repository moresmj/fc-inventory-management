using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class RemovedColumnChangeStatusReasonStAdvanceOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeStatusReason",
                table: "STAdvanceOrderDetails");

            //migrationBuilder.RenameColumn(
            //    name: "ChangeStatusReason",
            //    table: "STAdvanceOrder",
            //    newName: "changeStatusReasons");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "changeStatusReasons",
                table: "STAdvanceOrder",
                newName: "ChangeStatusReason");

            migrationBuilder.AddColumn<string>(
                name: "ChangeStatusReason",
                table: "STAdvanceOrderDetails",
                nullable: true);
        }
    }
}

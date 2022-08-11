using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class RemovedStoreReleaseStatusOnSTShowroomDeliveryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoreReleaseStatus",
                table: "STShowroomDeliveries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreReleaseStatus",
                table: "STShowroomDeliveries",
                nullable: true);
        }
    }
}

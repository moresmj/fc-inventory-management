using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class RenamedWHInventorDetailIdToWHInventoryDetailIdAddedNewTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WHInventorDetailId",
                table: "WHStock",
                newName: "WHInventoryDetailId");

            migrationBuilder.CreateTable(
                name: "STStock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DeliveryStatus = table.Column<int>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    OnHand = table.Column<int>(nullable: true),
                    STInventoryDetailId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STStock", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STStock");

            migrationBuilder.RenameColumn(
                name: "WHInventoryDetailId",
                table: "WHStock",
                newName: "WHInventorDetailId");
        }
    }
}

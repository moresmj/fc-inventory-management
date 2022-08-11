using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedSTStockSummary_WHStockSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "STStockSummary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Available = table.Column<int>(nullable: false),
                    Broken = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ForRelease = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: true),
                    ItemName = table.Column<string>(nullable: true),
                    OnHand = table.Column<int>(nullable: false),
                    SerialNumber = table.Column<int>(nullable: true),
                    SizeId = table.Column<int>(nullable: true),
                    SizeName = table.Column<string>(nullable: true),
                    StoreId = table.Column<int>(nullable: true),
                    Tonality = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STStockSummary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WHStockSummary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Available = table.Column<int>(nullable: false),
                    Broken = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ForRelease = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: true),
                    ItemName = table.Column<string>(nullable: true),
                    OnHand = table.Column<int>(nullable: false),
                    SerialNumber = table.Column<int>(nullable: true),
                    SizeId = table.Column<int>(nullable: true),
                    SizeName = table.Column<string>(nullable: true),
                    Tonality = table.Column<string>(nullable: true),
                    WarehouseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WHStockSummary", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STStockSummary");

            migrationBuilder.DropTable(
                name: "WHStockSummary");
        }
    }
}

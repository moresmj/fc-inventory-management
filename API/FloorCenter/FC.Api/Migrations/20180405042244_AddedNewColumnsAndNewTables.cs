using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedNewColumnsAndNewTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveryStatus",
                table: "STPurchaseReturns",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WHDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApprovedDeliveryDate = table.Column<DateTime>(nullable: true),
                    DRNumber = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DeliveryDate = table.Column<DateTime>(nullable: true),
                    DriverName = table.Column<string>(nullable: true),
                    PlateNumber = table.Column<string>(nullable: true),
                    STReturnId = table.Column<int>(nullable: true),
                    StoreId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WHDeliveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WHDeliveryDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DeliveryStatus = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    ReleaseStatus = table.Column<int>(nullable: false),
                    WHDeliveryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WHDeliveryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WHDeliveryDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WHDeliveryDetails_WHDeliveries_WHDeliveryId",
                        column: x => x.WHDeliveryId,
                        principalTable: "WHDeliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WHDeliveryDetails_ItemId",
                table: "WHDeliveryDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WHDeliveryDetails_WHDeliveryId",
                table: "WHDeliveryDetails",
                column: "WHDeliveryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WHDeliveryDetails");

            migrationBuilder.DropTable(
                name: "WHDeliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryStatus",
                table: "STPurchaseReturns");
        }
    }
}

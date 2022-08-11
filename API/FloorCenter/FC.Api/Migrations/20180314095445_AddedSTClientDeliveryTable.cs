using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedSTClientDeliveryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "STClientDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DeliveredQuantity = table.Column<int>(nullable: true),
                    DeliveryStatus = table.Column<int>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    ReleaseStatus = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    STDeliveryId = table.Column<int>(nullable: true),
                    STOrderDetailId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STClientDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STClientDeliveries_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_STClientDeliveries_STDeliveries_STDeliveryId",
                        column: x => x.STDeliveryId,
                        principalTable: "STDeliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_STClientDeliveries_ItemId",
                table: "STClientDeliveries",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_STClientDeliveries_STDeliveryId",
                table: "STClientDeliveries",
                column: "STDeliveryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STClientDeliveries");
        }
    }
}

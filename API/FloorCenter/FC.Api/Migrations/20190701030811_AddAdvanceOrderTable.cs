using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddAdvanceOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "STAdvanceOrder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AONumber = table.Column<string>(maxLength: 50, nullable: true),
                    Address1 = table.Column<string>(maxLength: 255, nullable: true),
                    Address2 = table.Column<string>(maxLength: 255, nullable: true),
                    Address3 = table.Column<string>(maxLength: 255, nullable: true),
                    ClientName = table.Column<string>(maxLength: 255, nullable: true),
                    ContactNumber = table.Column<string>(maxLength: 255, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DeliveryStatus = table.Column<int>(nullable: true),
                    OrderStatus = table.Column<int>(nullable: true),
                    PONumber = table.Column<string>(maxLength: 50, nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    SINumber = table.Column<string>(maxLength: 50, nullable: true),
                    SalesAgent = table.Column<string>(nullable: true),
                    StoreId = table.Column<int>(nullable: true),
                    WarehouseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STAdvanceOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STAdvanceOrder_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_STAdvanceOrder_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "STAdvanceOrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DeliveryStatus = table.Column<int>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    STAdvanceOrderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STAdvanceOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STAdvanceOrderDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_STAdvanceOrderDetails_STAdvanceOrder_STAdvanceOrderId",
                        column: x => x.STAdvanceOrderId,
                        principalTable: "STAdvanceOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_STAdvanceOrder_StoreId",
                table: "STAdvanceOrder",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_STAdvanceOrder_WarehouseId_StoreId",
                table: "STAdvanceOrder",
                columns: new[] { "WarehouseId", "StoreId" });

            migrationBuilder.CreateIndex(
                name: "IX_STAdvanceOrderDetails_STAdvanceOrderId",
                table: "STAdvanceOrderDetails",
                column: "STAdvanceOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_STAdvanceOrderDetails_ItemId_STAdvanceOrderId",
                table: "STAdvanceOrderDetails",
                columns: new[] { "ItemId", "STAdvanceOrderId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STAdvanceOrderDetails");

            migrationBuilder.DropTable(
                name: "STAdvanceOrder");
        }
    }
}

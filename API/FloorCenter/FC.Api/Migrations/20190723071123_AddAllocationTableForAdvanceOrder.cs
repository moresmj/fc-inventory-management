using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddAllocationTableForAdvanceOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderStatus",
                table: "STAdvanceOrder",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WHAllocateAdvanceOrder",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllocationDate = table.Column<DateTime>(nullable: true),
                    AllocationNumber = table.Column<string>(maxLength: 50, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    StAdvanceOrderId = table.Column<int>(nullable: true),
                    StoreId = table.Column<int>(nullable: true),
                    WarehouseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WHAllocateAdvanceOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WHAllocateAdvanceOrder_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WHAllocateAdvanceOrder_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WHAllocateAdvanceOrderDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllocatedAdvanceOrderId = table.Column<int>(nullable: true),
                    AllocatedQuantity = table.Column<int>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    WHAllocateAdvanceOrderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WHAllocateAdvanceOrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WHAllocateAdvanceOrderDetail_WHAllocateAdvanceOrder_WHAllocateAdvanceOrderId",
                        column: x => x.WHAllocateAdvanceOrderId,
                        principalTable: "WHAllocateAdvanceOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WHAllocateAdvanceOrder_StoreId",
                table: "WHAllocateAdvanceOrder",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_WHAllocateAdvanceOrder_WarehouseId",
                table: "WHAllocateAdvanceOrder",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_WHAllocateAdvanceOrderDetail_WHAllocateAdvanceOrderId",
                table: "WHAllocateAdvanceOrderDetail",
                column: "WHAllocateAdvanceOrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WHAllocateAdvanceOrderDetail");

            migrationBuilder.DropTable(
                name: "WHAllocateAdvanceOrder");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "STAdvanceOrder");
        }
    }
}

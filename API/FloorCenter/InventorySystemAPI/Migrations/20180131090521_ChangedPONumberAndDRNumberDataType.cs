using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class ChangedPONumberAndDRNumberDataType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PONumber",
                table: "WHInventory",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DRNumber",
                table: "WHInventory",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "STInventory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    PODate = table.Column<DateTime>(type: "date", nullable: true),
                    PONumber = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    RequestStatus = table.Column<int>(nullable: false),
                    StoreId = table.Column<int>(nullable: true),
                    TransactionType = table.Column<int>(nullable: true),
                    WarehouseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STInventory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "STInventoryDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApprovedQuantity = table.Column<int>(nullable: true),
                    ApprovedRemarks = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    RequestedQuantity = table.Column<int>(nullable: true),
                    RequestedRemarks = table.Column<string>(nullable: true),
                    STInventoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STInventoryDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STInventoryDetail_STInventory_STInventoryId",
                        column: x => x.STInventoryId,
                        principalTable: "STInventory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_STInventoryDetail_STInventoryId",
                table: "STInventoryDetail",
                column: "STInventoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STInventoryDetail");

            migrationBuilder.DropTable(
                name: "STInventory");

            migrationBuilder.AlterColumn<int>(
                name: "PONumber",
                table: "WHInventory",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DRNumber",
                table: "WHInventory",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class addWhModifyItemTonality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WHModifyItemTonality",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateApproved = table.Column<DateTime>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    RequestStatus = table.Column<int>(nullable: true),
                    STOrderId = table.Column<int>(nullable: true),
                    WarehouseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WHModifyItemTonality", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WHModifyItemTonality_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WHModifyItemTonalityDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    OldItemId = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    RequestStatus = table.Column<int>(nullable: true),
                    WHModifyItemTonalityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WHModifyItemTonalityDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WHModifyItemTonalityDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WHModifyItemTonalityDetails_WHModifyItemTonality_WHModifyItemTonalityId",
                        column: x => x.WHModifyItemTonalityId,
                        principalTable: "WHModifyItemTonality",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WHModifyItemTonality_WarehouseId_STOrderId",
                table: "WHModifyItemTonality",
                columns: new[] { "WarehouseId", "STOrderId" });

            migrationBuilder.CreateIndex(
                name: "IX_WHModifyItemTonalityDetails_ItemId",
                table: "WHModifyItemTonalityDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WHModifyItemTonalityDetails_WHModifyItemTonalityId_ItemId",
                table: "WHModifyItemTonalityDetails",
                columns: new[] { "WHModifyItemTonalityId", "ItemId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WHModifyItemTonalityDetails");

            migrationBuilder.DropTable(
                name: "WHModifyItemTonality");
        }
    }
}

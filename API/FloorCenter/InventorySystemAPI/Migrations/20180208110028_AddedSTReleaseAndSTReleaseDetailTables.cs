using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class AddedSTReleaseAndSTReleaseDetailTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "STRelease",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateReleased = table.Column<DateTime>(type: "date", nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STRelease", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "STReleaseDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    STReleaseId = table.Column<int>(nullable: true),
                    STSalesDetailId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STReleaseDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STReleaseDetail_STRelease_STReleaseId",
                        column: x => x.STReleaseId,
                        principalTable: "STRelease",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_STReleaseDetail_STSalesDetail_STSalesDetailId",
                        column: x => x.STSalesDetailId,
                        principalTable: "STSalesDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_STReleaseDetail_STReleaseId",
                table: "STReleaseDetail",
                column: "STReleaseId");

            migrationBuilder.CreateIndex(
                name: "IX_STReleaseDetail_STSalesDetailId",
                table: "STReleaseDetail",
                column: "STSalesDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STReleaseDetail");

            migrationBuilder.DropTable(
                name: "STRelease");
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedTablesNeededForImportPhysicalCountAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "STImports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateApproved = table.Column<DateTime>(type: "date", nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DateUploaded = table.Column<DateTime>(type: "date", nullable: true),
                    RequestStatus = table.Column<int>(nullable: true),
                    StoreId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STImports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WHImports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateApproved = table.Column<DateTime>(type: "date", nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DateUploaded = table.Column<DateTime>(type: "date", nullable: true),
                    RequestStatus = table.Column<int>(nullable: true),
                    StoreId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WHImports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "STImportDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllowUpdate = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    PhysicalCount = table.Column<int>(nullable: true),
                    STImportId = table.Column<int>(nullable: true),
                    SystemCount = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STImportDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STImportDetails_STImports_STImportId",
                        column: x => x.STImportId,
                        principalTable: "STImports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WHImportDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllowUpdate = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    PhysicalCount = table.Column<int>(nullable: true),
                    STImportId = table.Column<int>(nullable: true),
                    SystemCount = table.Column<int>(nullable: true),
                    WHImportId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WHImportDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WHImportDetails_WHImports_WHImportId",
                        column: x => x.WHImportId,
                        principalTable: "WHImports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_STImportDetails_STImportId",
                table: "STImportDetails",
                column: "STImportId");

            migrationBuilder.CreateIndex(
                name: "IX_WHImportDetails_WHImportId",
                table: "WHImportDetails",
                column: "WHImportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STImportDetails");

            migrationBuilder.DropTable(
                name: "WHImportDetails");

            migrationBuilder.DropTable(
                name: "STImports");

            migrationBuilder.DropTable(
                name: "WHImports");
        }
    }
}

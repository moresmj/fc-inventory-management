using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedNewTablesSTReturnAndSTReturnDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "STReturns",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    FormNumber = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    RequestStatus = table.Column<int>(nullable: true),
                    StoreId = table.Column<int>(nullable: true),
                    WarehouseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STReturns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "STReturnDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BrokenQuantity = table.Column<int>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    GoodQuantity = table.Column<int>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    STReturnId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STReturnDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STReturnDetails_STReturns_STReturnId",
                        column: x => x.STReturnId,
                        principalTable: "STReturns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_STReturnDetails_STReturnId",
                table: "STReturnDetails",
                column: "STReturnId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STReturnDetails");

            migrationBuilder.DropTable(
                name: "STReturns");
        }
    }
}

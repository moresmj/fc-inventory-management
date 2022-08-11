using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedStoreSalesTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "STSales",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    Address3 = table.Column<string>(nullable: true),
                    ClientName = table.Column<string>(nullable: true),
                    ContactNumber = table.Column<string>(nullable: true),
                    DRNumber = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    ORNumber = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    SINumber = table.Column<string>(nullable: true),
                    STOrderId = table.Column<int>(nullable: true),
                    StoreId = table.Column<int>(nullable: true),
                    TransactionNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STSales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "STSalesDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    STSalesId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STSalesDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STSalesDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_STSalesDetails_STSales_STSalesId",
                        column: x => x.STSalesId,
                        principalTable: "STSales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_STSalesDetails_ItemId",
                table: "STSalesDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_STSalesDetails_STSalesId",
                table: "STSalesDetails",
                column: "STSalesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STSalesDetails");

            migrationBuilder.DropTable(
                name: "STSales");
        }
    }
}

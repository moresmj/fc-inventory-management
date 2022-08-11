using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedSTClientReturnTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "STClientReturns",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DeliveryStatus = table.Column<int>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    ReceivedRemarks = table.Column<string>(nullable: true),
                    ReleaseStatus = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    ReturnReason = table.Column<int>(nullable: true),
                    STReturnId = table.Column<int>(nullable: true),
                    STSalesDetailId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STClientReturns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STClientReturns_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_STClientReturns_STReturns_STReturnId",
                        column: x => x.STReturnId,
                        principalTable: "STReturns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_STClientReturns_ItemId",
                table: "STClientReturns",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_STClientReturns_STReturnId",
                table: "STClientReturns",
                column: "STReturnId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STClientReturns");
        }
    }
}

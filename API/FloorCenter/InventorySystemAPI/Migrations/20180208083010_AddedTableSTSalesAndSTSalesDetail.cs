using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class AddedTableSTSalesAndSTSalesDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "STSales",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Agent = table.Column<int>(nullable: true),
                    ContactNumber = table.Column<string>(nullable: true),
                    CustomerAddress1 = table.Column<string>(nullable: true),
                    CustomerAddress2 = table.Column<string>(nullable: true),
                    CustomerAddress3 = table.Column<string>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DeliveryType = table.Column<int>(nullable: true),
                    PaymentType = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    SIPODate = table.Column<DateTime>(type: "date", nullable: true),
                    SIPONumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STSales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "STSalesDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    OnHand = table.Column<int>(nullable: true),
                    STSalesId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STSalesDetail", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STSales");

            migrationBuilder.DropTable(
                name: "STSalesDetail");
        }
    }
}

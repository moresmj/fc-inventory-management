using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class AddedItemAttributeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemAttributeId",
                table: "Item",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemAttributeId1",
                table: "Item",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BreakageStrength = table.Column<string>(nullable: true),
                    CakeLayer = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Feature = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: true),
                    Material = table.Column<int>(nullable: false),
                    NoOfPattern = table.Column<int>(nullable: false),
                    PrintTech = table.Column<string>(nullable: true),
                    Purpose1 = table.Column<int>(nullable: true),
                    Purpose2 = table.Column<int>(nullable: true),
                    Rectified = table.Column<bool>(nullable: true),
                    SlipResistance = table.Column<string>(nullable: true),
                    SubType = table.Column<int>(nullable: false),
                    SurfaceFin = table.Column<int>(nullable: false),
                    Thickness = table.Column<string>(nullable: true),
                    Traffic = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    WaterAbs = table.Column<string>(nullable: true),
                    Weight = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAttribute", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_CategoryChildId",
                table: "Item",
                column: "CategoryChildId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_CategoryGrandChildId",
                table: "Item",
                column: "CategoryGrandChildId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_CategoryParentId",
                table: "Item",
                column: "CategoryParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_ItemAttributeId1",
                table: "Item",
                column: "ItemAttributeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Item_CategoryChild_CategoryChildId",
                table: "Item",
                column: "CategoryChildId",
                principalTable: "CategoryChild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Item_CategoryGrandChild_CategoryGrandChildId",
                table: "Item",
                column: "CategoryGrandChildId",
                principalTable: "CategoryGrandChild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Item_CategoryParent_CategoryParentId",
                table: "Item",
                column: "CategoryParentId",
                principalTable: "CategoryParent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Item_ItemAttribute_ItemAttributeId1",
                table: "Item",
                column: "ItemAttributeId1",
                principalTable: "ItemAttribute",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Item_CategoryChild_CategoryChildId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_Item_CategoryGrandChild_CategoryGrandChildId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_Item_CategoryParent_CategoryParentId",
                table: "Item");

            migrationBuilder.DropForeignKey(
                name: "FK_Item_ItemAttribute_ItemAttributeId1",
                table: "Item");

            migrationBuilder.DropTable(
                name: "ItemAttribute");

            migrationBuilder.DropIndex(
                name: "IX_Item_CategoryChildId",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_CategoryGrandChildId",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_CategoryParentId",
                table: "Item");

            migrationBuilder.DropIndex(
                name: "IX_Item_ItemAttributeId1",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "ItemAttributeId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "ItemAttributeId1",
                table: "Item");
        }
    }
}

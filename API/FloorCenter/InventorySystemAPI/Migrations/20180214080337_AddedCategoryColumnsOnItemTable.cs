using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace InventorySystemAPI.Migrations
{
    public partial class AddedCategoryColumnsOnItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryChildId",
                table: "Item",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryGrandChildId",
                table: "Item",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryParentId",
                table: "Item",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryGrandChild_CategoryChildId",
                table: "CategoryGrandChild",
                column: "CategoryChildId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryChild_CategoryParentId",
                table: "CategoryChild",
                column: "CategoryParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryChild_CategoryParent_CategoryParentId",
                table: "CategoryChild",
                column: "CategoryParentId",
                principalTable: "CategoryParent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryGrandChild_CategoryChild_CategoryChildId",
                table: "CategoryGrandChild",
                column: "CategoryChildId",
                principalTable: "CategoryChild",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryChild_CategoryParent_CategoryParentId",
                table: "CategoryChild");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryGrandChild_CategoryChild_CategoryChildId",
                table: "CategoryGrandChild");

            migrationBuilder.DropIndex(
                name: "IX_CategoryGrandChild_CategoryChildId",
                table: "CategoryGrandChild");

            migrationBuilder.DropIndex(
                name: "IX_CategoryChild_CategoryParentId",
                table: "CategoryChild");

            migrationBuilder.DropColumn(
                name: "CategoryChildId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "CategoryGrandChildId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "CategoryParentId",
                table: "Item");
        }
    }
}

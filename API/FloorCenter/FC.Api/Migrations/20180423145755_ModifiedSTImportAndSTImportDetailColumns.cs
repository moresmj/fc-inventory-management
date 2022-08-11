using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class ModifiedSTImportAndSTImportDetailColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "AllowUpdate",
                table: "STImportDetails",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.CreateIndex(
                name: "IX_STImports_StoreId",
                table: "STImports",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_STImportDetails_ItemId",
                table: "STImportDetails",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_STImportDetails_Items_ItemId",
                table: "STImportDetails",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_STImports_Stores_StoreId",
                table: "STImports",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_STImportDetails_Items_ItemId",
                table: "STImportDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_STImports_Stores_StoreId",
                table: "STImports");

            migrationBuilder.DropIndex(
                name: "IX_STImports_StoreId",
                table: "STImports");

            migrationBuilder.DropIndex(
                name: "IX_STImportDetails_ItemId",
                table: "STImportDetails");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowUpdate",
                table: "STImportDetails",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);
        }
    }
}

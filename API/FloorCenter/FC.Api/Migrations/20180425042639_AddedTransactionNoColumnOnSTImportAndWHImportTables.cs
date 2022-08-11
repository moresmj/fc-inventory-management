using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedTransactionNoColumnOnSTImportAndWHImportTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionNo",
                table: "WHImports",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionNo",
                table: "STImports",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionNo",
                table: "WHImports");

            migrationBuilder.DropColumn(
                name: "TransactionNo",
                table: "STImports");
        }
    }
}

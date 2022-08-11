using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class AddedIndexingOnInventoryContexts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WHStocks_ItemId_WarehouseId_OnHand_DeliveryStatus_ReleaseStatus",
                table: "WHStocks");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserName_StoreId_WarehouseId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_STStocks_ItemId_StoreId_OnHand_ReleaseStatus_DeliveryStatus",
                table: "STStocks");

            migrationBuilder.DropIndex(
                name: "IX_STOrders_StoreId_OrderType_TransactionNo_PONumber_WarehouseId",
                table: "STOrders");


            migrationBuilder.DropIndex(
                name: "IX_STDeliveries_STOrderId",
                table: "STDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_STDeliveries_StoreId_STOrderId_STSalesId",
                table: "STDeliveries");

            migrationBuilder.CreateIndex(
                name: "IX_WHStocks_ItemId",
                table: "WHStocks",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WHStocks_WarehouseId",
                table: "WHStocks",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_WHStocks_ItemId_STShowroomDeliveryId",
                table: "WHStocks",
                columns: new[] { "ItemId", "STShowroomDeliveryId" });

            migrationBuilder.CreateIndex(
                name: "IX_WHStocks_ItemId_STOrderDetailId_ReleaseStatus",
                table: "WHStocks",
                columns: new[] { "ItemId", "STOrderDetailId", "ReleaseStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_STTransferDetails_STTransferId_ItemId",
                table: "STTransferDetails",
                columns: new[] { "STTransferId", "ItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_STStocks_ItemId_StoreId",
                table: "STStocks",
                columns: new[] { "ItemId", "StoreId" });

            migrationBuilder.CreateIndex(
                name: "IX_STSalesDetails_STSalesId_ItemId",
                table: "STSalesDetails",
                columns: new[] { "STSalesId", "ItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_STOrders_Id",
                table: "STOrders",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_STOrders_OrderToStoreId",
                table: "STOrders",
                column: "OrderToStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_STOrders_PONumber",
                table: "STOrders",
                column: "PONumber");

            migrationBuilder.CreateIndex(
                name: "IX_STOrders_StoreId",
                table: "STOrders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_STOrders_WHDRNumber",
                table: "STOrders",
                column: "WHDRNumber");

            migrationBuilder.CreateIndex(
                name: "IX_STOrders_StoreId_TransactionType",
                table: "STOrders",
                columns: new[] { "StoreId", "TransactionType" });

            migrationBuilder.CreateIndex(
                name: "IX_STOrderDetails_STOrderId_ItemId",
                table: "STOrderDetails",
                columns: new[] { "STOrderId", "ItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_STImportDetails_STImportId_AllowUpdate",
                table: "STImportDetails",
                columns: new[] { "STImportId", "AllowUpdate" });

            migrationBuilder.CreateIndex(
                name: "IX_STDeliveries_Id",
                table: "STDeliveries",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_STDeliveries_DRNumber_DeliveryDate",
                table: "STDeliveries",
                columns: new[] { "DRNumber", "DeliveryDate" });

            migrationBuilder.CreateIndex(
                name: "IX_STDeliveries_STOrderId_Id",
                table: "STDeliveries",
                columns: new[] { "STOrderId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_STDeliveries_StoreId_Id",
                table: "STDeliveries",
                columns: new[] { "StoreId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_Code",
                table: "Items",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Id",
                table: "Items",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ImageName",
                table: "Items",
                column: "ImageName");

            migrationBuilder.CreateIndex(
                name: "IX_Items_SerialNumber",
                table: "Items",
                column: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Tonality",
                table: "Items",
                column: "Tonality");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Code_Tonality",
                table: "Items",
                columns: new[] { "Code", "Tonality" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_SerialNumber_Code",
                table: "Items",
                columns: new[] { "SerialNumber", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_SerialNumber_Tonality",
                table: "Items",
                columns: new[] { "SerialNumber", "Tonality" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WHStocks_ItemId",
                table: "WHStocks");

            migrationBuilder.DropIndex(
                name: "IX_WHStocks_WarehouseId",
                table: "WHStocks");

            migrationBuilder.DropIndex(
                name: "IX_WHStocks_ItemId_STShowroomDeliveryId",
                table: "WHStocks");

            migrationBuilder.DropIndex(
                name: "IX_WHStocks_ItemId_STOrderDetailId_ReleaseStatus",
                table: "WHStocks");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserName",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_STTransferDetails_STTransferId_ItemId",
                table: "STTransferDetails");

            migrationBuilder.DropIndex(
                name: "IX_STStocks_ItemId_StoreId",
                table: "STStocks");

            migrationBuilder.DropIndex(
                name: "IX_STSalesDetails_STSalesId_ItemId",
                table: "STSalesDetails");

            migrationBuilder.DropIndex(
                name: "IX_STOrders_Id",
                table: "STOrders");

            migrationBuilder.DropIndex(
                name: "IX_STOrders_OrderToStoreId",
                table: "STOrders");

            migrationBuilder.DropIndex(
                name: "IX_STOrders_PONumber",
                table: "STOrders");

            migrationBuilder.DropIndex(
                name: "IX_STOrders_StoreId",
                table: "STOrders");

            migrationBuilder.DropIndex(
                name: "IX_STOrders_WHDRNumber",
                table: "STOrders");

            migrationBuilder.DropIndex(
                name: "IX_STOrders_StoreId_TransactionType",
                table: "STOrders");

            migrationBuilder.DropIndex(
                name: "IX_STOrderDetails_STOrderId_ItemId",
                table: "STOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_STImportDetails_STImportId_AllowUpdate",
                table: "STImportDetails");

            migrationBuilder.DropIndex(
                name: "IX_STDeliveries_Id",
                table: "STDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_STDeliveries_DRNumber_DeliveryDate",
                table: "STDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_STDeliveries_STOrderId_Id",
                table: "STDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_STDeliveries_StoreId_Id",
                table: "STDeliveries");

            migrationBuilder.DropIndex(
                name: "IX_Items_Code",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_Id",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ImageName",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_SerialNumber",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_Tonality",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_Code_Tonality",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_SerialNumber_Code",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_SerialNumber_Tonality",
                table: "Items");

            migrationBuilder.CreateIndex(
                name: "IX_WHStocks_ItemId_WarehouseId_OnHand_DeliveryStatus_ReleaseStatus",
                table: "WHStocks",
                columns: new[] { "ItemId", "WarehouseId", "OnHand", "DeliveryStatus", "ReleaseStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName_StoreId_WarehouseId",
                table: "Users",
                columns: new[] { "UserName", "StoreId", "WarehouseId" });

            migrationBuilder.CreateIndex(
                name: "IX_STTransferDetails_STTransferId_ItemId_Quantity",
                table: "STTransferDetails",
                columns: new[] { "STTransferId", "ItemId", "Quantity" });

            migrationBuilder.CreateIndex(
                name: "IX_STStocks_ItemId_StoreId_OnHand_ReleaseStatus_DeliveryStatus",
                table: "STStocks",
                columns: new[] { "ItemId", "StoreId", "OnHand", "ReleaseStatus", "DeliveryStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_STShowroomDeliveries_STOrderDetailId_STDeliveryId_ItemId_ReleaseStatus_DeliveryStatus_Quantity",
                table: "STShowroomDeliveries",
                columns: new[] { "STOrderDetailId", "STDeliveryId", "ItemId", "ReleaseStatus", "DeliveryStatus", "Quantity" });

            migrationBuilder.CreateIndex(
                name: "IX_STSalesDetails_STSalesId_ItemId_DeliveryStatus_Quantity",
                table: "STSalesDetails",
                columns: new[] { "STSalesId", "ItemId", "DeliveryStatus", "Quantity" });

            migrationBuilder.CreateIndex(
                name: "IX_STOrders_StoreId_OrderType_TransactionNo_PONumber_WarehouseId",
                table: "STOrders",
                columns: new[] { "StoreId", "OrderType", "TransactionNo", "PONumber", "WarehouseId" });

            migrationBuilder.CreateIndex(
                name: "IX_STOrderDetails_STOrderId_ItemId_ReleaseStatus_DeliveryStatus_RequestedQuantity_ApprovedQuantity",
                table: "STOrderDetails",
                columns: new[] { "STOrderId", "ItemId", "ReleaseStatus", "DeliveryStatus", "RequestedQuantity", "ApprovedQuantity" });

            migrationBuilder.CreateIndex(
                name: "IX_STImportDetails_STImportId_AllowUpdate_PhysicalCount",
                table: "STImportDetails",
                columns: new[] { "STImportId", "AllowUpdate", "PhysicalCount" });

            migrationBuilder.CreateIndex(
                name: "IX_STDeliveries_STOrderId",
                table: "STDeliveries",
                column: "STOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_STDeliveries_StoreId_STOrderId_STSalesId",
                table: "STDeliveries",
                columns: new[] { "StoreId", "STOrderId", "STSalesId" });

            migrationBuilder.CreateIndex(
                name: "IX_STClientDeliveries_STOrderDetailId_STDeliveryId_ItemId_ReleaseStatus_DeliveryStatus_Quantity",
                table: "STClientDeliveries",
                columns: new[] { "STOrderDetailId", "STDeliveryId", "ItemId", "ReleaseStatus", "DeliveryStatus", "Quantity" });
        }
    }
}

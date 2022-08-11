﻿// <auto-generated />
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Inventory;
using InventorySystemAPI.Models.Store.Sales;
using InventorySystemAPI.Models.User;
using InventorySystemAPI.Models.Warehouse.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace InventorySystemAPI.Migrations
{
    [DbContext(typeof(FloorCenterContext))]
    [Migration("20180209082358_AddedColumnsDeliveryTypeDRNoDRDateDeliveryDateOnSTRelease")]
    partial class AddedColumnsDeliveryTypeDRNoDRDateDeliveryDateOnSTRelease
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("InventorySystemAPI.Models.Company.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Company");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Item.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<string>("Remarks");

                    b.Property<int?>("SerialNumber");

                    b.Property<int?>("SizeId");

                    b.Property<string>("Tonality");

                    b.HasKey("Id");

                    b.HasIndex("SizeId");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Size.Size", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Size");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Inventory.STInventory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<DateTime?>("PODate")
                        .HasColumnType("date");

                    b.Property<string>("PONumber");

                    b.Property<string>("Remarks");

                    b.Property<int?>("RequestStatus");

                    b.Property<int?>("StoreId");

                    b.Property<int?>("TransactionType");

                    b.Property<int?>("WarehouseId");

                    b.HasKey("Id");

                    b.ToTable("STInventory");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Inventory.STInventoryDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("ApprovedQuantity");

                    b.Property<string>("ApprovedRemarks");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int?>("DeliveryStatus");

                    b.Property<int?>("ItemId");

                    b.Property<int?>("RequestedQuantity");

                    b.Property<string>("RequestedRemarks");

                    b.Property<int?>("STInventoryId");

                    b.HasKey("Id");

                    b.HasIndex("STInventoryId");

                    b.ToTable("STInventoryDetail");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Releasing.STRelease", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DRDate")
                        .HasColumnType("date");

                    b.Property<string>("DRNo")
                        .HasColumnType("date");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateReleased")
                        .HasColumnType("date");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<DateTime?>("DeliveryDate")
                        .HasColumnType("date");

                    b.Property<int?>("DeliveryType");

                    b.HasKey("Id");

                    b.ToTable("STRelease");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Releasing.STReleaseDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int?>("Quantity");

                    b.Property<int?>("STReleaseId");

                    b.Property<int?>("STSalesDetailId");

                    b.HasKey("Id");

                    b.HasIndex("STReleaseId");

                    b.HasIndex("STSalesDetailId");

                    b.ToTable("STReleaseDetail");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Request.STDelivery", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<DateTime?>("DeliveryDate");

                    b.Property<int?>("ReceivedBy");

                    b.Property<int?>("STInventoryId");

                    b.Property<int?>("StoreId");

                    b.HasKey("Id");

                    b.HasIndex("STInventoryId");

                    b.ToTable("STDelivery");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Request.STDeliveryDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int?>("DeliveredQuantity");

                    b.Property<int?>("DeliveryStatus");

                    b.Property<int?>("ItemId");

                    b.Property<int?>("Quantity");

                    b.Property<string>("Remarks");

                    b.Property<int?>("STDeliveryId");

                    b.Property<int?>("STInventoryDetailId");

                    b.HasKey("Id");

                    b.HasIndex("STDeliveryId");

                    b.ToTable("STDeliveryDetail");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Sales.STSales", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("Agent");

                    b.Property<string>("ContactNumber");

                    b.Property<string>("CustomerAddress1");

                    b.Property<string>("CustomerAddress2");

                    b.Property<string>("CustomerAddress3");

                    b.Property<string>("CustomerName");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int?>("DeliveryType");

                    b.Property<int?>("PaymentType");

                    b.Property<string>("Remarks");

                    b.Property<DateTime?>("SIPODate")
                        .HasColumnType("date");

                    b.Property<string>("SIPONumber");

                    b.Property<int?>("StoreId");

                    b.HasKey("Id");

                    b.ToTable("STSales");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Sales.STSalesDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int?>("DeliveryStatus");

                    b.Property<int?>("ItemId");

                    b.Property<int?>("Quantity");

                    b.Property<int?>("STSalesId");

                    b.HasKey("Id");

                    b.HasIndex("STSalesId");

                    b.ToTable("STSalesDetail");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Stock.STStock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int?>("ItemId");

                    b.Property<int?>("OnHand");

                    b.Property<int?>("STDeliveryDetailId");

                    b.Property<int?>("STSalesDetailId");

                    b.HasKey("Id");

                    b.HasIndex("STDeliveryDetailId");

                    b.ToTable("STStock");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Store", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("Code");

                    b.Property<int?>("CompanyId");

                    b.Property<string>("ContactNumber");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<string>("Name");

                    b.Property<int?>("WarehouseId");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("WarehouseId");

                    b.ToTable("Store");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.User.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<int?>("Assignment");

                    b.Property<string>("ContactNumber");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<string>("EmailAddress");

                    b.Property<string>("FullName");

                    b.Property<string>("Password");

                    b.Property<int?>("StoreId");

                    b.Property<string>("UserName");

                    b.Property<int?>("UserType");

                    b.Property<int?>("WarehouseId");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Warehouse.Inventory.WHInventory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CheckedBy");

                    b.Property<DateTime?>("DRDate")
                        .HasColumnType("date");

                    b.Property<string>("DRNumber");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<DateTime?>("PODate")
                        .HasColumnType("date");

                    b.Property<string>("PONumber");

                    b.Property<DateTime?>("ReceivedDate");

                    b.Property<string>("Remarks");

                    b.Property<string>("TransactionId");

                    b.Property<int?>("WarehouseId");

                    b.HasKey("Id");

                    b.ToTable("WHInventory");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Warehouse.Inventory.WHInventoryDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int?>("ItemId");

                    b.Property<int?>("Quantity");

                    b.Property<string>("Remarks");

                    b.Property<int?>("WHInventoryId");

                    b.HasKey("Id");

                    b.HasIndex("WHInventoryId");

                    b.ToTable("WHInventoryDetail");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Warehouse.Stock.WHStock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int?>("DeliveryStatus");

                    b.Property<int?>("ItemId");

                    b.Property<int?>("OnHand");

                    b.Property<int?>("STDeliveryDetailId");

                    b.Property<int?>("TransactionType");

                    b.Property<int?>("WHInventoryDetailId");

                    b.HasKey("Id");

                    b.ToTable("WHStock");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Warehouse.Warehouse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("Code");

                    b.Property<string>("ContactNumber");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Warehouse");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Item.Item", b =>
                {
                    b.HasOne("InventorySystemAPI.Models.Size.Size", "Size")
                        .WithMany()
                        .HasForeignKey("SizeId");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Inventory.STInventoryDetail", b =>
                {
                    b.HasOne("InventorySystemAPI.Models.Store.Inventory.STInventory")
                        .WithMany("RequestedItems")
                        .HasForeignKey("STInventoryId");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Releasing.STReleaseDetail", b =>
                {
                    b.HasOne("InventorySystemAPI.Models.Store.Releasing.STRelease")
                        .WithMany("ReleasedItems")
                        .HasForeignKey("STReleaseId");

                    b.HasOne("InventorySystemAPI.Models.Store.Sales.STSalesDetail")
                        .WithMany("ReleasedItems")
                        .HasForeignKey("STSalesDetailId");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Request.STDelivery", b =>
                {
                    b.HasOne("InventorySystemAPI.Models.Store.Inventory.STInventory", "STInventory")
                        .WithMany()
                        .HasForeignKey("STInventoryId");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Request.STDeliveryDetail", b =>
                {
                    b.HasOne("InventorySystemAPI.Models.Store.Request.STDelivery", "STDelivery")
                        .WithMany("ItemsToBeDelivered")
                        .HasForeignKey("STDeliveryId");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Sales.STSalesDetail", b =>
                {
                    b.HasOne("InventorySystemAPI.Models.Store.Sales.STSales")
                        .WithMany("OrderedItems")
                        .HasForeignKey("STSalesId");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Stock.STStock", b =>
                {
                    b.HasOne("InventorySystemAPI.Models.Store.Request.STDeliveryDetail", "STDeliveryDetail")
                        .WithMany()
                        .HasForeignKey("STDeliveryDetailId");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Store.Store", b =>
                {
                    b.HasOne("InventorySystemAPI.Models.Company.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("InventorySystemAPI.Models.Warehouse.Warehouse", "Warehouse")
                        .WithMany()
                        .HasForeignKey("WarehouseId");
                });

            modelBuilder.Entity("InventorySystemAPI.Models.Warehouse.Inventory.WHInventoryDetail", b =>
                {
                    b.HasOne("InventorySystemAPI.Models.Warehouse.Inventory.WHInventory")
                        .WithMany("DeliveredItems")
                        .HasForeignKey("WHInventoryId");
                });
#pragma warning restore 612, 618
        }
    }
}

﻿// <auto-generated />
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Items;
using FC.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace FC.Api.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20180313102941_AddedWarehouseIdColumnOnWHStockTable")]
    partial class AddedWarehouseIdColumnOnWHStockTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FC.Core.Domain.Companies.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("FC.Core.Domain.Items.CategoryChild", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CategoryParentId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("CategoryParentId");

                    b.ToTable("CategoryChildren");
                });

            modelBuilder.Entity("FC.Core.Domain.Items.CategoryGrandChild", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CategoryChildId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("CategoryChildId");

                    b.ToTable("CategoryGrandChildren");
                });

            modelBuilder.Entity("FC.Core.Domain.Items.CategoryParent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("CategoryParents");
                });

            modelBuilder.Entity("FC.Core.Domain.Items.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CategoryChildId");

                    b.Property<int?>("CategoryGrandChildId");

                    b.Property<int?>("CategoryParentId");

                    b.Property<string>("Code");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<string>("Description");

                    b.Property<int?>("ItemAttributeId");

                    b.Property<string>("Name");

                    b.Property<string>("Remarks");

                    b.Property<decimal?>("SRP");

                    b.Property<int?>("SerialNumber");

                    b.Property<int?>("SizeId");

                    b.Property<string>("Tonality");

                    b.HasKey("Id");

                    b.HasIndex("CategoryChildId");

                    b.HasIndex("CategoryGrandChildId");

                    b.HasIndex("CategoryParentId");

                    b.HasIndex("ItemAttributeId");

                    b.HasIndex("SizeId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("FC.Core.Domain.Items.ItemAttribute", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BreakageStrength");

                    b.Property<int>("CakeLayer");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int>("Feature");

                    b.Property<int>("Material");

                    b.Property<int>("NoOfPattern");

                    b.Property<string>("PrintTech");

                    b.Property<int?>("Purpose1");

                    b.Property<int?>("Purpose2");

                    b.Property<bool?>("Rectified");

                    b.Property<string>("SlipResistance");

                    b.Property<int>("SubType");

                    b.Property<int>("SurfaceFin");

                    b.Property<string>("Thickness");

                    b.Property<int?>("Traffic");

                    b.Property<int>("Type");

                    b.Property<string>("WaterAbs");

                    b.Property<int>("Weight");

                    b.HasKey("Id");

                    b.ToTable("ItemAttributes");
                });

            modelBuilder.Entity("FC.Core.Domain.Sizes.Size", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Sizes");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.STDelivery", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ApprovedDeliveryDate");

                    b.Property<string>("DRNumber");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<DateTime?>("DeliveryDate");

                    b.Property<string>("DriverName");

                    b.Property<string>("PlateNumber");

                    b.Property<int?>("STOrderId");

                    b.Property<int?>("StoreId");

                    b.HasKey("Id");

                    b.HasIndex("STOrderId");

                    b.ToTable("STDeliveries");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.STOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address1");

                    b.Property<string>("Address2");

                    b.Property<string>("Address3");

                    b.Property<string>("ClientName");

                    b.Property<string>("ContactNumber");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int?>("DeliveryType");

                    b.Property<int?>("OrderType");

                    b.Property<DateTime?>("PODate")
                        .HasColumnType("date");

                    b.Property<string>("PONumber");

                    b.Property<string>("Remarks");

                    b.Property<int?>("RequestStatus");

                    b.Property<int?>("StoreId");

                    b.Property<string>("TransactionNo");

                    b.Property<int?>("TransactionType");

                    b.Property<int?>("WarehouseId");

                    b.HasKey("Id");

                    b.HasIndex("StoreId");

                    b.HasIndex("WarehouseId");

                    b.ToTable("STOrders");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.STOrderDetail", b =>
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

                    b.Property<int?>("STOrderId");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("STOrderId");

                    b.ToTable("STOrderDetails");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.Store", b =>
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

                    b.ToTable("Stores");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.STSales", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address1");

                    b.Property<string>("Address2");

                    b.Property<string>("Address3");

                    b.Property<string>("ClientName");

                    b.Property<string>("ContactNumber");

                    b.Property<string>("DRNumber");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<string>("ORNumber");

                    b.Property<string>("Remarks");

                    b.Property<string>("SINumber");

                    b.Property<int?>("STOrderId");

                    b.Property<int?>("StoreId");

                    b.Property<string>("TransactionNo");

                    b.HasKey("Id");

                    b.HasIndex("STOrderId");

                    b.ToTable("STSales");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.STSalesDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int?>("ItemId");

                    b.Property<int?>("Quantity");

                    b.Property<int?>("STSalesId");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("STSalesId");

                    b.ToTable("STSalesDetails");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.STShowroomDelivery", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int?>("DeliveredQuantity");

                    b.Property<int?>("DeliveryStatus");

                    b.Property<int?>("ItemId");

                    b.Property<int?>("Quantity");

                    b.Property<int?>("ReleaseStatus");

                    b.Property<string>("Remarks");

                    b.Property<int?>("STDeliveryId");

                    b.Property<int?>("STOrderDetailId");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("STDeliveryId");

                    b.ToTable("STShowroomDeliveries");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.STStock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int?>("ItemId");

                    b.Property<int?>("OnHand");

                    b.Property<int?>("STSalesDetailId");

                    b.Property<int?>("STShowroomDeliveryId");

                    b.Property<int?>("StoreId");

                    b.HasKey("Id");

                    b.ToTable("STStocks");
                });

            modelBuilder.Entity("FC.Core.Domain.Users.User", b =>
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

                    b.Property<byte[]>("PasswordHash");

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<int?>("StoreId");

                    b.Property<string>("UserName");

                    b.Property<int?>("UserType");

                    b.Property<int?>("WarehouseId");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FC.Core.Domain.Warehouses.Warehouse", b =>
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

                    b.ToTable("Warehouses");
                });

            modelBuilder.Entity("FC.Core.Domain.Warehouses.WHReceive", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

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

                    b.Property<string>("TransactionNo");

                    b.Property<int?>("UserId");

                    b.Property<int?>("WarehouseId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("WHReceives");
                });

            modelBuilder.Entity("FC.Core.Domain.Warehouses.WHReceiveDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int?>("ItemId");

                    b.Property<int?>("Quantity");

                    b.Property<string>("Remarks");

                    b.Property<int?>("WHReceiveId");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("WHReceiveId");

                    b.ToTable("WHReceiveDetails");
                });

            modelBuilder.Entity("FC.Core.Domain.Warehouses.WHStock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime?>("DateReleased");

                    b.Property<DateTime?>("DateUpdated");

                    b.Property<int?>("DeliveryStatus");

                    b.Property<int?>("ItemId");

                    b.Property<int?>("OnHand");

                    b.Property<int?>("ReleaseStatus");

                    b.Property<int?>("STOrderDetailId");

                    b.Property<int?>("TransactionType");

                    b.Property<int?>("WHReceiveDetailId");

                    b.Property<int?>("WarehouseId");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.ToTable("WHStocks");
                });

            modelBuilder.Entity("FC.Core.Domain.Items.CategoryChild", b =>
                {
                    b.HasOne("FC.Core.Domain.Items.CategoryParent")
                        .WithMany("Children")
                        .HasForeignKey("CategoryParentId");
                });

            modelBuilder.Entity("FC.Core.Domain.Items.CategoryGrandChild", b =>
                {
                    b.HasOne("FC.Core.Domain.Items.CategoryChild")
                        .WithMany("GrandChildren")
                        .HasForeignKey("CategoryChildId");
                });

            modelBuilder.Entity("FC.Core.Domain.Items.Item", b =>
                {
                    b.HasOne("FC.Core.Domain.Items.CategoryChild", "SubCategory")
                        .WithMany()
                        .HasForeignKey("CategoryChildId");

                    b.HasOne("FC.Core.Domain.Items.CategoryGrandChild", "SubSubCategory")
                        .WithMany()
                        .HasForeignKey("CategoryGrandChildId");

                    b.HasOne("FC.Core.Domain.Items.CategoryParent", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryParentId");

                    b.HasOne("FC.Core.Domain.Items.ItemAttribute", "ItemAttribute")
                        .WithMany()
                        .HasForeignKey("ItemAttributeId");

                    b.HasOne("FC.Core.Domain.Sizes.Size", "Size")
                        .WithMany()
                        .HasForeignKey("SizeId");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.STDelivery", b =>
                {
                    b.HasOne("FC.Core.Domain.Stores.STOrder")
                        .WithMany("Deliveries")
                        .HasForeignKey("STOrderId");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.STOrder", b =>
                {
                    b.HasOne("FC.Core.Domain.Stores.Store", "Store")
                        .WithMany()
                        .HasForeignKey("StoreId");

                    b.HasOne("FC.Core.Domain.Warehouses.Warehouse", "Warehouse")
                        .WithMany()
                        .HasForeignKey("WarehouseId");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.STOrderDetail", b =>
                {
                    b.HasOne("FC.Core.Domain.Items.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId");

                    b.HasOne("FC.Core.Domain.Stores.STOrder")
                        .WithMany("OrderedItems")
                        .HasForeignKey("STOrderId");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.Store", b =>
                {
                    b.HasOne("FC.Core.Domain.Companies.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("FC.Core.Domain.Warehouses.Warehouse", "Warehouse")
                        .WithMany()
                        .HasForeignKey("WarehouseId");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.STSales", b =>
                {
                    b.HasOne("FC.Core.Domain.Stores.STOrder", "Order")
                        .WithMany()
                        .HasForeignKey("STOrderId");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.STSalesDetail", b =>
                {
                    b.HasOne("FC.Core.Domain.Items.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId");

                    b.HasOne("FC.Core.Domain.Stores.STSales")
                        .WithMany("SoldItems")
                        .HasForeignKey("STSalesId");
                });

            modelBuilder.Entity("FC.Core.Domain.Stores.STShowroomDelivery", b =>
                {
                    b.HasOne("FC.Core.Domain.Items.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId");

                    b.HasOne("FC.Core.Domain.Stores.STDelivery")
                        .WithMany("ShowroomDeliveries")
                        .HasForeignKey("STDeliveryId");
                });

            modelBuilder.Entity("FC.Core.Domain.Warehouses.WHReceive", b =>
                {
                    b.HasOne("FC.Core.Domain.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("FC.Core.Domain.Warehouses.WHReceiveDetail", b =>
                {
                    b.HasOne("FC.Core.Domain.Items.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId");

                    b.HasOne("FC.Core.Domain.Warehouses.WHReceive")
                        .WithMany("ReceivedItems")
                        .HasForeignKey("WHReceiveId");
                });

            modelBuilder.Entity("FC.Core.Domain.Warehouses.WHStock", b =>
                {
                    b.HasOne("FC.Core.Domain.Items.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId");
                });
#pragma warning restore 612, 618
        }
    }
}

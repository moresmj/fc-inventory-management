using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FC.Api.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryParents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryParents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BreakageStrength = table.Column<string>(nullable: true),
                    CakeLayer = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Feature = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_ItemAttributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sizes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sizes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "STStocks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    OnHand = table.Column<int>(nullable: true),
                    STShowroomDeliveryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STStocks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    Assignment = table.Column<int>(nullable: true),
                    ContactNumber = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true),
                    StoreId = table.Column<int>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    UserType = table.Column<int>(nullable: true),
                    WarehouseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    ContactNumber = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryChildren",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryParentId = table.Column<int>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryChildren", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryChildren_CategoryParents_CategoryParentId",
                        column: x => x.CategoryParentId,
                        principalTable: "CategoryParents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WHReceives",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DRDate = table.Column<DateTime>(type: "date", nullable: true),
                    DRNumber = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    PODate = table.Column<DateTime>(type: "date", nullable: true),
                    PONumber = table.Column<string>(nullable: true),
                    ReceivedDate = table.Column<DateTime>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    TransactionNo = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    WarehouseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WHReceives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WHReceives_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    CompanyId = table.Column<int>(nullable: true),
                    ContactNumber = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    WarehouseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stores_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stores_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CategoryGrandChildren",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryChildId = table.Column<int>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryGrandChildren", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryGrandChildren_CategoryChildren_CategoryChildId",
                        column: x => x.CategoryChildId,
                        principalTable: "CategoryChildren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "STOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    PODate = table.Column<DateTime>(type: "date", nullable: true),
                    PONumber = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    RequestStatus = table.Column<int>(nullable: true),
                    StoreId = table.Column<int>(nullable: true),
                    TransactionNo = table.Column<string>(nullable: true),
                    TransactionType = table.Column<int>(nullable: true),
                    WarehouseId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STOrders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_STOrders_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryChildId = table.Column<int>(nullable: true),
                    CategoryGrandChildId = table.Column<int>(nullable: true),
                    CategoryParentId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ItemAttributeId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    SRP = table.Column<decimal>(nullable: true),
                    SerialNumber = table.Column<int>(nullable: true),
                    SizeId = table.Column<int>(nullable: true),
                    Tonality = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_CategoryChildren_CategoryChildId",
                        column: x => x.CategoryChildId,
                        principalTable: "CategoryChildren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_CategoryGrandChildren_CategoryGrandChildId",
                        column: x => x.CategoryGrandChildId,
                        principalTable: "CategoryGrandChildren",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_CategoryParents_CategoryParentId",
                        column: x => x.CategoryParentId,
                        principalTable: "CategoryParents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_ItemAttributes_ItemAttributeId",
                        column: x => x.ItemAttributeId,
                        principalTable: "ItemAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Sizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Sizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "STDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApprovedDeliveryDate = table.Column<DateTime>(nullable: true),
                    DRNumber = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DeliveryDate = table.Column<DateTime>(nullable: true),
                    DeliveryType = table.Column<int>(nullable: true),
                    DriverName = table.Column<string>(nullable: true),
                    PlateNumber = table.Column<string>(nullable: true),
                    STOrderId = table.Column<int>(nullable: true),
                    StoreId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STDeliveries_STOrders_STOrderId",
                        column: x => x.STOrderId,
                        principalTable: "STOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "STOrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApprovedQuantity = table.Column<int>(nullable: true),
                    ApprovedRemarks = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DeliveryStatus = table.Column<int>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    RequestedQuantity = table.Column<int>(nullable: true),
                    RequestedRemarks = table.Column<string>(nullable: true),
                    STOrderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STOrderDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_STOrderDetails_STOrders_STOrderId",
                        column: x => x.STOrderId,
                        principalTable: "STOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WHReceiveDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    WHReceiveId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WHReceiveDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WHReceiveDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WHReceiveDetails_WHReceives_WHReceiveId",
                        column: x => x.WHReceiveId,
                        principalTable: "WHReceives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WHStocks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateReleased = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DeliveryStatus = table.Column<int>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    OnHand = table.Column<int>(nullable: true),
                    ReleaseStatus = table.Column<int>(nullable: true),
                    STOrderDetailId = table.Column<int>(nullable: true),
                    TransactionType = table.Column<int>(nullable: true),
                    WHReceiveDetailId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WHStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WHStocks_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "STShowroomDeliveries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    DeliveredQuantity = table.Column<int>(nullable: true),
                    DeliveryStatus = table.Column<int>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    Quantity = table.Column<int>(nullable: true),
                    ReleaseStatus = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    STDeliveryId = table.Column<int>(nullable: true),
                    STOrderDetailId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STShowroomDeliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_STShowroomDeliveries_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_STShowroomDeliveries_STDeliveries_STDeliveryId",
                        column: x => x.STDeliveryId,
                        principalTable: "STDeliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryChildren_CategoryParentId",
                table: "CategoryChildren",
                column: "CategoryParentId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryGrandChildren_CategoryChildId",
                table: "CategoryGrandChildren",
                column: "CategoryChildId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryChildId",
                table: "Items",
                column: "CategoryChildId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryGrandChildId",
                table: "Items",
                column: "CategoryGrandChildId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CategoryParentId",
                table: "Items",
                column: "CategoryParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemAttributeId",
                table: "Items",
                column: "ItemAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_SizeId",
                table: "Items",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_STDeliveries_STOrderId",
                table: "STDeliveries",
                column: "STOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_STOrderDetails_ItemId",
                table: "STOrderDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_STOrderDetails_STOrderId",
                table: "STOrderDetails",
                column: "STOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_STOrders_StoreId",
                table: "STOrders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_STOrders_WarehouseId",
                table: "STOrders",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_CompanyId",
                table: "Stores",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_WarehouseId",
                table: "Stores",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_STShowroomDeliveries_ItemId",
                table: "STShowroomDeliveries",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_STShowroomDeliveries_STDeliveryId",
                table: "STShowroomDeliveries",
                column: "STDeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_WHReceiveDetails_ItemId",
                table: "WHReceiveDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WHReceiveDetails_WHReceiveId",
                table: "WHReceiveDetails",
                column: "WHReceiveId");

            migrationBuilder.CreateIndex(
                name: "IX_WHReceives_UserId",
                table: "WHReceives",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WHStocks_ItemId",
                table: "WHStocks",
                column: "ItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "STOrderDetails");

            migrationBuilder.DropTable(
                name: "STShowroomDeliveries");

            migrationBuilder.DropTable(
                name: "STStocks");

            migrationBuilder.DropTable(
                name: "WHReceiveDetails");

            migrationBuilder.DropTable(
                name: "WHStocks");

            migrationBuilder.DropTable(
                name: "STDeliveries");

            migrationBuilder.DropTable(
                name: "WHReceives");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "STOrders");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "CategoryGrandChildren");

            migrationBuilder.DropTable(
                name: "ItemAttributes");

            migrationBuilder.DropTable(
                name: "Sizes");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "CategoryChildren");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "CategoryParents");
        }
    }
}

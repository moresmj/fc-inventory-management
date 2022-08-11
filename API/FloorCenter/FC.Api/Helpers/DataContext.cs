using FC.Core.Domain.Companies;
using FC.Core.Domain.Items;
using FC.Core.Domain.Sizes;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using FC.Core.Domain.Warehouses;
using FC.Core.Domain.UserTrail;
using Microsoft.EntityFrameworkCore;
using System;
using FC.Core.Domain.Views;
using System.ComponentModel.DataAnnotations.Schema;
using FC.Core.Domain.Views.Notification;
using FC.Core.Domain.Views.Notification.Store;

namespace FC.Api.Helpers
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Store> Stores { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<ItemAttribute> ItemAttributes { get; set; }

        public DbSet<Size> Sizes { get; set; }

        public DbSet<CategoryParent> CategoryParents { get; set; }

        public DbSet<CategoryChild> CategoryChildren { get; set; }

        public DbSet<CategoryGrandChild> CategoryGrandChildren { get; set; }

        public DbSet<WHReceive> WHReceives { get; set; }

        public DbSet<WHReceiveDetail> WHReceiveDetails { get; set; }

        public DbSet<WHStock> WHStocks { get; set; }

        public DbSet<STOrder> STOrders { get; set; }

        public DbSet<STOrderDetail> STOrderDetails { get; set; }

        public DbSet<STDelivery> STDeliveries { get; set; }

        public DbSet<STShowroomDelivery> STShowroomDeliveries { get; set; }

        public DbSet<STStock> STStocks { get; set; }

        public DbSet<STSales> STSales { get; set; }

        public DbSet<STSalesDetail> STSalesDetails { get; set; }

        public DbSet<STClientDelivery> STClientDeliveries { get; set; }

        public DbSet<STTransfer> STTransfers { get; set; }

        public DbSet<STTransferDetail> STTransferDetails { get; set; }

        public DbSet<STReturn> STReturns { get; set; }

        public DbSet<STPurchaseReturn> STPurchaseReturns { get; set; }

        public DbSet<WHDelivery> WHDeliveries { get; set; }

        public DbSet<WHDeliveryDetail> WHDeliveryDetails { get; set; }

        public DbSet<STClientReturn> STClientReturns { get; set; }

        public DbSet<WHImport> WHImports { get; set; }

        public DbSet<WHImportDetail> WHImportDetails { get; set; }

        public DbSet<STImport> STImports { get; set; }

        public DbSet<STImportDetail> STImportDetails { get; set; }

        public DbSet<UserTrail> UserTrails { get; set; }

        public DbSet<STStockSummary> STStockSummary { get; set; }

        public DbSet<WHStockSummary> WHStockSummary { get; set; }

        
        public DbSet<CustomQuantity> CustomQuantity { get; set; }

        
        public DbSet<ApproveTransferView> ApproveTransferView { get; set; }

        
        public DbSet<BranchOrderView> BranchOrderViews { get; set; }

        public DbSet<WHModifyItemTonality> WHModifyItemTonality { get; set; }

        public DbSet<WHModifyItemTonalityDetails> WHModifyItemTonalityDetails { get; set; }
        
        public DbSet<StoreDealerAssignment> StoreDealerAssignment { get; set; }

        public DbSet<STAdvanceOrder> STAdvanceOrder { get; set; }

        public DbSet<STAdvanceOrderDetails> STAdvanceOrderDetails { get; set; }

        public DbSet<WHAllocateAdvanceOrder> WHAllocateAdvanceOrder { get; set; }

        public DbSet<WHAllocateAdvanceOrderDetail> WHAllocateAdvanceOrderDetail{ get; set; }

        public DbSet<WarehouseNotifSummary> WarehouseNotifSummary { get; set; }

        public DbSet<MainNotifSummaryView> MainNotifSummary { get; set; }

        public DbSet<MainDashboardSummaryView> MainDashboardSummary { get; set; }

        public DbSet<StoreDashboardSummaryView> StoreDashboardSummary { get; set; }

        public DbSet<LogisticsNotifSummaryView> LogisticsNotifSummary { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Item

            modelBuilder.Entity<Item>()
                .HasIndex(p => new { p.Id });

            modelBuilder.Entity<Item>()
                .HasIndex(p => new { p.SerialNumber });

            modelBuilder.Entity<Item>()
                .HasIndex(p => new { p.Code });

            modelBuilder.Entity<Item>()
              .HasIndex(p => new { p.Tonality });

            modelBuilder.Entity<Item>()
              .HasIndex(p => new { p.ImageName });

            modelBuilder.Entity<Item>()
                .HasIndex(p => new { p.SerialNumber, p.Code });

            modelBuilder.Entity<Item>()
                .HasIndex(p => new { p.SerialNumber, p.Tonality });

            modelBuilder.Entity<Item>()
                .HasIndex(p => new { p.Code, p.Tonality });

            modelBuilder.Entity<Item>()
                .HasIndex(p => new { p.SerialNumber, p.Code, p.Tonality });

            modelBuilder.Entity<Item>()
                .Property(p => p.IsActive)
                .HasDefaultValue(true);

            #endregion

            #region WHStock

            modelBuilder.Entity<WHStock>()
                .HasIndex(p => new { p.ItemId });

            modelBuilder.Entity<WHStock>()
                .HasIndex(p => new { p.STOrderDetailId });

            modelBuilder.Entity<WHStock>()
                .HasIndex(p => new { p.WarehouseId });

            modelBuilder.Entity<WHStock>()
                .HasIndex(p => new { p.STShowroomDeliveryId });

            modelBuilder.Entity<WHStock>()
                .HasIndex(p => new { p.STClientDeliveryId });

            modelBuilder.Entity<WHStock>()
                .HasIndex(p => new { p.ItemId, p.STShowroomDeliveryId });

            modelBuilder.Entity<WHStock>()
                .HasIndex(p => new { p.ItemId, p.STOrderDetailId, p.ReleaseStatus });


            #endregion

            #region STStock

            modelBuilder.Entity<STStock>()
                .HasIndex(p => new { p.StoreId });

            modelBuilder.Entity<STStock>()
                .HasIndex(p => new { p.STShowroomDeliveryId });

            modelBuilder.Entity<STStock>()
                .HasIndex(p => new { p.STClientDeliveryId });

            modelBuilder.Entity<STStock>()
                .HasIndex(p => new { p.ItemId, p.StoreId });

            #endregion

            #region STOrder

            modelBuilder.Entity<STOrder>()
                .HasIndex(p => new { p.Id });

            modelBuilder.Entity<STOrder>()
                .HasIndex(p => new { p.OrderToStoreId });

            modelBuilder.Entity<STOrder>()
                .HasIndex(p => new { p.StoreId });

            modelBuilder.Entity<STOrder>()
                .HasIndex(p => new { p.PONumber });

            modelBuilder.Entity<STOrder>()
                .HasIndex(p => new { p.WHDRNumber });

            modelBuilder.Entity<STOrder>()
                .HasIndex(p => new { p.StoreId, p.TransactionType });


            #endregion

            modelBuilder.Entity<STOrderDetail>()
                .HasIndex(p => new { p.STOrderId, p.ItemId });

            modelBuilder.Entity<STSales>()
                .HasIndex(p => new { p.STOrderId, p.OrderToStoreId, p.DeliveryType, p.TransactionNo });

            modelBuilder.Entity<STSalesDetail>()
               .HasIndex(p => new { p.STSalesId, p.ItemId });

            modelBuilder.Entity<User>()
                .HasIndex(p => new { p.UserName });

            modelBuilder.Entity<Store>()
                .HasIndex(p => new { p.Code, p.Name, p.CompanyId });

            modelBuilder.Entity<Warehouse>()
                .HasIndex(p => new { p.Vendor, p.Name, p.Code });

            #region STDelivery

            modelBuilder.Entity<STDelivery>()
                .HasIndex(p => new { p.StoreId, p.Id });

            modelBuilder.Entity<STDelivery>()
                .HasIndex(p => new { p.Id });

            modelBuilder.Entity<STDelivery>()
                .HasIndex(p => new { p.DRNumber, p.DeliveryDate });

            modelBuilder.Entity<STDelivery>()
                .HasIndex(p => new { p.STOrderId, p.Id });

            #endregion

            modelBuilder.Entity<STImport>()
                .HasIndex(p => new { p.StoreId, p.RequestStatus });

            modelBuilder.Entity<STImportDetail>()
                .HasIndex(p => new { p.STImportId, p.AllowUpdate });

            modelBuilder.Entity<STTransfer>()
                .HasIndex(p => new { p.StoreId });

            modelBuilder.Entity<STTransferDetail>()
                .HasIndex(p => new { p.STTransferId, p.ItemId });

            #region Views please uncomment when creating new migration

            //modelBuilder.Ignore<CustomQuantity>();

            //modelBuilder.Ignore<ApproveTransferView>();

            //modelBuilder.Ignore<BranchOrderView>();

            //modelBuilder.Ignore<StoreDealerAssignment>();

            //modelBuilder.Ignore<WarehouseNotifSummary>();

            //modelBuilder.Ignore<MainNotifSummaryView>();

            //modelBuilder.Ignore<MainDashboardSummaryView>();

            //modelBuilder.Ignore<StoreDashboardSummaryView>();

            //modelBuilder.Ignore<LogisticsNotifSummaryView>();

            #endregion


            modelBuilder.Entity<StoreDealerAssignment>()
                .HasIndex(p => new { p.StoreId, p.UserId });

            modelBuilder.Entity<WHModifyItemTonality>()
                .HasIndex(p => new { p.WarehouseId, p.STOrderId });

            modelBuilder.Entity<WHModifyItemTonalityDetails>()
                .HasIndex(p => new { p.WHModifyItemTonalityId, p.ItemId });

            modelBuilder.Entity<STAdvanceOrder>()
                .HasIndex(p => new { p.WarehouseId, p.StoreId });

            modelBuilder.Entity<STAdvanceOrderDetails>()
                .HasIndex(p => new {  p.ItemId, p.STAdvanceOrderId });


            modelBuilder.Entity<WHAllocateAdvanceOrder>();

            modelBuilder.Entity<WHAllocateAdvanceOrderDetail>();

            




        }

    }
}

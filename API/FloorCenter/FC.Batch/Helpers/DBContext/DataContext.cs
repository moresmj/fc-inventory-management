using FC.Core.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FC.Batch.Helpers.DBContext.Extension;
using FC.Core.Domain.Stores;
using FC.Batch.DTOs.Store;
using FC.Batch.DTOs.Warehouse;
using FC.Core.Domain.Items;
using FC.Core.Domain.Sizes;

namespace FC.Batch.Helpers.DBContext
{
    public class DataContext
        : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        public DbSet<Item> Item { get; set; }

        public DbSet<Size> Size { get; set; }

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

        public DbSet<STStockSummary> STStockSummary { get; set; }

        public DbSet<WHStockSummary> WHStockSummary { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region WHStock
            modelBuilder.WHStockModelCreating();
            #endregion

            #region STStock
            modelBuilder.STStockModelCreating();
            #endregion

            modelBuilder.STOrderDetailsModelCreating();
        }
    }
}

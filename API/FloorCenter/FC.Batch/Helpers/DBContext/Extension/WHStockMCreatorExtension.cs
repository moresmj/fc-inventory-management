using FC.Core.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Batch.Helpers.DBContext.Extension
{
    internal static class WHStockMCreatorExtension
    {
        public static void WHStockModelCreating(this ModelBuilder modelBuilder)
        {
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
        }

        public static IEnumerable<object> FindAll(this DbSet<WHStock> entity)
        {
            return entity.Include(p => p.STOrderDetail).ToList();
        }
    }
}

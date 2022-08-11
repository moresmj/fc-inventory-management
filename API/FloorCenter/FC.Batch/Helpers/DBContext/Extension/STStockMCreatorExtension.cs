using FC.Core.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Batch.Helpers.DBContext.Extension
{
    internal static class STStockMCreatorExtension
    {
        public static void STStockModelCreating(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<STStock>()
                .HasIndex(p => new { p.StoreId });

            modelBuilder.Entity<STStock>()
                .HasIndex(p => new { p.STShowroomDeliveryId });

            modelBuilder.Entity<STStock>()
                .HasIndex(p => new { p.STClientDeliveryId });

            modelBuilder.Entity<STStock>()
                .HasIndex(p => new { p.ItemId, p.StoreId });
        }
    }
}

using FC.Core.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Batch.Helpers.DBContext.Extension
{
    internal static class STOrderDetailsMCreatorExtension
    {
        public static void STOrderDetailsModelCreating(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<STOrderDetail>()
                .HasIndex(p => new { p.STOrderId, p.ItemId });
        }
    }
}

using FC.Core.Domain.Items;
using FC.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace FC.Core.Domain.Warehouses
{
    public class WHStockSummary : BaseEntity
    {
        public int? WarehouseId { get; set; }
        public int? ItemId { get; set; }
        public string SerialNumber { get; set; }
        public string Code { get; set; }
        public string ItemName { get; set; }
        public int? SizeId { get; set; }
        public string SizeName { get; set; }
        public string Tonality { get; set; }
        public string Description { get; set; }
        public int OnHand { get; set; }
        public int ForRelease { get; set; }
        public int Available { get; set; }
        public int Broken { get; set; }
        public int Reserved { get; set; }

        #region Navigation Properties

        public Warehouse Warehouses { get; set; }

        public Item Items { get; set; }

        #endregion

    }
}

using FC.Core.Domain.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace FC.Core.Domain.Stores
{
    public class STStockSummary : BaseEntity
    {
        public int? StoreId { get; set; }
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


        #region Navigation Properties

        public Item Items { get; set; }

        public Store Stores { get; set; }

        #endregion

    }
}

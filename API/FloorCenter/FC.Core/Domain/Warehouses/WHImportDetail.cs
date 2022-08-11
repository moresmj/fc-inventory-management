using FC.Core.Domain.Items;

namespace FC.Core.Domain.Warehouses
{
    public class WHImportDetail : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// STImport.Id
        /// </summary>
        public int? WHImportId { get; set; }

        /// <summary>
        /// Item.Id
        /// </summary>
        public int? ItemId { get; set; }

        #endregion

        public int? SystemCount { get; set; }

        public int? PhysicalCount { get; set; }

        public int? ReserveAdjustment { get; set; }
        
        public bool? AllowUpdate { get; set; }

        #region Navigation Properties

        public Item Item { get; set; }

        public int? Broken { get; set; }

        public string Remarks { get; set; }

        #endregion

    }
}

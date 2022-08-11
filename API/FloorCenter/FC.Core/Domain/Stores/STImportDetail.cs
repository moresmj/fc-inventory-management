using FC.Core.Domain.Items;

namespace FC.Core.Domain.Stores
{
    public class STImportDetail : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// STImport.Id
        /// </summary>
        public int? STImportId { get; set; }

        /// <summary>
        /// Item.Id
        /// </summary>
        public int? ItemId { get; set; }

        #endregion

        public int? SystemCount { get; set; }

        public int? PhysicalCount { get; set; }

        public bool? AllowUpdate { get; set; }

        public int? Broken { get; set; }

        public string Remarks { get; set; }

        #region Navigation Properties

        public Item Item { get; set; }

        #endregion

    }
}

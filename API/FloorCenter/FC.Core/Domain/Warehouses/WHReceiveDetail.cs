using FC.Core.Domain.Items;

namespace FC.Core.Domain.Warehouses
{
    public class WHReceiveDetail : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// WHRecieve.Id
        /// </summary>
        public int? WHReceiveId { get; set; }

        /// <summary>
        /// Item.Id
        /// </summary>
        public int? ItemId { get; set; }

        #endregion

        public int? Quantity { get; set; }

        public int? ReservedQuantity { get; set; }

        public string Remarks { get; set; }

        #region Navigation Properties

        public Item Item { get; set; }

        #endregion

    }
}

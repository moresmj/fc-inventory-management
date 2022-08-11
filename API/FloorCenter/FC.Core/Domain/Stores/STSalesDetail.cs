using FC.Core.Domain.Common;
using FC.Core.Domain.Items;

namespace FC.Core.Domain.Stores
{
    public class STSalesDetail : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// STSales.Id
        /// </summary>
        public int? STSalesId { get; set; }

        /// <summary>
        /// Item.Id
        /// </summary>
        public int? ItemId { get; set; }

        #endregion

        public int? Quantity { get; set; }
       

        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        #region Navigation Properties

        public virtual Item Item { get; set; }

        #endregion

    }
}

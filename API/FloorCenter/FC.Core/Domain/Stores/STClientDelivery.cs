using FC.Core.Domain.Common;
using FC.Core.Domain.Items;

namespace FC.Core.Domain.Stores
{
    public class STClientDelivery : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// STDelivery.Id
        /// </summary>
        public int? STDeliveryId { get; set; }

        /// <summary>
        /// STOrderDetail.Id
        /// </summary>
        public int? STOrderDetailId { get; set; }

        /// <summary>
        /// STSalesDetail.Id
        /// </summary>
        public int? STSalesDetailId { get; set; }

        /// <summary>
        /// Item.Id
        /// </summary>
        public int? ItemId { get; set; }

        #endregion

        public int? Quantity { get; set; }

        public int? DeliveredQuantity { get; set; }

        public string Remarks { get; set; }

        public DeliveryStatusEnum? DeliveryStatus { get; set; }
        
        public ReleaseStatusEnum? ReleaseStatus { get; set; }

        public bool? isTonalityAny { get; set; }

        #region Navigation Properties

        public Item Item { get; set; }

        #endregion

    }
}

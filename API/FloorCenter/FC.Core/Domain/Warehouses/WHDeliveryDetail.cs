using FC.Core.Domain.Common;
using FC.Core.Domain.Items;

namespace FC.Core.Domain.Warehouses
{
    public class WHDeliveryDetail : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// WHDelivery.Id
        /// </summary>
        public int? WHDeliveryId { get; set; }

        /// <summary>
        /// STPurhcaseReturn.Id
        /// </summary>
        public int? STPurchaseReturnId { get; set; }

        /// <summary>
        /// Item.Id
        /// </summary>
        public int? ItemId { get; set; }

        #endregion

        public int? Quantity { get; set; }

        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        public ReleaseStatusEnum ReleaseStatus { get; set; }

        public int? ReceivedGoodQuantity { get; set; }

        public int? ReceivedBrokenQuantity { get; set; }

        public string ReceivedRemarks { get; set; }

        public bool? isTonalityAny { get; set; }

        #region Navigation Properties

        public Item Item { get; set; }

        #endregion

    }
}

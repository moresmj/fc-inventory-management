using FC.Core.Domain.Common;
using FC.Core.Domain.Items;

namespace FC.Core.Domain.Stores
{
    public class STPurchaseReturn : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// STReturn.Id
        /// </summary>
        public int? STReturnId { get; set; }

        /// <summary>
        /// Item.Id
        /// </summary>
        public int? ItemId { get; set; }

        #endregion

        public int? GoodQuantity { get; set; }

        public int? BrokenQuantity { get; set; }

        public int? ActualBrokenQuantity { get; set; }

        public ReturnReasonEnum? ReturnReason { get; set; }

        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        public ReleaseStatusEnum? ReleaseStatus { get; set; }

        public string Remarks { get; set; }

        public bool? isTonalityAny { get; set; }
        #region Navigation Properties

        public Item Item { get; set; }

        #endregion

    }
}

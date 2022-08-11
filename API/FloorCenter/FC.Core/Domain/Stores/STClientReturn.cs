using FC.Core.Domain.Common;
using FC.Core.Domain.Items;

namespace FC.Core.Domain.Stores
{
    public class STClientReturn : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// STReturn.Id
        /// </summary>
        public int? STReturnId { get; set; }

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

        public ReturnReasonEnum? ReturnReason { get; set; }

        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        public ReleaseStatusEnum? ReleaseStatus { get; set; }

        public string Remarks { get; set; }

        public string ReceivedRemarks { get; set; }

        public bool? isTonalityAny { get; set; }


        #region Navigation Properties

        public Item Item { get; set; }

        #endregion

    }
}

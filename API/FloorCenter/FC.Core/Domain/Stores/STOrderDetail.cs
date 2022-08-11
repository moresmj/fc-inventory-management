using FC.Core.Domain.Common;
using FC.Core.Domain.Items;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FC.Core.Domain.Stores
{
    public class STOrderDetail : BaseEntity
    {

        #region Foreign Keys

        /// <summary>
        /// STOrder.Id
        /// </summary>
        public int? STOrderId { get; set; }

        /// <summary>
        /// Item.Id
        /// </summary>
        public int? ItemId { get; set; }

        #endregion

        public int? RequestedQuantity { get; set; }

        public string RequestedRemarks { get; set; }

        public int? ApprovedQuantity { get; set; }

        public string ApprovedRemarks { get; set; }

        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        public ReleaseStatusEnum? ReleaseStatus { get; set; }

        public bool? isTonalityAny { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateReleased { get; set; }

        #region Navigation Properties

        public virtual Item Item { get; set; }

        #endregion

    }
}

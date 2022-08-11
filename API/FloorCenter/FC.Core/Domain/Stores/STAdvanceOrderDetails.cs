using FC.Core.Domain.Common;
using FC.Core.Domain.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace FC.Core.Domain.Stores
{
    public class STAdvanceOrderDetails : BaseEntity
    {

        public int? STAdvanceOrderId { get; set; }

        public int? ItemId { get; set; }

        public int? Quantity { get; set; }

        public int? ApprovedQuantity { get; set; }

        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        //public ReleaseStatusEnum? ReleaseStatus { get; set; }

        public string Remarks { get; set; }

        public string Code { get; set; }

        public int? sizeId { get; set; }

        public string tonality { get; set; }

        public bool? isCustom { get; set; }

        #region Navigation Properties

        public virtual Item Item { get; set; }

        #endregion
    }
}

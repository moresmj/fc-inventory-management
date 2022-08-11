using FC.Core.Domain.Common;
using FC.Core.Domain.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace FC.Core.Domain.Warehouses
{
    public class WHModifyItemTonalityDetails : BaseEntity
    {

        public int? WHModifyItemTonalityId { get; set; }

        public int? ItemId { get; set; }

        public int? OldItemId { get; set; }

        public int? StClientDeliveryId { get; set; }

        public int? StShowroomDeliveryId { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }

        public string Remarks { get; set; }


        public Item Item { get; set; }
    }
}

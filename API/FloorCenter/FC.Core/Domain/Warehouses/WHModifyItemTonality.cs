using FC.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FC.Core.Domain.Warehouses
{
    public class WHModifyItemTonality : BaseEntity
    {

        public int? WarehouseId { get; set; }

        public int? STOrderId { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }

        public string Remarks { get; set; }

        public DateTime? DateApproved { get; set; }

        public ICollection<WHModifyItemTonalityDetails> ModifyItemTonalityDetails { get; set; }

        public Warehouse Warehouse { get; set; }
    }
}

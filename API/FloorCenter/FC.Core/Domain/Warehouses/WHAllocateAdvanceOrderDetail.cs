using System;
using System.Collections.Generic;
using System.Text;

namespace FC.Core.Domain.Warehouses
{
    public class WHAllocateAdvanceOrderDetail : BaseEntity
    {

        public int? WHAllocateAdvanceOrderId { get; set; }

        public int? ItemId { get; set; }

        public string Code { get; set; }

        public int? SizeId { get; set; }

        public int? AllocatedQuantity { get; set; }

        public string Remarks { get; set; }
    }
}

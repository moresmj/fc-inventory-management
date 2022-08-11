using FC.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FC.Core.Domain.Warehouses
{
    public class WHAllocateAdvanceOrder : BaseEntity
    {
        public int? StoreId { get; set; }

        public int? WarehouseId { get; set; }

        public int? StAdvanceOrderId { get; set; }

        [MaxLength(50)]
        public string AllocationNumber { get; set; }

        public DateTime? AllocationDate { get; set; }

        public string Remarks { get; set; }

        public ICollection<WHAllocateAdvanceOrderDetail> AllocateAdvanceOrderDetails { get; set; }


        #region Navigation Properties

        public Store Store { get; set; }

        public Warehouse Warehouse { get; set; }

        #endregion
    }
}

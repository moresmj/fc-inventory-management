using FC.Api.DTOs.Store.AdvanceOrder;
using FC.Api.Validators.Warehouse.AllocateAdvanceOrder;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Warehouse.AllocateAdvanceOrder
{
    [Validator(typeof(AllocateAdvanceOrderDTOValidator))]
    public class AllocateAdvanceOrderDTO
    {
        public int Id { get; set; }

        public int? stAdvanceOrderId { get; set; }

        public DateTime? AllocationDate { get; set; }

        public int? StoreId { get; set; }

        public int? WarehouseId { get; set; }

        public ICollection<AllocateAdvanceOrderDetailsDTO> AllocateAdvanceOrderDetails { get; set; }
    }
}

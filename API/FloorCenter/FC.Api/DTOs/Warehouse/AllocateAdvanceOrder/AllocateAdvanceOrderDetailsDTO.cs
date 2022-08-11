using FC.Api.DTOs.Item;
using FC.Api.Validators.Warehouse.AllocateAdvanceOrder;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Warehouse.AllocateAdvanceOrder
{
    [Validator(typeof(AllocateAdvanceOrderDetailsDTOValidator))]
    public class AllocateAdvanceOrderDetailsDTO
    {

        public int Id { get; set; }

        public int? ItemId { get; set; }

        public int? AllocatedQuantity { get; set; }

        public int? ApprovedQuantity { get; set; }

        public string Remarks { get; set; }

        public ItemDTO Item { get; set; }

        public int? STAdvanceOrderId { get; set; }

        public int? AllocateAdvanceOrderId { get; set; }

        public int? WarehouseId { get; set; }

        public bool? isCustom { get; set; }

        public string Code { get; set; }

        public int? SizeId { get; set; }
    }
}

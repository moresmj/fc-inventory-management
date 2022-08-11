using FC.Api.DTOs.Item;
//using FC.Api.Validators.Warehouse.ModifyTonality;
using FC.Core.Domain.Common;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Warehouse.ModifyTonality
{
    //[Validator(typeof(WHModifyItemTonalityDetailsDTOValidator))]
    public class WHModifyItemTonalityDetailsDTO
    {
        public int Id { get; set; }
        public int? WHModifyItemTonalityId { get; set; }

        public int? ItemId { get; set; }

        public int? OldItemId { get; set; }

        public int? StClientDeliveryId { get; set; }

        public int? StShowroomDeliveryId { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }

        public int? WarehouseId { get; set; }

        public int? requestedQty { get; set; }

        public string Remarks { get; set; }
        

        public ItemDTO Item { get; set; }
    }
}

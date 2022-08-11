using FC.Api.DTOs.Item;
using FC.Api.Validators.Store.AdvanceOrders;
using FC.Core.Domain.Common;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Store.AdvanceOrder
{
    [Validator(typeof(STAdvanceOrderDetailsDTOValidator))]
    public class STAdvanceOrderDetailsDTO
    {

        public int Id { get; set; }

        public int? ItemId { get; set; }

        public int? Quantity { get; set; }

        public int? ApprovedQuantity { get; set; }

        public DeliveryStatusEnum? DeliveryStatus { get; set; }

        public string Remarks { get; set; }

        public ItemDTO Item { get; set; }

        public bool forUpdate { get; set; }

        public int? STAdvanceOrderId { get; set; }



        public string Code { get; set; }

        public int? sizeId { get; set; }

        public string tonality { get; set; }

        public bool? isCustom { get; set; }

        
    }
}

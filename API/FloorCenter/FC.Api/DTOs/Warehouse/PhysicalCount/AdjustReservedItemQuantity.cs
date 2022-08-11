using FC.Api.Validators.Warehouse.PhysicalCount;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Warehouse.PhysicalCount
{
    //[Validator(typeof(AdjustReservedItemQuantityValidator))]
    public class AdjustReservedItemQuantity
    {


        public int Id { get; set; }

        public int? WarehouseId { get; set; }

        public ICollection<AdjustReservedItemQuantityDetails> Details { get; set; }
    }
}

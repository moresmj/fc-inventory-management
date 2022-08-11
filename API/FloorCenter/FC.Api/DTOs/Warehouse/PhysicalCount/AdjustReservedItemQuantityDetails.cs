using FC.Api.Validators.Warehouse.PhysicalCount;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Warehouse.PhysicalCount
{
    //[Validator(typeof(AdjustReservedItemQuantityDetailsValidator))]
    public class AdjustReservedItemQuantityDetails
    {

        /// </summary>
        public int? Id { get; set; }

        public int? WHImportId { get; set; }

        public int? ItemId { get; set; }

        public int? Adjustment { get; set; }

        public int? WarehouseId { get; set; }

        public string Remarks { get; set; }
    }
}

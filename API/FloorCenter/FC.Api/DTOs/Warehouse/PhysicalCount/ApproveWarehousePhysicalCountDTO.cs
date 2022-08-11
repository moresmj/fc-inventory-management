using FC.Api.Validators.Warehouse.PhysicalCount;
using FluentValidation.Attributes;
using System.Collections.Generic;

namespace FC.Api.DTOs.Warehouse.PhysicalCount
{

    [Validator(typeof(ApproveWarehousePhysicalCountDTOValidator))]
    public class ApproveWarehousePhysicalCountDTO
    {

        /// <summary>
        /// WHImport.Id
        /// </summary>
        public int Id { get; set; }

        public ICollection<ApproveWarehousePhysicalCountItems> Details { get; set; }
        
    }
}

using FC.Api.Validators.Store.PhysicalCount;
using FluentValidation.Attributes;
using System.Collections.Generic;

namespace FC.Api.DTOs.Store.PhysicalCount
{

    [Validator(typeof(ApproveStorePhysicalCountDTOValidator))]
    public class ApproveStorePhysicalCountDTO
    {

        /// <summary>
        /// STImport.Id
        /// </summary>
        public int Id { get; set; }

        public ICollection<ApproveStorePhysicalCountItems> Details { get; set; }
        
    }
}

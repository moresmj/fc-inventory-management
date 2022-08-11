using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs.Warehouse.PhysicalCount
{
    public class ApproveWarehousePhysicalCountItems
    {

        /// <summary>
        /// WHImportDetail.Id
        /// </summary>
        public int? Id { get; set; }

        public int? WHImportId { get; set; }

        public int? ItemId { get; set; }

        public bool? AllowUpdate { get; set; }

    }
}

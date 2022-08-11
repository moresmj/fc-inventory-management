using System.Collections.Generic;

namespace FC.Api.DTOs.Warehouse.PhysicalCount
{
    public class UploadWarehousePhysicalCountDTO
    {

        public int? WarehouseId { get; set; }

        public ICollection<UploadWarehousePhysicalCountItems> Details { get; set; }

    }
}

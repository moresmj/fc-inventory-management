using System.Collections.Generic;

namespace FC.Api.DTOs.Warehouse.PhysicalCount
{
    public class UploadWarehouseBreakageDTO
    {

        public int? WarehouseId { get; set; }

        public ICollection<UploadWarehouseBreakageItems> Details { get; set; }

    }
}

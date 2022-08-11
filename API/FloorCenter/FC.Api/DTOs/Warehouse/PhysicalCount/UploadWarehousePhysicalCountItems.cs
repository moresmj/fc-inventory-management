namespace FC.Api.DTOs.Warehouse.PhysicalCount
{
    public class UploadWarehousePhysicalCountItems
    {

        public int? WarehouseId { get; set; }

        public int? ItemId { get; set; }
        
        public int? SystemCount { get; set; }

        public int? PhysicalCount { get; set; }

        public int? Broken { get; set; }

        public string Remarks { get; set; }

    }
}

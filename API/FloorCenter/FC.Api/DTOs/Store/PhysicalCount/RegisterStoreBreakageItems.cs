namespace FC.Api.DTOs.Store.PhysicalCount
{
    public class RegisterStoreBreakageItems
    {

        public int? StoreId { get; set; }

        public int? ItemId { get; set; }
        
        public int? SystemCount { get; set; }

        public int? Broken { get; set; }

        public string Remarks { get; set; }

    }
}

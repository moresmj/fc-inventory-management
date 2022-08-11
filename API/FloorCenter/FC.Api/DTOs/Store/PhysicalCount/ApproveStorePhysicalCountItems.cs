namespace FC.Api.DTOs.Store.PhysicalCount
{
    public class ApproveStorePhysicalCountItems
    {

        /// <summary>
        /// STImportDetail.Id
        /// </summary>
        public int? Id { get; set; }

        public int? STImportId { get; set; }

        public int? ItemId { get; set; }

        public bool? AllowUpdate { get; set; }

    }
}

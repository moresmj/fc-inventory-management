using System.Collections.Generic;

namespace FC.Api.DTOs.Store.PhysicalCount
{
    public class UploadStorePhysicalCountDTO
    {

        public int? StoreId { get; set; }

        public ICollection<UploadStorePhysicalCountItems> Details { get; set; }

    }
}

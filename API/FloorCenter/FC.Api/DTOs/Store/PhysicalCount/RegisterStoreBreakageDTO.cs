using System.Collections.Generic;

namespace FC.Api.DTOs.Store.PhysicalCount
{
    public class RegisterStoreBreakageDTO
    {

        public int? StoreId { get; set; }

        public ICollection<RegisterStoreBreakageItems> Details { get; set; }

    }
}

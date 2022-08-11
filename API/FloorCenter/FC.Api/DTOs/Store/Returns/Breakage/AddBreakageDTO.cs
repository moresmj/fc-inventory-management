using System.Collections.Generic;

namespace FC.Api.DTOs.Store.Returns
{

    public class AddBreakageDTO
    {
        public int? StoreId { get; internal set; }

        public string ReturnFormNumber { get; set; }

        public string Remarks { get; set; }

        public ICollection<AddBreakageItems> PurchasedItems { get; set; }
    }
}

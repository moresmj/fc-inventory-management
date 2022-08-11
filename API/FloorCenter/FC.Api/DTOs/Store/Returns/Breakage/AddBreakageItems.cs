using FC.Core.Domain.Common;

namespace FC.Api.DTOs.Store.Returns
{
    public class AddBreakageItems 
    {

        public int? StoreId { get; internal set; }

        public string PONumber { get; set; }

        public int? ItemId { get; set; }

        public int? GoodQuantity { get; set; }

        public int? BrokenQuantity { get; set; }

        public int? ActualBrokenQuantity { get; set; }

        public ReturnReasonEnum? ReturnReason { get; set; }

        public string Remarks { get; set; }
    }
}

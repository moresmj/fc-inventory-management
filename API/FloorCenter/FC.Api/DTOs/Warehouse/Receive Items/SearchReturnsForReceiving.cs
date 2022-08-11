using FC.Core.Domain.Common;

namespace FC.Api.DTOs.Warehouse.Receive_Items
{
    public class SearchReturnsForReceiving
    {

        public int? WarehouseId { get; set; }

        public string TransactionNo { get; set; }

        public string ReturnFormNumber { get; set; }

        public string DRNumber { get; set; }


        public int?  StoreId { get; set; }

        public DeliveryStatusEnum? DeliveryStatus { get; set; }

    }
}

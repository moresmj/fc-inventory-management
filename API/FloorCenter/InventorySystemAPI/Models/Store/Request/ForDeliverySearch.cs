using InventorySystemAPI.Models.Store.Inventory;

namespace InventorySystemAPI.Models.Store.Request
{
    public class ForDeliverySearch
    {

        public int? Id { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }

        public int? StoreId { get; set; }

    }
}

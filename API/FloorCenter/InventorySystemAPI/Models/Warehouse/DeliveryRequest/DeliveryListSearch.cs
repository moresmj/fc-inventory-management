using InventorySystemAPI.Models.Store.Inventory;

namespace InventorySystemAPI.Models.Warehouse.DeliveryRequest
{
    public class DeliveryListSearch
    {

        public int? Id { get; set; }

        public RequestStatusEnum? RequestStatus { get; set; }

        public int? WarehouseId { get; set; }

    }
}

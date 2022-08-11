using System.Collections.Generic;
using FC.Api.DTOs.Warehouse.Receive_Items;
using FC.Api.Helpers;

namespace FC.Api.Services.Warehouses.Receive_Items
{
    public interface IReturnsForReceivingService
    {
        DataContext DataContext();

        IEnumerable<object> GetAll(SearchReturnsForReceiving search);

        object GetReturnsForReceivingByIdAndWarehouseId(int id, int? warehouseId);

        void ReceiveReturns(ReceiveReturnsDTO param);

    }
}

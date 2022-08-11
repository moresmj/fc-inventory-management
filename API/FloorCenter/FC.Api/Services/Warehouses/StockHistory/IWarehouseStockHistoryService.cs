using FC.Api.DTOs.Inventories;
using FC.Api.DTOs.StockHistory;
using FC.Api.Helpers;
using System.Collections.Generic;

namespace FC.Api.Services.Warehouses.StockHistory
{
    public interface IWarehouseStockHistoryService
    {

        DataContext DataContext();

        IEnumerable<StockHistoryDTO> GetByItemIdAndWarehouseId(int id, int? warehouseId);

        IEnumerable<StockHistoryDTO> GetMainSelectedWarehouseItemHistory(InventorySearchDTO dto);
    }
}

using FC.Api.DTOs.Inventories;
using FC.Api.DTOs.StockHistory;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using System.Collections.Generic;

namespace FC.Api.Services.Stores.StockHistory
{
    public interface IStoreStockHistoryService
    {

        DataContext DataContext();

        IEnumerable<StockHistoryDTO> GetInventoryStockHistory(InventorySearchDTO dto, InventoryMonitoringTypeEnum type);

        //optimized GetInventoryStockHistory implimented pagination
        object GetInventoryStockHistoryPaged(InventorySearchDTO search, InventoryMonitoringTypeEnum type, AppSettings appSettings);

        IEnumerable<StockHistoryDTO> GetByItemIdAndStoreId(int id, int? storeId);

        IEnumerable<StockHistoryDTO> GetMainSelectedStoreItemHistory(InventorySearchDTO dto);
        
    }
}

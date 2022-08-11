using FC.Api.DTOs.Inventories;
using FC.Api.Helpers;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Services.Warehouses
{
    public interface IWHStockService
    {
        
        /// <summary>
        /// Insert whstock
        /// </summary>
        /// <param name="whStock">WHStock</param>
        void InsertStock(WHStock whStock);

        int GetItemForReleasing(int? itemId,int? warehouseId, IQueryable<STOrder> orders);

        DataContext DataContext();

        IEnumerable<object> GetWarehouseItemsByWarehouseId(int? warehouseId);

        IEnumerable<object> GetItemsWithReservation(int? warehouseId);

        IEnumerable<object> GetWarehouseItemsByWarehouseIdWithUniqueCode(int? warehouseId);

        IEnumerable<object> GetInventoriesByWarehouseId(InventorySearchDTO dto);

        List<object> GetWarehouseInventories(InventorySearchDTO dto);

        int GetItemAvailableQuantity(int? itemId, int? warehouseId);

        int GetItemAvailableQuantity(int? itemId, int? warehouseId, bool deductForRelease);

        int GetItemBrokenQuantity(int? itemId, int? warehouseId);

        IEnumerable<object> GetInventoriesSummaryByWarehouseId(int? warehouseId);

        object GetInventoriesSummaryByWarehouseIdPaged(int? warehouseId, InventorySearchDTO search, AppSettings appSettings);

        object GetInventoriesByWarehouseIdPaged(InventorySearchDTO search, AppSettings appSettings);

        object GetWarehouseForReleasingDetails(int? ItemId, InventorySearchDTO search, AppSettings appSettings);

        object GetStoreForReleasingDetails(int? ItemId, InventorySearchDTO search, AppSettings appSettings);

        #region Old Functions

        IEnumerable<object> GetWarehouseItemsByWarehouseIdOld(int? warehouseId);

        IEnumerable<object> GetInventoriesByWarehouseIdOld(InventorySearchDTO dto);

        #endregion

    }
}

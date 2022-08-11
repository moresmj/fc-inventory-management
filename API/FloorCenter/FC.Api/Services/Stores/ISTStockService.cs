using FC.Api.DTOs.Inventories;
using FC.Api.Helpers;
using FC.Core.Domain.Stores;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Services.Stores
{
    public interface ISTStockService
    {

        /// <summary>
        /// Insert ststock
        /// </summary>
        /// <param name="stStock">STStock</param>
        void InsertSTStock(STStock stStock);

        object GetInventoriesByStoreIdPaged(InventorySearchDTO search, AppSettings appSettings);

        IEnumerable<object> GetInventoriesByStoreId(InventorySearchDTO search);

        IEnumerable<object> GetInventoriesByStoreIdForSales(int? storeId, InventorySearchDTO dto);

        IEnumerable<object> GetInventoriesByStoreIdForSalesFromSTStockSummary(int? storeId, InventorySearchDTO dto);

        IEnumerable<object> GetInventoriesByStoreIdForSalesDropdown(int? storeId, InventorySearchDTO dto);

        IEnumerable<object> GetInventoriesByStoreIdForSalesDropdownFromSTStockSummary(int? storeId, InventorySearchDTO dto);

        IEnumerable<object> GetStoreWithItemAvailable(int? storeId, ICollection<STTransferDetail> items);

        IEnumerable<object> GetInventoriesSummaryByStoreId(int? storeId);

        object GetInvetoriesSummaryByStoreIdPaged(int? storeId, InventorySearchDTO search, AppSettings appSettings);

        int GetItemForReleasing(int? itemId, int? storeId);

        int GetItemBrokenQuantity(int? itemId, int? storeId);

        int GetItemAvailableQuantity(int? itemId, int? storeId, bool deductReleasing = false);

        void UpdateSTStock(STStock stStock);

        List<object> GetStoreInventories(InventorySearchDTO dto);

        object GetStoresWithItem(InventorySearchDTO search);


         IQueryable<STOrder> stOrder { get; set; }
         IQueryable<STStock> stStock { get; set; }
         IQueryable<STSales> stSales { get; set; }
         IQueryable<STReturn> stReturns { get; set; }
         IQueryable<STImportDetail> stImportDetail { get; set; }
         IQueryable<STImport> stImport { get; set; }

        #region Old Functions

        IEnumerable<object> GetInventoriesByStoreIdOld(InventorySearchDTO search);

        #endregion

    }
}

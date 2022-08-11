using FC.Api.DTOs.Store.Releasing;
using FC.Api.Helpers;
using FC.Core.Domain.Stores;
using System.Collections.Generic;

namespace FC.Api.Services.Stores.Releasing
{
    public interface ISalesOrderService
    {

        DataContext DataContext();

        /// <summary>
        /// Get all sales records 
        /// WHERE SalesType = ClientOrder AND DeliveryType = ShowroomPickup
        /// </summary>
        /// <param name="search">Search parameters</param>
        /// <returns></returns>
        IEnumerable<object> GetAll(SearchSalesOrder search);

        /// Paged version of getall
        object GetAllPaged(SearchSalesOrder search, AppSettings appSettings);

        /// <summary>
        /// Get sales record by id and storeid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        STSales GetSalesRecordByIdAndStoreId(int? id, int? storeId);

        /// <summary>
        /// Get delivery record by id and storeid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        STDelivery GetDeliveryRecordByIdAndStoreId(int? id, int? storeId);

        /// <summary>
        /// Update sales record
        /// </summary>
        /// <param name="stockService"></param>
        /// <param name="param"></param>
        void UpdateSalesRecord(ISTStockService stockService, STSales param);

        /// <summary>
        /// Update delivery record
        /// </summary>
        /// <param name="stockService"></param>
        /// <param name="param"></param>
        string UpdateDelivery(ISTStockService stockService, STDelivery param);
        
    }
}

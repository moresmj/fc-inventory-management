using FC.Api.DTOs.Store.Releasing;
using FC.Api.Helpers;
using FC.Core.Domain.Stores;
using System.Collections.Generic;

namespace FC.Api.Services.Stores.Releasing
{
    public interface ITransferService
    {

        DataContext DataContext();

        /// <summary>
        /// Get all sales records 
        /// WHERE SalesType = Interbranch or Intercompany
        /// </summary>
        /// <param name="search">Search parameters</param>
        /// <returns></returns>
        IEnumerable<object> GetAll(SearchTransfer search);

        object GetAllPaged(SearchTransfer search, AppSettings appSettings);

        /// <summary>
        /// Get order record (pickup) by id and storeid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        STOrder GetPickupOrderByIdAndStoreId(int id, int? storeId);

        /// <summary>
        /// Update pickup order
        /// </summary>
        /// <param name="stockService"></param>
        /// <param name="param"></param>
        void UpdatePickupOrder(ISTStockService stockService, STOrder param);

        /// <summary>
        /// Get delivery record by id and storeid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        STDelivery GetDeliveryOrderByIdAndStoreId(int id, int? storeId);

        /// <summary>
        /// Update delivery record
        /// </summary>
        /// <param name="stockService"></param>
        /// <param name="param"></param>
        void UpdateDeliveryOrder(ISTStockService stockService, STDelivery param);

        /// <summary>
        /// Get sales record (showroom pickup) by id and storeid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        STDelivery GetShowroomPickupOrderByIdAndStoreId(int id, int? storeId);

        /// <summary>
        /// Update sales record (showroom pickup)
        /// </summary>
        /// <param name="stockService"></param>
        /// <param name="param"></param>
        string UpdateShowroomPickupOrder(ISTStockService stockService, STDelivery param);
        

    }
}

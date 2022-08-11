using FC.Api.DTOs.Store.Releasing;
using FC.Api.Helpers;
using FC.Core.Domain.Stores;
using System.Collections.Generic;

namespace FC.Api.Services.Stores.Releasing
{
    public interface IForClientOrderService
    {

        DataContext DataContext();

        /// <summary>
        /// Get all sales records 
        /// WHERE SalesType = ClientOrder AND DeliveryType = ShowroomPickup
        /// </summary>
        /// <param name="search">Search parameters</param>
        /// <returns></returns>
        IEnumerable<object> GetAll(SearchForClientOrder search);


        object GetAllPaged(SearchForClientOrder search, AppSettings appSettings);

        /// <summary>
        /// Get sales record by id
        /// </summary>
        /// <param name="id">STSales.Id</param>
        /// <param name="storeId">STSales.StoreId</param>
        /// <returns></returns>
        STSales GetByIdAndStoreId(int? id, int? storeId);

        /// <summary>
        /// Update sales record
        /// </summary>
        /// <param name="stockService"></param>
        /// <param name="param"></param>
        void Update(ISTStockService stockService, STSales param);

    }
}

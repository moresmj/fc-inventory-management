using FC.Api.DTOs.Store.Releasing;
using FC.Api.Helpers;
using FC.Core.Domain.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Stores.Releasing
{
    public interface ISameDaySalesService
    {

        DataContext DataContext();

        /// <summary>
        /// Get all sales records 
        /// WHERE SalesType = Releasing AND DeliveryType = Pickup
        /// </summary>
        /// <param name="search">Search parameters</param>
        /// <returns></returns>
        IEnumerable<object> GetAll(SearchSameDaySales search);

        object GetAllPaged(SearchSameDaySales search, AppSettings appSettings);

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

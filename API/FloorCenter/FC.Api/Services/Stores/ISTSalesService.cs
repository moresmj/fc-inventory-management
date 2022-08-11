using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Core.Domain.Stores;
using System.Collections.Generic;

namespace FC.Api.Services.Stores
{
    public interface ISTSalesService
    {

        DataContext DataContext();

        /// <summary>
        /// Insert sales
        /// </summary>
        /// <param name="sales">STSales</param>
        void InsertSales(STSales sales, bool deductStock = false, bool isSamedaySales = false);

        void InsertSalesForTransfer(STSales sales);

        /// <summary>
        /// Get all sales
        /// </summary>
        /// <param name="search">Search parameters</param>
        /// <returns>STSales</returns>
        IEnumerable<STSales> GetAllSalesForReleasing(SearchSalesForReleasing search);
        IEnumerable<object> GetAllSalesForReleasing(SearchSalesForReleasing search, IMapper mapper);

        STSales GetSalesForReleasingById(int? id, int? storeId);

        /// <summary>
        /// Update sales
        /// </summary>
        /// <param name="param">STSales</param>
        void UpdateSalesForReleasing(ISTStockService stockService, STSales param);

        /// <summary>
        /// Get all sales orders
        /// </summary>
        /// <param name="search">Search parameters</param>
        /// <returns>STSales</returns>
        object GetAllSalesOrders(SearchSalesOrder search);


        object GetAllSalesOrdersPaged(SearchSalesOrder search, AppSettings appSettings);


    }
}

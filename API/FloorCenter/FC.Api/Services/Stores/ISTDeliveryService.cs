using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Core.Domain.Stores;
using System.Collections.Generic;

namespace FC.Api.Services.Stores
{
    public interface ISTDeliveryService
    {


        /// <summary>
        /// Insert delivery
        /// </summary>
        /// <param name="stdelivery">STDelivery</param>
        void InsertDeliveryToShowroom(STDelivery stdelivery);


        /// <summary>
        /// Update delivery
        /// </summary>
        /// <param name="param">STDelivery</param>
        void UpdateDelivery(STDelivery param);

        void UpdateDeliveryStatus(ISTStockService stockService, STDelivery param);

        void InsertToSTStock(SaveReceiveItem param, ISTStockService service,ISTOrderService storderservice);


        void InsertDeliveryForClient(STDelivery stdelivery);

        IEnumerable<object> GetAllForDeliveries(SearchDeliveries search, IMapper mapper);

        IEnumerable<object> GetAllDeliveriesForSales(SearchDeliveries search, IMapper mapper);

        object GetAllDeliveriesForSalesPaged(SearchDeliveries search, IMapper mapper, AppSettings appSettings);

        /// <summary>
        /// Get sales order deliveries records
        /// </summary>
        /// <param name="id">STSales.Id</param>
        /// <param name="storeId">Store.Id</param>
        /// <returns></returns>
        object GetSalesOrderDeliveriesBySalesId(int? id, int? storeId);

        void InsertSalesOrderDelivery(ISTStockService stockService, STDelivery delivery);

        STDelivery GetSalesDeliveryForReleasingByIdAndStoreId(int id, int? storeId);

        void UpdateSalesDeliveryForReleasing(ISTDeliveryService deliveryService, ISTStockService stockService, STDelivery param);

        object GetAllForDeliveriesPaged2(SearchDeliveries search, IMapper mapper, AppSettings appSettings);

        object GetAllForDeliveriesPaged(SearchDeliveries search, IMapper mapper, AppSettings appSettings);

        object GetAllDeliveriesForShowroom(SearchDeliveries search, IMapper mapper, AppSettings appSettings);

    }
}

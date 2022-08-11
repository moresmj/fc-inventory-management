using System.Collections.Generic;
using FC.Api.DTOs.Store.Returns;
using FC.Api.Helpers;
using FC.Core.Domain.Warehouses;

namespace FC.Api.Services.Stores.Releasing
{
    public interface IPurchaseReturnService
    {
        DataContext DataContext();

        IEnumerable<object> GetAll(SearchReturns search);

        object GetAllPaged(SearchReturns search, AppSettings appSettings);

        WHDelivery GetDeliveryByIdAndStoreId(int id, int? storeId);

        void ReleasePurchaseReturnDelivery(ISTStockService stockService, WHDelivery param);

    }
}

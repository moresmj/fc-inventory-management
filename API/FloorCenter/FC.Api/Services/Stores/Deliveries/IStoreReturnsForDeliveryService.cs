using System.Collections.Generic;
using FC.Api.DTOs.Store.Deliveries;
using FC.Api.Helpers;
using FC.Api.Services.Warehouses.Returns;

namespace FC.Api.Services.Stores.Deliveries
{
    public interface IStoreReturnsForDeliveryService
    {
        IEnumerable<object> GetAll(SearchReturnsForDeliveries search);

        object GetAllPaged(SearchReturnsForDeliveries search, AppSettings appSettings);

        void UpdateStoreReturnsDelivery(UpdateStoreReturnsDeliveryDTO param);

    }
}

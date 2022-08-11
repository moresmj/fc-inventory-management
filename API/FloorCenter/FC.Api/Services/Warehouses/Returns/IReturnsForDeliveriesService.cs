using FC.Api.DTOs.Warehouse.Delivery;
using FC.Api.Helpers;
using System.Collections.Generic;

namespace FC.Api.Services.Warehouses.Returns
{
    public interface IReturnsForDeliveriesService
    {

        DataContext DataContext();

        IEnumerable<object> GetAll(SearchReturnsForDeliveries search);

        object GetAllPaged(SearchReturnsForDeliveries search, AppSettings appSettings);

        object GetAllPaged2(SearchReturnsForDeliveries search, AppSettings appSettings);

        void UpdateReturnsDelivery(UpdateReturnsDeliveryDTO param);

    }
}

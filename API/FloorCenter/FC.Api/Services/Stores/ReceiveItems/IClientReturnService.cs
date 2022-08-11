using System.Collections.Generic;
using FC.Api.DTOs.Store.ReceiveItems;
using FC.Api.Helpers;

namespace FC.Api.Services.Stores.ReceiveItems
{
    public interface IClientReturnService
    {
        DataContext DataContext();

        IEnumerable<object> GetAll(SearchReceiveItemsClientReturns search);

        object GetByIdAndStoreId(int? id, int? storeId);

        void ReceiveClientReturns(ReceiveClientReturnsDTO param);

    }
}

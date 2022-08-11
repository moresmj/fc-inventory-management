using System.Collections.Generic;
using FC.Api.DTOs.Store.Returns.ClientReturn;
using FC.Api.Helpers;

namespace FC.Api.Services.Stores.Returns
{
    public interface IClientReturnService
    {

        DataContext DataContext();

        IEnumerable<object> GetAll(SearchClientReturn search);

        object GetAllPaged(SearchClientReturn search, AppSettings appSettings);

        object GetByIdAndStoreId(int id, int? storeId);

        string AddClientReturn(AddClientReturnDTO param);

    }
}

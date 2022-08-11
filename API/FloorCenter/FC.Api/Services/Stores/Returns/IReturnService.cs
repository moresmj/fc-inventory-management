using System.Collections.Generic;
using FC.Api.DTOs.Store.Returns;
using FC.Api.Helpers;
using FC.Core.Domain.Stores;

namespace FC.Api.Services.Stores.Returns
{
    public interface IReturnService
    {

        IEnumerable<object> GetAllForApproval(SearchReturns search);

        object GetForApprovalPaged(SearchReturns search, AppSettings appSettings);

        object GetForApprovalPaged2(SearchReturns search, AppSettings appSettings);

        STReturn GetPurchaseReturnBy(int id);

        void ApprovePurchaseReturn(STReturn param);

        IEnumerable<object> GetAllReturns(SearchReturns search);

        object GetAllReturnsPaged(SearchReturns search, AppSettings appSettings);

        object GetMainSelectedStoreRTV(SearchReturns search, AppSettings appSettings);

        object GetMainSelectedStoreRTV2(SearchReturns search, AppSettings appSettings);

    }
}

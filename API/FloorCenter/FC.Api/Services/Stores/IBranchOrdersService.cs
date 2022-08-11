using System.Collections.Generic;
using FC.Api.DTOs.Store.BranchOrders;
using FC.Api.Helpers;

namespace FC.Api.Services.Stores
{
    public interface IBranchOrdersService
    {

        DataContext DataContext();

        IEnumerable<object> GetAllBranchOrders(Search search, ISTStockService stockService);

        object GetAllBranchOrdersPaged(Search search, ISTStockService stockService, AppSettings appSettings);

        void UpdateTransferRequest(UpdateTransferRequestDTO dto, ISTSalesService salesService);

    }
}

using FC.Api.DTOs.Store;
using FC.Api.DTOs.Store.BranchOrders;
using FC.Api.DTOs.Store.Transfers;
using FC.Api.Helpers;
using FC.Core.Domain.Stores;
using System.Collections.Generic;
using System.Security.Claims;

namespace FC.Api.Services.Stores
{
    public interface ISTTransferService
    {

        DataContext DataContext();

        IEnumerable<object> GetAllForTransferApproval(SearchTransfers search, ISTStockService stockService);

        object GetForTransferApprovalPaged(SearchTransfers search, ISTStockService stockService, AppSettings appSettings);

        object GetForTransferApprovalPaged2(SearchTransfers search, ISTStockService stockService, AppSettings appSettings);

        void ApproveTransferRequestOnMain(ApproveTransferRequestDTO dto, ISTSalesService salesService);

        /// <summary>
        /// Insert transfer record
        /// </summary>
        /// <param name="param">STTransfer</param>
        int? AddTransferOrder(STTransfer param, ISTStockService stockService);

        void SaveTransferToOrder(AddTransferOrderDTO dto, ClaimsPrincipal currentuser, AppSettings appSettings);

        string GetTransactionNumberForMultipleStore();

    }
}

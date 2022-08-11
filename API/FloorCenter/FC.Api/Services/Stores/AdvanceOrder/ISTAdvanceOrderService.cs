using FC.Api.DTOs.Store;
using FC.Api.DTOs.Store.AdvanceOrder;
using FC.Api.DTOs.Store.AdvanceOrders;
//using FC.Api.DTOs.Warehouse.AllocateAdvanceOrder;
using FC.Api.Helpers;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Stores.AdvanceOrder
{
    public interface ISTAdvanceOrderService
    {

        void ApproveAdvanceOrder(STAdvanceOrderDTO order);

        string UpdateDelivery(ModifyAdvanceOrderDTO details, AppSettings appSettings, Boolean isDealer);

        void AllocateAdvanceOrder(int? id, WHAllocateAdvanceOrder order, AppSettings appSettings);

        object GetApprovedAdvanceOrderList(SearchApproveRequests search, AppSettings appSettings);

        object GetAdvanceOrderById(int? id);

        int? GetAdvanceOrderForAllocationQuantity(STAdvanceOrderDetails advanceOrderDetails, int? itemId, int? stAdvanceOrderId);
        int? GetAdvanceOrderAllocatedQuantity(STAdvanceOrderDetails advanceOrderDetails, int? itemId, int? stAdvanceOrderId);


    }
}

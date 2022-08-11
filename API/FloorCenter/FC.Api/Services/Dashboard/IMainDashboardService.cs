using FC.Api.DTOs.Store;
using FC.Api.DTOs.Store.Returns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Dashboard
{
    public interface IMainDashboardService
    {
        object DashboardSummary();

        object NotificationdSummary();

        object GetStocksSummary();

        object GetApproveRequestSummary();

        object GetReturnPendingSummary();

        object GetAdjustmentStore();

        object GetAdjustmentWarehouse();

        object GetPendingTransfers();

        object GetPendingAssignDr();
    }
}

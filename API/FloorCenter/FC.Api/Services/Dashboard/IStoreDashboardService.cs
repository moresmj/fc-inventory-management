using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Dashboard
{
    public interface IStoreDashboardService
    {
        object DashboardNotificationSummary(int? storeId, IMapper mapper);

        object DashboardSummary(int? storeId, IMapper mapper);

        object GetStocksSummary(int? storeId);

        object GetPendingOrders(int? storeId);

        object GetPendingDeliveries(int? storeId, IMapper mapper);

        object GetWaitingDeliveries(int? storeId, IMapper mapper);

        object GetWaitingForPickUp(int? storeId);

        object GetPendingSales(int? storeId);

        IEnumerable<object> GetStoreStockAlerts(int? storeId);

        object GetPendingToReceiveReturns(int? storeId);
    }
}

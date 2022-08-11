using AutoMapper;
using FC.Api.DTOs.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Dashboard
{
    public interface IWarehouseDashboardService
    {
        object GetStocksSummary(int? warehouseId);

        object GetApproveOrders(int? warehouseId);

        object GetWaitingDeliveries(int? warehouseId, IMapper mapper);

        object GetWaitingForPickUp(int? warehouseId);

        IEnumerable<object> GetWarehouseStockAlerts(int? warehouseId);

        object GetWarehouseRTVApproval(int? warehouseId);

        object DashboardSummary(int? warehouseId, IMapper mapper);

    }
}

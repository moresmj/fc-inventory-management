using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.DTOs.Store.Returns;
using FC.Api.Services.Dashboard;
using FC.Api.Services.Stores;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace FC.Api.Controllers
{
    [Route("api/dashboard")]
    public class DashboardController : BaseController
    {

        private IWHStockService _warehouseStockService;
        private IMainDashboardService _mainDashboardService;
        private IStoreDashboardService _storeDashboardService;
        private IWarehouseDashboardService _warehouseDashboardService;
        private ILogisticsDashboardService _logisticsDashboardService;
        private IMapper _mapper;

        public DashboardController(
            IWHStockService whStockService,
            IMainDashboardService mainDashboardService,
            IStoreDashboardService storeDashboardService,
            IWarehouseDashboardService warehouseDashboardService,
            ILogisticsDashboardService logisticsDashboardService,
            IMapper mapper
            )
        {
            _warehouseStockService = whStockService;
            _mainDashboardService = mainDashboardService;
            _storeDashboardService = storeDashboardService;
            _warehouseDashboardService = warehouseDashboardService;
            _logisticsDashboardService = logisticsDashboardService;
            _mapper = mapper;
        }


        #region Main

        [HttpGet]
        [Route("main/sse/summary")]
        public IActionResult GetMainSummary()
        {
            //Thread.Sleep(10000);
            var data = _mainDashboardService.NotificationdSummary();

            //return Content($"data: {{\"approveRequestTotal\": {data["ApproveRequestTotal"]  }" +
            //                       $",\"approveRequestItemTotal\": {data["ApproveRequestItemTotal"]  }" +
            //                       $",\"pendingReturnsTotal\": {data["PendingReturnsTotal"]  }" +
            //                       $",\"pendingReturnsTotalItem\": {data["PendingReturnsTotalItem"]  }" +
            //                       $",\"storeAdjustmentTotal\": {data["StoreAdjustmentTotal"]  }" +
            //                       $",\"storeAdjustmentTotalItem\": {data["StoreAdjustmentTotalItem"]  }" +
            //                       $",\"pendingTransferTotal\": {data["PendingTransferTotal"]  }" +
            //                       $",\"pendingTransferTotalItem\": {data["PendingTransferTotalItem"]  }" +
            //                       $",\"pendingAssignDrTotal\": {data["PendingAssignDrTotal"]  }" +
            //                       $",\"pendingAssignDrTotalItem\": {data["PendingAssignDrTotalItem"]  }" +
            //                       $",\"notificationsTotal\": {data["NotificationsTotal"]  }" +
            //                       $"}}\n\n", "text/event-stream");
            return Ok(data);
        }

        [Route("main/dashboard/summary")]
        public IActionResult GetMainDashboardSummary()
        {
            var data = _mainDashboardService.DashboardSummary();

            return Ok(data);
        }

        [HttpGet]
        [Route("main/stockssummary")]
        public IActionResult GetMainStockSummary()
        {
            var list = _mainDashboardService.GetStocksSummary();
            return Ok(list);
        }

        #region Main- Unused can be used later on

        [Route("main/sse/approvesummary")]
        public IActionResult GetMainApproveRequestSummarySse()
        {
            Thread.Sleep(10000);
            dynamic list = _mainDashboardService.GetApproveRequestSummary();
            return Content($"data: {{\"approveRequestTotal\": {list?.ApproveRequestTotal },\"approveRequestItemTotal\": {list?.ApproveRequestItemTotal }}}\n\n", "text/event-stream");
        }

        [HttpGet]
        [Route("main/pendingreturns")]
        public IActionResult GetMainPendingReturnsSummary()
        {

            dynamic list = _mainDashboardService.GetReturnPendingSummary();

            return Content($"data: {{\"returnTotal\": {list?.PendingReturns },\"returnTotalItem\": {list?.PendingReturnsQty }}}\n\n", "text/event-stream");

        }

        [HttpGet]
        [Route("main/pendingStoreAdjustment")]
        public IActionResult GetMainPendingStoreAdjustment()
        {
            dynamic list = _mainDashboardService.GetAdjustmentStore();
            return Content($"data: {{\"storeAdjustmentTotal\": {list?.StoreAdjustmentTotal },\"storeAdjustmentTotalItem\": {list?.StoreAdjustmentTotalItem }}}\n\n", "text/event-stream");
        }


        [HttpGet]
        [Route("main/pendingWarehouseAdjustment")]
        public IActionResult GetMainPendingWarehouseAdjustment()
        {
            dynamic list = _mainDashboardService.GetAdjustmentWarehouse();
            return Content($"data: {{\"warehouseAdjustmentTotal\": {list?.WarehouseAdjustment },\"warehouseAdjustmentTotalItem\": {list?.WarehouseAdjustmentItem }}}\n\n", "text/event-stream");
        }

        [HttpGet]
        [Route("main/pendingAssignDr")]
        public IActionResult GetPendingAssignDr()
        {
            dynamic list = _mainDashboardService.GetPendingAssignDr();
            return Content($"data: {{\"pendingAssignDrtotal\": {list?.pendingAssignDrCount },\"pendingAssignDrtotalItem\": {list?.pendingAssignDrCountTotalItem }}}\n\n", "text/event-stream");
        }

        #endregion


        #endregion

        #region Store

        [HttpGet]
        [Route("store/sse/notif_summary")]
        public IActionResult GetStoreNotificationSummary()
        {
            //Thread.Sleep(10000);
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());
            //dynamic data = _storeDashboardService.DashboardNotificationSummary(storeId, _mapper);

            //return Content($"data: {{\"ordersWaitingForDeliveriesTotal\": {data["OrdersWaitingForDeliveriesTotal"]  }" +
            //                       $",\"pendingBranchOrdersApprovalTotal\": {data["PendingBranchOrdersApprovalTotal"]  }" +
            //                       $",\"notificationsTotal\": {data["NotificationsTotal"]  }" +
            //                       $"}}\n\n", "text/event-stream");

            var data = _storeDashboardService.DashboardNotificationSummary(storeId, _mapper);
            return Ok(data);
        }


        [HttpGet]
        [Route("store/sse/summary")]
        public IActionResult GetStoreSummary()
        {
            //Thread.Sleep(10000);
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());
            //dynamic data = _storeDashboardService.DashboardSummary(storeId, _mapper);

            //return Content($"data: {{\"pendingOrdersTotal\": {data["PendingOrdersTotal"]  }" +
            //                       $",\"pendingOrdersTotalItem\": {data["PendingOrdersTotalItem"]  }" +
            //                       $",\"pendingDeliveriesTotal\": {data["PendingDeliveriesTotal"]  }" +
            //                       $",\"pendingDeliveriesTotalItem\": {data["PendingDeliveriesTotalItem"]  }" +
            //                       $",\"waitingDeliveriesTotal\": {data["WaitingDeliveriesTotal"]  }" +
            //                       $",\"waitingDeliveriesTotalItem\": {data["WaitingDeliveriesTotalItem"]  }" +
            //                       $",\"waitingForPickUpTotal\": {data["WaitingForPickUpTotal"]  }" +
            //                       $",\"waitingForPickUpTotalItem\": {data["WaitingForPickUpTotalItem"]  }" +
            //                       $",\"pendingSalesTotal\": {data["PendingSalesTotal"]  }" +
            //                       $",\"pendingSalesTotalItem\": {data["PendingSalesTotalItem"]  }" +
            //                       $",\"pendingToReceiveReturnsTotal\": {data["PendingToReceiveReturnsTotal"]  }" +
            //                       $",\"pendingToReceiveReturnsTotalItems\": {data["PendingToReceiveReturnsTotalItems"]  }" +
            //                       $",\"notificationsTotal\": {data["NotificationsTotal"]  }" +
            //                       $"}}\n\n", "text/event-stream");

            var data = _storeDashboardService.DashboardSummary(storeId, _mapper);

            return Ok(data);
        }


        [HttpGet]
        [Route("store/stockssummary")]
        public IActionResult GetStoreStocksSummary()
        {
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());
            var list = _storeDashboardService.GetStocksSummary(storeId);

            return Ok(list);
        }

        #region Store- UnUsed can be used later on 

        [HttpGet]
        [Route("store/orders/pending")]
        public IActionResult GetStorePendingOrders()
        {
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());
            var list = _storeDashboardService.GetPendingOrders(storeId);

            return Ok(list);
        }

        [HttpGet]
        [Route("store/deliveries/pending")]
        public IActionResult GetStorePendingDeliveries()
        {
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());
            var list = _storeDashboardService.GetPendingDeliveries(storeId, _mapper);

            return Ok(list);
        }

        [HttpGet]
        [Route("store/deliveries/waiting")]
        public IActionResult GetStoreWaitingDeliveries()
        {
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());
            var list = _storeDashboardService.GetWaitingDeliveries(storeId, _mapper);

            return Ok(list);
        }

        [HttpGet]
        [Route("store/deliveries/pickup")]
        public IActionResult GetWaitingForPickUp()
        {
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());
            var list = _storeDashboardService.GetWaitingForPickUp(storeId);

            return Ok(list);
        }

        [HttpGet]
        [Route("store/sales/pending")]
        public IActionResult GetPendingSales()
        {
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());
            var list = _storeDashboardService.GetPendingSales(storeId);

            return Ok(list);
        }

        [HttpGet]
        [Route("store/stockalerts")]
        public IActionResult GetStoreStockAlerts()
        {
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());
            var list = _storeDashboardService.GetStoreStockAlerts(storeId);

            return Ok(list);
        }

        [HttpGet]
        [Route("store/returns/pending_to_receive")]
        public IActionResult GetPendingToReceiveReturns()
        {
            var storeId = this.GetCurrentUserStoreId(_warehouseStockService.DataContext());
            var list = _storeDashboardService.GetPendingToReceiveReturns(storeId);

            return Ok(list);
        }

        #endregion



        #endregion

        #region Warehouse

        [HttpGet]
        [Route("warehouse/sse/summary")]
        public IActionResult GetWarehouseSummary()
        {
            //Thread.Sleep(10000);
            var warehouseId = this.GetCurrentUserWarehouseId(_warehouseStockService.DataContext());
            var data = _warehouseDashboardService.DashboardSummary(warehouseId, _mapper);

            //return Content($"data: {{\"waitingDeliveriesTotal\": {data["WaitingDeliveriesTotal"]  }" +
            //                       $",\"waitingDeliveriesTotalItem\": {data["WaitingDeliveriesTotalItem"]  }" +
            //                       $",\"waitingForPickUpTotal\": {data["WaitingForPickUpTotal"]  }" +
            //                       $",\"waitingForPickUpTotalItem\": {data["WaitingForPickUpTotalItem"]  }" +
            //                       //$",\"waitingRTVApprovalTotal\": {data["WaitingRTVApprovalTotal"]  }" +
            //                       //$",\"waitingRTVApprovalTotalItem\": {data["WaitingRTVApprovalTotalItem"]  }" +
            //                       $",\"notificationsTotal\": {data["NotificationsTotal"]  }" +
            //                       $"}}\n\n", "text/event-stream");


            //return Content($"data: {{\"waitingDeliveriesTotal\": {data.WaitingDeliveriesTotal }" +
            //                     $",\"waitingDeliveriesTotalItem\": {data.WaitingDeliveriesTotalItem  }" +
            //                     $",\"waitingForPickUpTotal\": {data.WaitingForPickUpTotal  }" +
            //                     $",\"waitingForPickUpTotalItem\": {data.WaitingForPickUpTotalItem  }" +
            //                     $",\"notificationsTotal\": {data.NotificationsTotal  }" +
            //                     $"}}\n\n", "text/event-stream");
            return Ok(data);
        }

        [HttpGet]
        [Route("warehouse/stockssummary")]
        public IActionResult GetWarehouseStocksSummary()
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_warehouseStockService.DataContext());
            var list = _warehouseDashboardService.GetStocksSummary(warehouseId);

            return Ok(list);
        }

        [HttpGet]
        [Route("warehouse/approveorders")]
        public IActionResult GetWarehouseApproveOrders()
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_warehouseStockService.DataContext());
            var list = _warehouseDashboardService.GetApproveOrders(warehouseId);

            return Ok(list);
        }

        [HttpGet]
        [Route("warehouse/deliveries/waiting")]
        public IActionResult GetWarehouseWaitingDeliveries()
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_warehouseStockService.DataContext());
            var list = _warehouseDashboardService.GetWaitingDeliveries(warehouseId, _mapper);
            return Ok(list);
        }

        [HttpGet]
        [Route("warehouse/deliveries/pickup")]
        public IActionResult GetWarehouseWaitingForPickUp()
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_warehouseStockService.DataContext());
            var list = _warehouseDashboardService.GetWaitingForPickUp(warehouseId);
            return Ok(list);
        }

        [HttpGet]
        [Route("warehouse/stockalerts")]
        public IActionResult GetWarehouseStockAlerts()
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_warehouseStockService.DataContext());
            var list = _warehouseDashboardService.GetWarehouseStockAlerts(warehouseId);

            return Ok(list);
        }

        [HttpGet]
        [Route("warehouse/returns/waiting_rtv_approval")]
        public IActionResult GetWarehouseRTVApproval()
        {
            var warehouseId = this.GetCurrentUserWarehouseId(_warehouseStockService.DataContext());
            var list = _warehouseDashboardService.GetWarehouseRTVApproval(warehouseId);

            return Ok(list);
        }

        #endregion

        #region Logistics

        [Route("logistics/sse/summary")]
        public IActionResult GetLogisticsSummary()
        {
            //Thread.Sleep(10000);
            //dynamic data = _logisticsDashboardService.DashboardSummary(_mapper);

            //return Content($"data: {{\"pendingOrderDeliveriesTotal\": {data["PendingOrderDeliveriesTotal"]  }" +
            //                       $",\"pendingOrderDeliveriesTotalItem\": {data["PendingOrderDeliveriesTotalItem"]  }" +
            //                       $",\"waitingOrderDeliveriesTotal\": {data["WaitingOrderDeliveriesTotal"]  }" +
            //                       $",\"waitingDeliveriesTotalItem\": {data["WaitingDeliveriesTotalItem"]  }" +
            //                       $",\"pendingSalesDeliveryTotal\": {data["PendingSalesDeliveryTotal"]  }" +
            //                       $",\"pendingSalesDeliveryTotalItem\": {data["PendingSalesDeliveryTotalItem"]  }" +
            //                       $",\"pendingPickUpClientReturnsTotal\": {data["PendingPickUpClientReturnsTotal"]  }" +
            //                       $",\"pendingPickUpClientReturnsTotalItem\": {data["PendingPickUpClientReturnsTotalItem"]  }" +
            //                       $",\"pendingPickUpRTVTotal\": {data["PendingPickUpRTVTotal"]  }" +
            //                       $",\"pendingPickUpRTVTotalItem\": {data["PendingPickUpRTVTotalItem"]  }" +
            //                       $",\"notificationsTotal\": {data["NotificationsTotal"]  }" +
            //                       $"}}\n\n", "text/event-stream");

            var data = _logisticsDashboardService.DashboardSummary(_mapper);

            return Ok(data);
        }

        #endregion






    }
}

using AutoMapper;
using FC.Api.DTOs.Inventories;
using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Dashboard
{


    public class StoreDashboardService : IStoreDashboardService
    {

        private DataContext _context;
        private ISTStockService _stockService;
        private ILogisticsDashboardService _logisticsDashboardService;


        public StoreDashboardService(DataContext context, ISTStockService stockService, ILogisticsDashboardService logisticsDashboardService)
        {
            _context = context;
            _stockService = stockService;
            _logisticsDashboardService = logisticsDashboardService;
        }


        public object DashboardNotificationSummary(int? storeId, IMapper mapper)
        {
            // List<object> results = new List<object>();

            // results.Add(GetOrdersWaitingForDeliveries(storeId));
            // results.Add(GetPendingBranchOrdersApproval(storeId));

            //var test = ObjectSummary(results).ToList();

            var result = GetStoreNotification(storeId);


            return result;
        }


        public object DashboardSummary(int? storeId, IMapper mapper)
        {
            //List<object> results = new List<object>();

            //results.Add(GetStocksSummary(storeId));
            //results.Add(GetPendingOrders(storeId));
            //results.Add(GetPendingDeliveries(storeId, mapper));
            //results.Add(GetWaitingDeliveries(storeId, mapper));
            //results.Add(GetWaitingForPickUp(storeId));
            //results.Add(GetPendingSales(storeId));
            //results.Add(GetPendingToReceiveReturns(storeId));

            var results = _context.StoreDashboardSummary.FromSql("SELECT * FROM dbo.GetStoreDashboardSummary({0})", storeId).FirstOrDefault();

            //return ObjectSummary(results);
            return results;

        }

        public object GetOrdersWaitingForDeliveries(int? storeId)
        {
            var total = _context.STOrders.AsNoTracking()
                                    .Include(p => p.Deliveries)
                                    .Include(p => p.Warehouse)
                                    .Where(p => p.StoreId == storeId
                                                && p.RequestStatus == RequestStatusEnum.Approved
                                                && (p.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder || p.DeliveryType != DeliveryTypeEnum.Pickup)
                                                && (p.ORNumber != null || p.SINumber != null || p.WHDRNumber != null)
                                                && p.Deliveries.Count() == 0).Count();


            return new
            {
                NotificationsTotal = total,
                OrdersWaitingForDeliveriesTotal = total
            };
        }

        public object GetStoreNotification(int? storeId)
        {

            var orders = _context.STOrders.AsNoTracking()
                                    .Include(p => p.Deliveries)
                                    .Include(p => p.Warehouse)
                                    .Where(p =>  p.RequestStatus == RequestStatusEnum.Approved);

            var waitingForDel = orders.Where(p => 
                                                    p.StoreId == storeId 
                                                && (p.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder || p.DeliveryType != DeliveryTypeEnum.Pickup)
                                                && (p.ORNumber != null || p.SINumber != null || p.WHDRNumber != null)
                                                && p.Deliveries.Count() == 0).Count();

            var waitingForAddDel = orders.Where(p =>
                                        p.StoreId == storeId
                                    && (p.TransactionType == TransactionTypeEnum.PO)
                                    && (p.ORNumber != null || p.SINumber != null || p.WHDRNumber != null)
                                    && p.Deliveries.Count() == 0).Count();


            //_context.STOrders.AsNoTracking()
            //                        .Include(p => p.Deliveries)
            //                        .Include(p => p.Warehouse)
            //                        .Where(p => p.StoreId == storeId
            //                                    && p.RequestStatus == RequestStatusEnum.Approved
            //                                    && (p.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder || p.DeliveryType != DeliveryTypeEnum.Pickup)
            //                                    && (p.ORNumber != null || p.SINumber != null || p.WHDRNumber != null)
            //                                    && p.Deliveries.Count() == 0).Count();


            var pendingForApproval = orders.Where(p => 
                                               p.OrderToStoreId == storeId 
                                               && p.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                               && (p.ORNumber == null && p.WHDRNumber == null && p.SINumber == null)).Count();

            //_context.STOrders.AsNoTracking()
            //                       .Where(p => p.OrderToStoreId == storeId
            //                                   && p.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
            //                                   && p.RequestStatus == RequestStatusEnum.Approved
            //                                   && (p.ORNumber == null && p.WHDRNumber == null && p.SINumber == null)).Count();


            return new
            {
                OrdersWaitingForAddDeliveriesTotal = waitingForAddDel,
                OrdersWaitingForDeliveriesTotal = waitingForDel,
                PendingBranchOrdersApprovalTotal = pendingForApproval,
                NotificationsTotal = waitingForDel + pendingForApproval,

            };
        }


        public object GetPendingBranchOrdersApproval(int? storeId)
        {
            int total = _context.STOrders.AsNoTracking()
                                    .Where(p => p.OrderToStoreId == storeId
                                                && p.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                && p.RequestStatus == RequestStatusEnum.Approved
                                                && (p.ORNumber == null && p.WHDRNumber == null && p.SINumber == null)).Count();

            return new
            {
                NotificationsTotal = total,
                PendingBranchOrdersApprovalTotal = total
            };
        }


        public object GetStocksSummary(int? storeId)
        {
            InventorySearchDTO dto = new InventorySearchDTO();
            dto.StoreId = storeId;

            IQueryable<STStockSummary> inventory = (IQueryable<STStockSummary>)_stockService.GetInventoriesByStoreId(dto);

            var record = new
            {
                CurrentStocks = inventory.Sum(p => p.OnHand),
                AvailableStocks = inventory.Sum(p => p.Available),
                OutOfStockItems = inventory.Where(p => p.Available == 0).Count(),
                BrokenItems = inventory.Sum(p => p.Broken)
            };

            return record;
        }

        public object GetPendingOrders(int? storeId)
        {
            IQueryable<STOrder> query = _context.STOrders
                                                .Include(p => p.OrderedItems)
                                                .Where(p => p.StoreId == storeId &&
                                                            p.RequestStatus == RequestStatusEnum.Pending);


            var record = new
            {
                NotificationsTotal = query.Count(),
                PendingOrdersTotal = query.Count(),
                PendingOrdersTotalItem = query.Select(x => x.OrderedItems.Sum(y => y.RequestedQuantity)).Sum(z => z)
            };

            return record;

        }

        public object GetPendingDeliveries(int? storeId, IMapper mapper)
        {
            var query = _logisticsDashboardService.DeliveryRecords(mapper, storeId, IdTypeEnum.StoreId);
            var records = from x in query
                          select new
                          {
                              x.Id,
                              x.STOrderId,
                              TransactionType = (x.Order != null) ? x.Order.TransactionType : null,
                              OrderType = (x.Order != null) ? x.Order.OrderType : null,

                              DeliveryType = (x.Order != null) ? x.Order.DeliveryType : DeliveryTypeEnum.Delivery,

                              DeliveryStatus = (x.ShowroomDeliveries.Count > 0) ? _context.STShowroomDeliveries.Where(y => y.STDeliveryId == x.Id).Select(z => z.DeliveryStatus).FirstOrDefault()
                                                : _context.STClientDeliveries.Where(y => y.STDeliveryId == x.Id).Select(z => z.DeliveryStatus).FirstOrDefault(),
                              x.ApprovedDeliveryDate,
                              DeliveryQty = (x.ShowroomDeliveries != null && x.ShowroomDeliveries.Count() > 0)
                                                ? x.ShowroomDeliveries.Sum(p => p.Quantity)
                                                : x.ClientDeliveries.Sum(p => p.Quantity),

                              RequestStatus = (x.Order != null) ? x.Order.RequestStatus : null
                          };

            var totalRecord = records.Count(x =>
                            x.DeliveryStatus == DeliveryStatusEnum.Pending &&
                            x.RequestStatus == RequestStatusEnum.Approved);


            var record = new
            {
                NotificationsTotal = totalRecord,
                PendingDeliveriesTotal = totalRecord,
                PendingDeliveriesTotalItem = records.Where(x =>
                            x.DeliveryStatus == DeliveryStatusEnum.Pending &&
                            x.RequestStatus == RequestStatusEnum.Approved)
                            .Sum(x => x.DeliveryQty)
            };

            return record;

        }

        public object GetWaitingDeliveries(int? storeId, IMapper mapper)
        {
            var query = _logisticsDashboardService.DeliveryRecords(mapper, storeId, IdTypeEnum.StoreId);
            var records = (from x in query
                          select new
                          {
                              x.Id,
                              x.STOrderId,
                              TransactionType = (x.Order != null) ? x.Order.TransactionType : null,
                              OrderType = (x.Order != null) ? x.Order.OrderType : null,

                              DeliveryType = (x.Order != null) ? x.Order.DeliveryType : DeliveryTypeEnum.Delivery,

                              DeliveryStatus = (x.Order != null) ? _context.STShowroomDeliveries.Where(y => y.STDeliveryId == x.Id).Select(z => z.DeliveryStatus).FirstOrDefault() : null,
                              x.ApprovedDeliveryDate,
                              DeliveryQty = (x.ShowroomDeliveries != null && x.ShowroomDeliveries.Count() > 0)
                                                ? x.ShowroomDeliveries.Sum(p => p.Quantity)
                                                : x.ClientDeliveries.Sum(p => p.Quantity),

                              RequestStatus = (x.Order != null) ? x.Order.RequestStatus : null
                          }).ToList();

            var totalRecord = records.Count(x =>
                            x.ApprovedDeliveryDate != null &&
                            x.RequestStatus == RequestStatusEnum.Approved &&
                            x.DeliveryStatus == DeliveryStatusEnum.Waiting &&
                            (x.DeliveryType == DeliveryTypeEnum.Delivery || x.DeliveryType == DeliveryTypeEnum.ShowroomPickup));

            var record = new
            {
                NotificationsTotal = totalRecord,
                WaitingDeliveriesTotal = totalRecord,
                WaitingDeliveriesTotalItem = records.Where(x =>
                            x.ApprovedDeliveryDate != null &&
                            x.RequestStatus == RequestStatusEnum.Approved &&
                            x.DeliveryStatus == DeliveryStatusEnum.Waiting &&
                            (x.DeliveryType == DeliveryTypeEnum.Delivery || x.DeliveryType == DeliveryTypeEnum.ShowroomPickup))
                            .Sum(x => x.DeliveryQty)
            };

            return record;

        }

        public object GetWaitingForPickUp(int? storeId)
        {
            IQueryable<STOrder> query = _context.STOrders
                                                .Include(p => p.OrderedItems)
                                                .Where(p => p.StoreId == storeId &&
                                                            p.RequestStatus == RequestStatusEnum.Approved &&
                                                            p.OrderedItems.Where(x => x.ReleaseStatus == ReleaseStatusEnum.Waiting).Count() > 0 &&
                                                            p.DeliveryType == DeliveryTypeEnum.Pickup);



            var record = new
            {
                NotificationsTotal = query.Count(),
                WaitingForPickUpTotal = query.Count(),
                WaitingForPickUpTotalItem = query.Select(x => x.OrderedItems.Sum(y => y.ApprovedQuantity)).Sum(z => z)
            };

            return record;

        }

        public object GetPendingSales(int? storeId)
        {
            IQueryable<STSales> query = _context.STSales
                                                .Include(p => p.SoldItems)
                                                .Where(p => p.StoreId == storeId &&
                                                            p.ORNumber == null &&
                                                            p.SalesType == SalesTypeEnum.Releasing);


            var record = new
            {
                NotificationsTotal = query.Count(),
                PendingSalesTotal = query.Count(),
                PendingSalesTotalItem = query.Select(x => x.SoldItems.Sum(y => y.Quantity)).Sum(z => z)
            };

            return record;

        }

        public IEnumerable<object> GetStoreStockAlerts(int? storeId)
        {
            InventorySearchDTO dto = new InventorySearchDTO();
            dto.StoreId = storeId;

            IEnumerable<STStockSummary> inventory = (IEnumerable<STStockSummary>)_stockService.GetInventoriesByStoreId(dto);

            inventory = inventory.OrderBy(p => p.Available).Take(5);

            var record = inventory.OrderBy(p => p.Available).ToList();
            return record;
        }

        public object GetPendingToReceiveReturns(int? storeId)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.ClientPurchasedItems)
                                                 .Where(p => p.StoreId == storeId &&
                                                        p.ReturnType == ReturnTypeEnum.ClientReturn);

            var retList = new List<STReturn>();
            foreach (var x in query)
            {
                List<STClientReturn> clientPurchasedItems = null;

                if (x.ClientReturnType == ClientReturnTypeEnum.StoreReturn)
                {
                    clientPurchasedItems = x.ClientPurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Pending).ToList();
                }
                else
                {
                    clientPurchasedItems = x.ClientPurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList();
                }

                if (clientPurchasedItems != null && clientPurchasedItems.Count > 0)
                {
                    STReturn obj = new STReturn
                    {
                        ClientPurchasedItems = clientPurchasedItems
                    };

                    retList.Add(obj);

                }
            }

            var record = new
            {
                NotificationsTotal = retList.Count(),
                PendingToReceiveReturnsTotal = retList.Count(),
                PendingToReceiveReturnsTotalItems = retList.Select(x => x.ClientPurchasedItems.Sum(y => y.Quantity)).Sum(z => z)
            };

            return record;

        }

        public Dictionary<string, int> ObjectSummary(List<object> data)
        {

            var notificationsTotal = 0;
            var results = new Dictionary<string, int>();

            for (int i = 0; i < data.Count; i++)
            {
                foreach (System.Reflection.PropertyInfo fi in data[i].GetType().GetProperties())
                {
                    if (fi.Name == "NotificationsTotal")
                    {
                        notificationsTotal += Convert.ToInt32(fi.GetValue(data[i], null));
                    }
                    results[fi.Name] = Convert.ToInt32(fi.GetValue(data[i], null));
                }
            }

            results["NotificationsTotal"] = notificationsTotal;
            return results;
        }

    }
}

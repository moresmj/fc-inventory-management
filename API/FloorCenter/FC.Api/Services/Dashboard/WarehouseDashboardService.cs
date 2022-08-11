using AutoMapper;
using FC.Api.DTOs.Inventories;
using FC.Api.DTOs.Store;
using FC.Api.DTOs.Warehouse;
using FC.Api.Helpers;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Dashboard
{
    public class WarehouseDashboardService : IWarehouseDashboardService
    {
        private DataContext _context;
        private IWHStockService _warehouseStockService;
        private ILogisticsDashboardService _logisticsDashboardService;

        public WarehouseDashboardService(DataContext context, IWHStockService warehouseStockService, ILogisticsDashboardService logisticsDashboardService)
        {
            _context = context;
            _warehouseStockService = warehouseStockService;
            _logisticsDashboardService = logisticsDashboardService;
        }

        public object GetStocksSummary(int? warehouseId)
        {
            InventorySearchDTO dto = new InventorySearchDTO();
            dto.WarehouseId = warehouseId;

            List<WHStockSummary> inventory = (List<WHStockSummary>)this._warehouseStockService.GetInventoriesByWarehouseId(dto);

            var record = new
            {
                CurrentStocks = inventory.Sum(p => p.OnHand),
                AvailableStocks = inventory.Sum(p => p.Available),
                OutOfStockItems = inventory.Where(p => p.Available == 0).Count(),
                BrokenItems = inventory.Sum(p => p.Broken),
                StockAlertList = inventory.OrderBy(p => p.Available).Take(5)
            };

            return record;
        }


        public object DashboardSummary(int? warehouseId, IMapper mapper)
        {
            //List<object> results = new List<object>();


            //results.Add(GetWaitingDeliveries(warehouseId, mapper));
            //results.Add(GetWaitingForPickUp(warehouseId));

            //results.Add(GetWarehouseRTVApproval(warehouseId));
            //results.Add(GetStocksSummary(warehouseId));

            var notification = _context.WarehouseNotifSummary.FromSql("SELECT * FROM  dbo.GetWarehouseNotificationSumary({0})", warehouseId).FirstOrDefault();
          



            return notification;
            //return ObjectSummary(results);

        }



        public object GetApproveOrders(int? warehouseId)
        {
            //IQueryable<STOrderDetail> query = _context.STOrderDetails
            //                                    .Include(p => p.STOrder)
            //                                    .Where(p =>
            //                                                p.STOrder.WarehouseId == warehouseId &&
            //                                                p.STOrder.RequestStatus == RequestStatusEnum.Approved &&
            //                                                (p.STOrder.OrderType == OrderTypeEnum.ShowroomStockOrder || p.STOrder.OrderType == OrderTypeEnum.ClientOrder) &&
            //                                                p.ReleaseStatus != ReleaseStatusEnum.Released);



            //var record = new
            //{r
            //    ApproveRequestTotal = query.Count(),
            //    ApproveRequestItemTotal = query.Sum(p => p.ApprovedQuantity)
            //};

            return null;
        }


        public object GetWaitingDeliveries(int? warehouseId, IMapper mapper)
        {

            IQueryable<STOrder> query = _context.STOrders
                                            .Where(p => p.WarehouseId == warehouseId && p.DeliveryType != DeliveryTypeEnum.Pickup)
                                            .Include(p => p.Deliveries)
                                                .ThenInclude(p => p.ShowroomDeliveries)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size);

            List<STOrder> retList = new List<STOrder>();

            foreach (var r in query)
            {

                var order = new STOrder
                {
                    Id = r.Id,
                    TransactionNo = r.TransactionNo,
                    StoreId = r.StoreId,
                    Store = _context.Stores.Where(p => p.Id == r.StoreId).FirstOrDefault(),
                    TransactionType = r.TransactionType,
                    DeliveryType = r.DeliveryType,
                    WarehouseId = r.WarehouseId,
                    Warehouse = _context.Warehouses.Where(p => p.Id == r.WarehouseId).FirstOrDefault(),
                    PONumber = r.PONumber,
                    PODate = r.PODate,
                    Remarks = r.Remarks,
                    OrderType = r.OrderType,
                    ClientName = r.ClientName,
                    ContactNumber = r.ContactNumber,
                    Address1 = r.Address1,
                    Address2 = r.Address2,
                    Address3 = r.Address3,
                    RequestStatus = r.RequestStatus,
                    WHDRNumber = r.WHDRNumber,
                };

                order.Deliveries = new List<STDelivery>();

           
                foreach (var del in r.Deliveries)
                {
                    var delivery = new STDelivery
                    {
                        Id = del.Id,
                        ApprovedDeliveryDate = del.ApprovedDeliveryDate,
                        DeliveryDate = del.DeliveryDate,
                        DriverName = del.DriverName,
                        DRNumber = del.DRNumber,
                        PlateNumber = del.PlateNumber,
                        STOrderId = del.STOrderId,
                        StoreId = del.StoreId,
                        ShowroomDeliveries = new List<STShowroomDelivery>(),
                        ClientDeliveries = new List<STClientDelivery>()
                    };

                    if (order.OrderType == OrderTypeEnum.ShowroomStockOrder || order.DeliveryType == DeliveryTypeEnum.ShowroomPickup)
                    {
                        GetShowroomDeliveriesForReleasing(order, del, delivery);
                    }
                    else
                    {
                         GetClientDeliveriesForReleasing(order, del, delivery);
                    }

                }

                if (order.Deliveries != null && order.Deliveries.Count > 0)
                {
                    retList.Add(order);
                }
              
            }

            var record = new
            {
                NotificationsTotal = retList.Sum(p => p.Deliveries.Count),
                WaitingDeliveriesTotal = retList.Sum(p => p.Deliveries.Count),
                WaitingDeliveriesTotalItem = retList.Select(x => x.Deliveries.Select(y => y.ShowroomDeliveries.Sum(z => z.Quantity)).Sum()).Sum() + retList.Select(x => x.Deliveries.Select(y => y.ClientDeliveries.Sum(z => z.Quantity)).Sum()).Sum()

            };


            return record;


        }


        public object GetWaitingForPickUp(int? warehouseId)
        {
            IQueryable<STOrder> query = _context.STOrders
                                                .Include(p => p.OrderedItems).AsNoTracking()
                                                .Where(p => p.WarehouseId == warehouseId &&
                                                            p.RequestStatus == RequestStatusEnum.Approved &&
                                                            p.DeliveryType == DeliveryTypeEnum.Pickup &&
                                                            p.OrderedItems.Where(x => x.ReleaseStatus != ReleaseStatusEnum.Released).Count() > 0 &&
                                                            p.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder)
                                                            .Include(p => p.Deliveries)
                                                                .ThenInclude(p => p.ShowroomDeliveries)
                                                                    .ThenInclude(p => p.Item)
                                                                        .ThenInclude(p => p.Size);




            List<STOrder> retList = new List<STOrder>();


            foreach (var r in query)
            {

                var order = new STOrder
                {
                    Id = r.Id,
                    TransactionNo = r.TransactionNo,
                    StoreId = r.StoreId,
                    Store = _context.Stores.Where(p => p.Id == r.StoreId).FirstOrDefault(),
                    TransactionType = r.TransactionType,
                    DeliveryType = r.DeliveryType,
                    WarehouseId = r.WarehouseId,
                    Warehouse = _context.Warehouses.Where(p => p.Id == r.WarehouseId).FirstOrDefault(),
                    PONumber = r.PONumber,
                    PODate = r.PODate,
                    Remarks = r.Remarks,
                    OrderType = r.OrderType,
                    ClientName = r.ClientName,
                    ContactNumber = r.ContactNumber,
                    Address1 = r.Address1,
                    Address2 = r.Address2,
                    Address3 = r.Address3,
                    RequestStatus = r.RequestStatus,
                    WHDRNumber = r.WHDRNumber,
                };

                order.Deliveries = new List<STDelivery>();
                foreach (var del in r.Deliveries)
                {
                    var delivery = new STDelivery
                    {
                        Id = del.Id,
                        ApprovedDeliveryDate = del.ApprovedDeliveryDate,
                        DeliveryDate = del.DeliveryDate,
                        DriverName = del.DriverName,
                        DRNumber = del.DRNumber,
                        PlateNumber = del.PlateNumber,
                        STOrderId = del.STOrderId,
                        StoreId = del.StoreId,
                        ShowroomDeliveries = new List<STShowroomDelivery>(),
                        ClientDeliveries = new List<STClientDelivery>()
                    };

                        GetClientPickUpForReleasing(order, del, delivery);

                        
                }
                

                if (order.Deliveries != null && order.Deliveries.Count > 0)
                {
                    retList.Add(order);
                }

            }



            var rec = new
            {
                NotificationsTotal = retList.Sum(p => p.Deliveries.Count),
                WaitingForPickUpTotal = retList.Sum(p => p.Deliveries.Count),
                WaitingForPickUpTotalItem = retList.Select(x => x.Deliveries.Select(y => y.ClientDeliveries.Sum(z => z.Quantity)).Sum()).Sum()/* query.Select(x => x.OrderedItems.Sum(y => y.ApprovedQuantity)).Sum(z => z)*/
            };

            return rec;

        }


        private void GetClientPickUpForReleasing(STOrder order, STDelivery del, STDelivery delivery)
        {
            if (del.ClientDeliveries == null)
            {
                del.ClientDeliveries = _context.STClientDeliveries
                                                .Include(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Where(p => p.STDeliveryId == del.Id)
                                            .ToList();
            }


            if (del.ClientDeliveries != null && del.ClientDeliveries.Count > 0)
            {
                foreach (var client in del.ClientDeliveries.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting && p.ReleaseStatus == ReleaseStatusEnum.Waiting))
                {
                    delivery.ClientDeliveries.Add(client);
                }

                if (delivery.ClientDeliveries != null && delivery.ClientDeliveries.Count > 0)
                {
                    order.Deliveries.Add(delivery);
                }
            }
        }

        private void GetShowroomDeliveriesForReleasing(STOrder order, STDelivery del, STDelivery delivery)
        {
            if (del.ShowroomDeliveries == null)
            {
                del.ShowroomDeliveries = _context.STShowroomDeliveries
                                                .Include(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Where(p => p.STDeliveryId == del.Id)
                                            .ToList();
            }


            if (del.ShowroomDeliveries != null && del.ShowroomDeliveries.Count > 0)
            {
                foreach (var show in del.ShowroomDeliveries.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting && p.ReleaseStatus == ReleaseStatusEnum.Waiting))
                {
                    delivery.ShowroomDeliveries.Add(show);
                }

                if (delivery.ShowroomDeliveries != null && delivery.ShowroomDeliveries.Count > 0)
                {
                    order.Deliveries.Add(delivery);
                }
            }
        }

        private void GetClientDeliveriesForReleasing(STOrder order, STDelivery del, STDelivery delivery)
        {
            if (del.ClientDeliveries == null)
            {
                del.ClientDeliveries = _context.STClientDeliveries
                                                .Include(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Where(p => p.STDeliveryId == del.Id)
                                            .ToList();
            }


            if (del.ClientDeliveries != null && del.ClientDeliveries.Count > 0)
            {
                foreach (var client in del.ClientDeliveries.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting && p.ReleaseStatus == ReleaseStatusEnum.Waiting))
                {
                    delivery.ClientDeliveries.Add(client);
                }

                if (delivery.ClientDeliveries != null && delivery.ClientDeliveries.Count > 0)
                {
                    order.Deliveries.Add(delivery);
                }
            }
        }

        public IEnumerable<object> GetWarehouseStockAlerts(int? warehouseId)
        {
            InventorySearchDTO dto = new InventorySearchDTO();
            dto.WarehouseId = warehouseId;

            List<WHStockSummary> inventory = (List<WHStockSummary>)this._warehouseStockService.GetInventoriesByWarehouseId(dto);

            var record = inventory.OrderBy(p => p.Available).ToList();
            return record.Take(5);
        }

        public object GetWarehouseRTVApproval(int? warehouseId)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                .Include(p => p.PurchasedItems)
                                                .Where(p => p.WarehouseId == warehouseId &&
                                                            p.RequestStatus == RequestStatusEnum.Pending);

            var brokenQuantity = query.Select(x => x.PurchasedItems.Sum(y => y.BrokenQuantity)).Sum(z => z);
            var goodQuantity = query.Select(x => x.PurchasedItems.Sum(y => y.GoodQuantity)).Sum(z => z);

            var record = new
            {
                NotificationsTotal = query.Count(),
                WaitingRTVApprovalTotal = query.Count(),
                WaitingRTVApprovalTotalItem = brokenQuantity + goodQuantity
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

        private IQueryable<STOrder> GetApprovedOrderedItem(int? itemId, int? warehouseId)
        {
            var records = _context.STOrders
                                  .Include(p => p.OrderedItems)
                                  .Where(p => p.WarehouseId == warehouseId
                                              && p.RequestStatus == RequestStatusEnum.Approved);

            var orders = new List<STOrder>();
            foreach (var rec in records)
            {
                if (rec.OrderedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting
                                                && p.ReleaseStatus == ReleaseStatusEnum.Waiting
                                                && p.ItemId == itemId)
                                    .Count() > 0)
                {
                    orders.Add(rec);
                }
            }

            return orders.AsQueryable();
        }


    }
}

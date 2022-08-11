using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.DTOs.Warehouse;
using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Dashboard
{
    public class LogisticsDashboardService : ILogisticsDashboardService
    {

        private DataContext _context;

        public LogisticsDashboardService(DataContext context)
        {
            _context = context;
        }


        public object DashboardSummary(IMapper mapper)
        {
            //List<object> results = new List<object>();

            //results.Add(GetPendingDeliveryOrders(mapper));
            //results.Add(GetPendingDeliverySales());
            //results.Add(GetPendingPickUpClientReturns());
            //results.Add(GetPendingPickUpRTV());

            //return ObjectSummary(results);

            var results = _context.LogisticsNotifSummary.FromSql("SELECT * FROM dbo.GetLogisticsNotifSummary()").FirstOrDefault();

            return results;

        }


        public object GetPendingDeliveryOrders(IMapper mapper)
        {
            var query = DeliveryRecords(mapper);
            var records = from x in query
                          select new
                          {
                              x.ApprovedDeliveryDate,
                              DeliveryQty = (x.ShowroomDeliveries != null && x.ShowroomDeliveries.Count() > 0)
                                                ? x.ShowroomDeliveries.Sum(p => p.Quantity)
                                                : x.ClientDeliveries.Sum(p => p.Quantity),

                              RequestStatus = (x.Order != null) ? x.Order.RequestStatus : null,
                          };

            var PendingOrderDeliveriesTotal = records.Count(x => x.ApprovedDeliveryDate == null && (x.RequestStatus == RequestStatusEnum.Approved/* || x.RequestStatus == null*/));
            var WaitingOrderDeliveriesTotal = records.Count(x => x.ApprovedDeliveryDate != null && (x.RequestStatus == RequestStatusEnum.Approved || x.RequestStatus == null));


            var record = new
            {
                NotificationsTotal = PendingOrderDeliveriesTotal ,
                PendingOrderDeliveriesTotal,
                PendingOrderDeliveriesTotalItem = records.Where(x => x.ApprovedDeliveryDate == null).Sum(x => x.DeliveryQty),
                WaitingOrderDeliveriesTotal,
                WaitingDeliveriesTotalItem = records.Where(x => x.ApprovedDeliveryDate != null).Sum(x => x.DeliveryQty)
            };

            return record;


        }


        public object GetPendingDeliverySales()
        {
            IQueryable<STDelivery> query = _context.STDeliveries.AsNoTracking()
                                           .Include(p => p.ClientDeliveries)
                                           .Where(p => p.STSalesId != null &&
                                                    (p.ClientDeliveries.Where(x => x.DeliveryStatus == DeliveryStatusEnum.Pending).Count() > 1));

            return new
            {
                NotificationsTotal = query.Count(),
                PendingSalesDeliveryTotal = query.Count(),
                PendingSalesDeliveryTotalItem = query.Sum(x => x.ClientDeliveries.Sum(y => y.Quantity))
            };
        }

        public object GetPendingPickUpClientReturns()
        {
            IQueryable<STReturn> query = _context.STReturns.AsNoTracking()
                                     .Include(p => p.Store)
                                     .Include(p => p.ClientPurchasedItems)
                                     .Where
                                     (p =>
                                        (p.ReturnType == ReturnTypeEnum.ClientReturn
                                        && p.ClientReturnType == ClientReturnTypeEnum.RequestPickup
                                        && (p.ClientPurchasedItems.Where(x => x.DeliveryStatus == DeliveryStatusEnum.Pending).Count() > 0))
                                     );
 

            return new
            {
                NotificationsTotal = query.Count(),
                PendingPickUpClientReturnsTotal = query.Count(),
                PendingPickUpClientReturnsTotalItem = query.Sum(x => x.ClientPurchasedItems.Sum(y => y.Quantity))
            };
        }

        public object GetPendingPickUpRTV()
        {
            IQueryable<STReturn> query = _context.STReturns.AsNoTracking()
                                       .Include(p => p.Deliveries)
                                          .ThenInclude(p => p.WarehouseDeliveries)
                                       .Where
                                       (p =>
                                           (p.RequestStatus == RequestStatusEnum.Approved || (p.RequestStatus == RequestStatusEnum.Pending && p.ReturnType == ReturnTypeEnum.Breakage))
                                       );

            return new
            {
                NotificationsTotal = query.Sum(p => p.Deliveries.Sum(x => x.WarehouseDeliveries.Where(y=>y.DeliveryStatus== DeliveryStatusEnum.Pending).Count())),
                PendingPickUpRTVTotal = query.Sum(p => p.Deliveries.Sum(x => x.WarehouseDeliveries.Where(y => y.DeliveryStatus == DeliveryStatusEnum.Pending).Count())),
                PendingPickUpRTVTotalItem = query.Sum(p => p.Deliveries.Sum(x => x.WarehouseDeliveries.Where(y => y.DeliveryStatus == DeliveryStatusEnum.Pending).Sum(y => y.Quantity)))
            };

        }


        public IEnumerable<STDeliveryDTO> DeliveryRecords(IMapper mapper, int? id = null, IdTypeEnum? idType = null)
        {

            IQueryable<STDelivery> list = _context.STDeliveries.AsNoTracking()
                                                  .Include(p => p.ShowroomDeliveries)
                                                  .Include(p => p.ClientDeliveries);


            if (idType == IdTypeEnum.StoreId && id.HasValue)
            {
                list = list.Where(x => x.StoreId == id);
            }


            var deliveries = new List<STDeliveryDTO>();

            foreach (var del in list)
            {
                var mappedDelivery = mapper.Map<STDeliveryDTO>(del);

                var order = _context.STOrders.Where(p => p.Id == del.STOrderId).FirstOrDefault();
                if (order != null)
                {
                    var mappedOrder = mapper.Map<STOrderDTO>(order);
                    mappedDelivery.Order = mappedOrder;


                    var warehouse = _context.Warehouses.Where(p => p.Id == order.WarehouseId).FirstOrDefault();
                    if (warehouse != null)
                    {
                        var mappedWarehouse = mapper.Map<WarehouseDTO>(warehouse);
                        mappedDelivery.Order.Warehouse = mappedWarehouse;
                       
                    }

                    var store = _context.Stores.Where(p => p.Id == del.StoreId).FirstOrDefault();
                    if (store != null)
                    {
                        var mappedStore = mapper.Map<StoreDTO>(store);
                        mappedDelivery.Order.Store = mappedStore;
                    }

                    if (warehouse != null && warehouse.Vendor)
                    {
                        //skip vendor sales
                    }
                    else
                    {
                        deliveries.Add(mappedDelivery);
                    }

                }
                else
                {
                    var store = _context.Stores.Where(p => p.Id == del.StoreId).FirstOrDefault();
                    if (store != null)
                    {
                        var mappedStore = mapper.Map<StoreDTO>(store);
                        mappedDelivery.Store = mappedStore;
                    }
                    deliveries.Add(mappedDelivery);
                }
                
            }

            // Warehouse Filtering
            if (idType == IdTypeEnum.WarehouseId && id.HasValue)
            {
                deliveries = deliveries.Where(x => x.Order?.WarehouseId == id).ToList();
            }

            return deliveries.AsEnumerable(); ;

        }

        private Dictionary<string, int> ObjectSummary(List<object> data)
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

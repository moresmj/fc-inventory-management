using FC.Api.DTOs.Inventories;
using FC.Api.DTOs.Store;
using FC.Api.DTOs.Store.Returns;
using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FC.Api.Services.Stores.PhysicalCount;
using FC.Api.Services.Stores.Returns;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Dashboard
{
    public class MainDashboardService : IMainDashboardService
    {

        private DataContext _context;
        private ISTStockService _stockService;
        private IReturnService _returnService;
        private IUploadPhysicalCountService _storeUploadPhysicalCountService;
        private Services.Warehouses.PhysicalCount.IUploadPhysicalCountService _warehouseUploadPhysicalCountService;
        private ISTOrderService _orderService;

        public MainDashboardService(DataContext context, ISTStockService stockService, IReturnService returnService, IUploadPhysicalCountService storeUploadPhysicalCountService,Services.Warehouses.PhysicalCount.IUploadPhysicalCountService warehouseUploadPhysicalCountService,
            ISTOrderService orderService)
        {
            _context = context;
            _stockService = stockService;
            _returnService = returnService;
            _storeUploadPhysicalCountService = storeUploadPhysicalCountService;
            _warehouseUploadPhysicalCountService = warehouseUploadPhysicalCountService;
            _orderService = orderService;
        }

        public object NotificationdSummary()
        {
            //List<object> results = new List<object>();

            //results.Add(GetStocksSummary());
            //results.Add(GetReturnPendingSummary());
            //results.Add(GetApproveRequestSummary());
            //results.Add(GetAdjustmentStore());
            //results.Add(GetPendingTransfers());
            //results.Add(GetPendingAssignDr());

            //return ObjectSummary(results);

            var result = _context.MainNotifSummary.FromSql("SELECT * FROM dbo.GetMainPageNotifSummary()").FirstOrDefault();

            result.NotificationsTotal = result.approveRequestTotal + result.pendingAssignDrTotal + result.pendingReturnsTotal + result.pendingTransferTotal + result.storeAdjustmentTotal;

            return result;

        }

        public object DashboardSummary()
        {
            
            var result = _context.MainDashboardSummary.FromSql("SELECT * FROM dbo.GetMainPageDashBoardItemCountSummary()").FirstOrDefault();

       
            return result;

        }

        public object GetStocksSummary()
        {
            List<int?> storeIds = new List<int?>();
            storeIds = _context.STStocks.Where(p => p.StoreId != null).Select(p => p.StoreId).Distinct().ToList();


            List<object> records = new List<object>();

            var currentStocks = 0;
            var availableStocks = 0;
            var outOfStockItems = 0;
            var brokenItems = 0;

            InventorySearchDTO dto = new InventorySearchDTO();

            for (int i = 0; i < storeIds.Count(); i++)
            {
                dto.StoreId = storeIds[i];

                List<STStockSummary> inventory = (List<STStockSummary>)_stockService.GetInventoriesByStoreId(dto);
                currentStocks += inventory.Sum(p => p.OnHand);
                availableStocks += inventory.Sum(p => p.Available);
                outOfStockItems += inventory.Where(p => p.Available == 0).Count();
                brokenItems += inventory.Sum(p => p.Broken);
            }

            var record = new
            {
                CurrentStocks = currentStocks,
                AvailableStocks = availableStocks,
                OutOfStockItems = outOfStockItems,
                BrokenItems = brokenItems,
            };

            return record;
        }

        public object GetApproveRequestSummary()
        {
            IQueryable<STOrder> query = _context.STOrders
                                                .Include(p => p.OrderedItems)
                                                .Where(p => p.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder && p.RequestStatus == RequestStatusEnum.Pending);



            var record = new
            {
                NotificationsTotal = query.Count(),
                ApproveRequestTotal = query.Count(),
                ApproveRequestItemTotal = query.Select(x => x.OrderedItems.Sum(y => y.RequestedQuantity)).Sum(z => z)
            };

            return record;
        }

        public object GetReturnPendingSummary()
        {

            IQueryable<STReturn> query = _context.STReturns
                                     .Include(p => p.PurchasedItems)
                                            .Where(p => p.ReturnType != ReturnTypeEnum.ClientReturn && p.RequestStatus == RequestStatusEnum.Pending);


            var returns = BuildList(query);


            var returItems = returns.ToList();
            var totalQty = 0;
            for (int i = 0; i < returns.ToList().Count(); i++)
            {
                
               var test = returItems[i];
               totalQty += (int)test;
               
            }

            var record = new
            {
                NotificationsTotal = returns.ToList().Count(),
                PendingReturnsTotal = returns.ToList().Count(),
                PendingReturnsTotalItem = totalQty
            };
     

            return record;

        }

        public object GetAdjustmentStore()
        {

            List<int> storeAdjustmentId = new List<int>();
            storeAdjustmentId = _context.STImports.Select(p => p.Id).Distinct().ToList();
            var param = new ApprovePhysicalCountSearchDTO();
         
            param.RequestStatus = new RequestStatusEnum?[1];
            param.RequestStatus[0] = RequestStatusEnum.Pending;

            var itemList = _storeUploadPhysicalCountService.GetAll(param);
            


            var adjustItemCount = 0;
 

            for (int i = 0; i < storeAdjustmentId.Count(); i++)
            {

                adjustItemCount += _storeUploadPhysicalCountService.GetById2(storeAdjustmentId[i]);


            }

            var record = new
            {
                NotificationsTotal = itemList.Count(),
                StoreAdjustmentTotal = itemList.Count(),
                StoreAdjustmentTotalItem = adjustItemCount
            };

            return record;

        }


        public object GetAdjustmentWarehouse()
        {

            List<int> warehouseAdjustmentId = new List<int>();
            warehouseAdjustmentId = _context.WHImports.Select(p => p.Id).Distinct().ToList();
            var itemList = _warehouseUploadPhysicalCountService.GetAll();



            var adjustItemCount = 0;


            for (int i = 0; i < warehouseAdjustmentId.Count(); i++)
            {

                adjustItemCount += _warehouseUploadPhysicalCountService.GetById2(warehouseAdjustmentId[i]);


            }

            var record = new
            {
                NotificationsTotal = itemList.Count(),
                WarehouseAdjustmentTotal = itemList.Count(),
                WarehouseAdjustmentTotalItem = adjustItemCount
            };

            return record;

        }

        public object  GetPendingTransfers()
        {
            IQueryable<STOrder> query = _context.STOrders
                                               .Where(p => p.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder && p.RequestStatus == RequestStatusEnum.Pending);

            var record = new
            {
                NotificationsTotal = query.Count(),
                PendingTransferTotal = query.Count(),
                PendingTransferTotalItem = query.Select(x => x.OrderedItems.Sum(y => y.RequestedQuantity)).Sum(z => z)
            };

            return record;

        }

        public object GetPendingAssignDr()
        {

            IQueryable<STOrder> query = _context.STOrders
                                     .Include(p => p.OrderedItems)
                                     .Where(p =>
                                         (p.OrderType == OrderTypeEnum.ShowroomStockOrder || p.OrderType == OrderTypeEnum.ClientOrder) &&
                                         p.RequestStatus == RequestStatusEnum.Approved && p.WHDRNumber == null);


            var record = new
            {
                NotificationsTotal = query.Count(),
                PendingAssignDrTotal = query.Count(),
                PendingAssignDrTotalItem = query.Select(p => p.OrderedItems.Sum(x => x.ApprovedQuantity)).Sum(z =>z),

            };

                 
            return record;



        }


        private IEnumerable<object> BuildList(IQueryable<STReturn> query)
        {
            var retList = new List<object>();
            foreach (var x in query)
            {
                var Total = 0;

                var obj = new
                {
                    x.Id,
                    x.ReturnType,
                    Items = (x.ReturnType != ReturnTypeEnum.ClientReturn)
                            ?
                            x.PurchasedItems.Select(p => new
                            {
                                TotalQty = p.BrokenQuantity + p.GoodQuantity,
                                p.DeliveryStatus
                            })
                            :
                            null,
                    ClientPurchasedItems = (x.ReturnType == ReturnTypeEnum.ClientReturn)
                                           ?
                                            x.ClientPurchasedItems.Select(p => new
                                            {
                                                TotalQty = p.Quantity
                                            })
                                            : null
                };
                Total += obj.Items.Sum(p => (int)p.TotalQty);

                //added to check if breakage item is already delivered before displaying
                if (x.ReturnType == ReturnTypeEnum.Breakage)
                {
                    var delivered = 0;
                    foreach (var item in obj.Items)
                    {
                        if (item.DeliveryStatus == DeliveryStatusEnum.Delivered)
                        {
                            delivered++;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (delivered > 0)
                    {
                        retList.Add(Total);
                    }

                }
                else
                {
                    retList.Add(Total);
                }



            }

            return retList;
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

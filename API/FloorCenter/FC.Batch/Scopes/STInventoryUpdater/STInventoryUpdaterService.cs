using EFCore.BulkExtensions;
using FC.Api.DTOs.Warehouse;
using FC.Api.Services.Stores;
using FC.Api.Services.Warehouses;
using FC.Batch.DTOs.Inventories;
using FC.Batch.DTOs.Warehouse;
using FC.Batch.Helpers.DBContext;
using FC.Batch.Helpers.DBContext.Extension;
using FC.Core.Domain.Common;
using FC.Core.Domain.Items;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FC.Batch.Scopes.STInventoryUpdater
{

    public interface ISTInventoryUpdaterService : IScopedExecuteService
    {
    }

    internal class STInventoryUpdaterService
        : ISTInventoryUpdaterService
    {
        private FC.Api.Helpers.DataContext _context;

        private DateTime? currentTime { get; set; }
        private DateTime? timeFrom { get; set; }


        public IQueryable<Item> item { get; set; }
        public List<Store> stores { get; set; }
        public IQueryable<STOrder> stOrder { get; set; }
        public IQueryable<STStock> stStock { get; set; }
        public IQueryable<WHStock> whStock { get; set; }

        public IQueryable<STSales> stSales { get; set; }


        public STInventoryUpdaterService(
            FC.Api.Helpers.DataContext context)
        {
            _context = context;
        }

        public string ConfigSection => "Batch:Task:InventoryUpdater";

        public void Execute(CancellationToken cancellationToken, dynamic state)
        {
            //currentTime = DateTime.Now;
            //// 10 Minutes Before the current time
            //timeFrom = currentTime.Value.AddMinutes(-10);

            //item = _context.Items.AsNoTracking().Include(p => p.Size);
            //stOrder = _context.STOrders.AsNoTracking().Include(p => p.OrderedItems);


            //// Bulk Update and Insert Reference:
            //// https://github.com/borisdj/EFCore.BulkExtensions

            //// Update Store Stocks Summary
            //UpdateSTStockSummary();
            DbCommand cmd = _context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "SP_StoreStockSummaryUpdater";
            cmd.CommandType = CommandType.StoredProcedure;

            if (cmd.Connection.State != ConnectionState.Open)
            {
                cmd.Connection.Open();
            }
            cmd.ExecuteNonQuery();
            cmd.Connection.Dispose();
            cmd.Connection.Close();

        }

        private void UpdateSTStockSummary()
        {
            stores = _context.Stores.AsNoTracking().ToList();

            stStock = _context.STStocks.AsNoTracking();

            stSales = _context.STSales;

            int stStockSummaryCount = _context.STStockSummary.Count();

            if (stStockSummaryCount == 0)
            {
                this.PopulateStoreStockSummary();
            }
            else
            {
                // Fetch all items that were changed during this [Current Time and 10 Minutes Before]
                List<StoreChanges> stocksHasChanges = _context.STStocks.Where(p => (timeFrom <= p.DateCreated && currentTime.Value >= p.DateCreated) || (timeFrom <= p.DateUpdated && currentTime.Value >= p.DateUpdated) || (timeFrom <= p.ChangeDate && currentTime.Value >= p.ChangeDate))
                                        .Select(p => new StoreChanges
                                        {
                                            StoreId = p.StoreId,
                                            ItemId = p.ItemId
                                        }).Distinct()
                                        .ToList();

                // will check if changes is made on item table
                List<StoreChanges> updatedItem = _context.Items.Where(p => (timeFrom <= p.DateUpdated && currentTime.Value >= p.DateUpdated))
                  .Select(p => new StoreChanges
                  {
                      ItemId = p.Id,
                      StoreId = null
                  }).Distinct()
                  .ToList();

                if (updatedItem.Count > 0)
                {
                    var storeId = _context.Stores.Select(p => p.Id).ToList();

                    foreach (var item in updatedItem)
                    {

                        var newList = new List<StoreChanges>();
                        for (int i = 0; i < storeId.Count(); i++)
                        {
                            var newItem = new StoreChanges();
                            newItem.StoreId = storeId[i];
                            newItem.ItemId = item.ItemId;

                            newList.Add(newItem);
                        }
                        stocksHasChanges.AddRange(newList);

                    }

                }



                using (var transaction = _context.Database.BeginTransaction())
                {
                    for (int i = 0; i < stocksHasChanges.Count(); i++)
                    {
                        // Fetch only the records on the inventory that has changes
                        List<STStockSummary> records = (from y in _context.STStocks.AsNoTracking()
                                                        where stocksHasChanges.Where(p => p.StoreId == y.StoreId && p.ItemId == y.ItemId).Count() > 0
                                                        group y.Item by y.ItemId into x
                                                        select new STStockSummary
                                                        {
                                                            StoreId = stocksHasChanges[i].StoreId,
                                                            ItemId = x.Key,
                                                        }).ToList();

                        // Insert the record by List [Object == Database name]
                        _context.BulkInsertOrUpdate(this.GetInventoriesByStoreId(stocksHasChanges[i].StoreId, records, true), new BulkConfig { PreserveInsertOrder = true, SetOutputIdentity = true });
                    }
                    transaction.Commit();
                }

            }

        }


        #region Store

        public void PopulateStoreStockSummary()
        {
            List<int?> StoreIds = _context.STStocks.AsNoTracking().Select(p => p.StoreId).Distinct().ToList();

            using (var transaction = _context.Database.BeginTransaction())
            {
                for (int i = 0; i < StoreIds.Count(); i++)
                {
                    List<STStockSummary> records = (from y in _context.STStocks.AsNoTracking()
                                                    where y.StoreId == StoreIds[i]
                                                    group y.Item by y.ItemId into x
                                                    select new STStockSummary
                                                    {
                                                        StoreId = StoreIds[i],
                                                        ItemId = x.Key,
                                                    }).ToList();


                    _context.BulkInsertOrUpdate(this.GetInventoriesByStoreId(StoreIds[i], records), new BulkConfig { PreserveInsertOrder = true, SetOutputIdentity = true });
                }
                transaction.Commit();
            }
        }


        public List<STStockSummary> GetInventoriesByStoreId(int? storeId, List<STStockSummary> records, bool isPopulated = false)
        {

            var items = new List<STStockSummary>();

            for (int i = 0; i < records.Count(); i++)
            {
                var rec = records[i];

                // Record with ID means [Update] else [Insert]
                if (isPopulated)
                {
                    rec.Id = _context.STStockSummary.Where(p => p.StoreId == rec.StoreId && p.ItemId == rec.ItemId).Select(p => p.Id).FirstOrDefault();
                    rec.DateUpdated = DateTime.Now;
                }
                else
                {
                    rec.DateCreated = DateTime.Now;
                }

                // Item Details
                var itemDetails = item.Where(p => p.Id == rec.ItemId).FirstOrDefault();

                rec.SerialNumber = itemDetails.SerialNumber;
                rec.Code = itemDetails.Code;
                rec.ItemName = itemDetails.Name;
                rec.SizeId = itemDetails.SizeId;
                rec.SizeName = itemDetails.Size.Name;
                rec.Tonality = itemDetails.Tonality;
                rec.Description = itemDetails.Description;

                var totalItemForReleasing = this.GetItemForReleasingStore(rec.ItemId, storeId);

                rec.ForRelease = totalItemForReleasing;
                //3rd parameter set to true to be able to deduct for releasing items
                var totalItemAvailable = this.GetItemAvailableQuantityStore(rec.ItemId, storeId, true);

                var totalItemBroken = this.GetItemBrokenRegisterStore(rec.ItemId, storeId);
        
                var totalItemReceivedBroken = this.GetItemBrokenQuantityStore(rec.ItemId, storeId);

                rec.Broken = totalItemBroken + totalItemReceivedBroken;

                rec.OnHand = totalItemAvailable + rec.Broken + totalItemForReleasing;

                rec.Available = rec.OnHand - totalItemForReleasing - rec.Broken;

                items.Add(rec);
            }

            return items.ToList();
        }



        #endregion



        #region Store Inventory Methods


        private void GetTransferRecordsStore(int? itemId, int? storeId, out int orderTransferQty, out int deliveredTransferQty)
        {

            var orderTransfer = (_context.STOrders
                                        .Include(p => p.OrderedItems)
                                        .Where(p => p.OrderToStoreId == storeId
                                                    && p.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                    && p.DeliveryType != DeliveryTypeEnum.Pickup
                                                    && p.OrderedItems.Where(x => x.ReleaseStatus != ReleaseStatusEnum.Released).Count() > 0)).ToList();

            orderTransferQty = 0;
            deliveredTransferQty = 0;

            foreach (var order in orderTransfer)
            {

                //  Get deliveries
                var deliveries = _context.STDeliveries
                                            .Include(p => p.ClientDeliveries)
                                            .Include(p => p.ShowroomDeliveries)
                                            .Where(p => p.STOrderId == order.Id).ToList();


                var StoreCompanyFrom = this.stores.Where(p => p.Id == order.StoreId).Select(p => p.CompanyId).FirstOrDefault();
                var StoreCompanyTo = this.stores.Where(p => p.Id == order.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault();

                bool isForRelease = false;
                if (StoreCompanyFrom == StoreCompanyTo)
                {
                    isForRelease = (order.ORNumber != null) ? true : false;
                }
                else
                {
                    isForRelease = (order.SINumber != null && order.WHDRNumber != null) ? true : false;
                }

                if (isForRelease)
                {

                    if (deliveries.Count() > 0)
                    {
                        deliveredTransferQty += Convert.ToInt32(deliveries.Where(p => p.STOrderId == order.Id).Sum(p => p.ClientDeliveries.Where(x =>
                                                                        x.ItemId == itemId && 
                                                                        x.ReleaseStatus == ReleaseStatusEnum.Released).Sum(z => z.Quantity)));

                        deliveredTransferQty += Convert.ToInt32(deliveries.Where(p => p.STOrderId == order.Id).Sum(p => p.ShowroomDeliveries.Where(x =>
                                                                        x.ItemId == itemId &&
                                                                        x.ReleaseStatus == ReleaseStatusEnum.Released).Sum(z => z.Quantity)));
                    }

                    //  Get order approved quantity
                    orderTransferQty += Convert.ToInt32(order.OrderedItems
                                                                .Where(p => p.ItemId == itemId
                                                                            && p.ReleaseStatus == ReleaseStatusEnum.Waiting)
                                                                .Sum(p => p.ApprovedQuantity));

                }

                
                
            }
        }

        #region Releasing

        public int GetItemForReleasingStore(int? itemId, int? storeId)
        {

            var allOrders = this.stStock
                        .Include(p => p.STOrderDetail)
                        .Where(p => p.StoreId == storeId
                                    && p.ItemId == itemId
                                    && p.STOrderDetailId.HasValue
                                    && (
                                            p.DeliveryStatus == DeliveryStatusEnum.Waiting
                                            || p.DeliveryStatus == DeliveryStatusEnum.Pending
                                        )
                                    && (
                                            p.ReleaseStatus == ReleaseStatusEnum.Pending
                                            || p.ReleaseStatus == ReleaseStatusEnum.Waiting
                                        )
                                )
                                .ToList();

            var forReleasing = Math.Abs(Convert.ToInt32(allOrders.Sum(p => p.OnHand)));


            int ioNotForRelease = 0;
            for (int i = 0; i < allOrders.Count(); i++)
            {
                var order = this.stOrder.Where(p => p.Id == allOrders[i].STOrderDetail.STOrderId
                                            && p.RequestStatus == RequestStatusEnum.Approved
                                            && p.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder).FirstOrDefault();

                if(order != null && (order.StoreId.HasValue && order.OrderToStoreId.HasValue))
                {
                    var StoreCompanyFrom = this.stores.Where(p => p.Id == order.StoreId).Select(p => p.CompanyId).FirstOrDefault();
                    var StoreCompanyTo = this.stores.Where(p => p.Id == order.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault();

                    bool isNotForRelease = false;
                    if (StoreCompanyFrom == StoreCompanyTo)
                    {
                        isNotForRelease = (string.IsNullOrEmpty(order.ORNumber)) ? true : false;
                    }
                    else
                    {
                        isNotForRelease = (string.IsNullOrEmpty(order.SINumber) && string.IsNullOrEmpty(order.WHDRNumber)) ? true : false;
                    }

                    if (isNotForRelease)
                    {
                        ioNotForRelease += (int)allOrders[i].OnHand;
                    }
                }
            }

            // + Because OnHand is negative
            forReleasing = forReleasing + ioNotForRelease;




            #region Sales Order
            var sales = this.stSales
                                .Include(p => p.SoldItems)
                                .Include(p => p.Deliveries)
                                    .ThenInclude(p => p.ClientDeliveries)
                                .Where(p => p.StoreId == storeId
                                            && p.SalesType == SalesTypeEnum.SalesOrder
                                            && (p.DeliveryType == DeliveryTypeEnum.Delivery || p.DeliveryType == DeliveryTypeEnum.Pickup));

            var soldQty = 0;
            var deliveredQty = 0;


            soldQty = Convert.ToInt32(sales.Sum(p => p.SoldItems.Where(x => x.ItemId == itemId).Sum(y => y.Quantity)));

            deliveredQty = Convert.ToInt32(sales.Sum(p => p.Deliveries.Sum(x => x.ClientDeliveries.Where(y => y.ItemId == itemId
                                                                        && y.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                                        && y.ReleaseStatus == ReleaseStatusEnum.Released)
                                                            .Sum(z => z.Quantity))));

            #endregion

            #region Transfer

            int orderTransferQty, deliveredTransferQty;
            GetTransferRecordsStore(itemId, storeId, out orderTransferQty, out deliveredTransferQty);

            #endregion

            #region RTV

            var objSTReturns = _context.STReturns
                                       .Include(p => p.Deliveries)
                                            .ThenInclude(p => p.WarehouseDeliveries)
                                        .Include(p => p.PurchasedItems)
                                       .Where(p => p.StoreId == storeId
                                                   && ((p.ReturnType == ReturnTypeEnum.RTV
                                                   && p.RequestStatus == RequestStatusEnum.Approved) || (p.ReturnType == ReturnTypeEnum.Breakage
                                                   && p.RequestStatus == RequestStatusEnum.Pending)));

            var totalWarehouseDeliveries = 0;
            var totalReturnQuantity = 0;

            if (objSTReturns != null && objSTReturns.Count() > 0)
            {
                foreach (var obj in objSTReturns)
                {
                    var totalReleasedItem = 0;
                    if (obj.Deliveries != null && obj.Deliveries.Count() > 0 && obj.ReturnType != ReturnTypeEnum.Breakage)
                    {
                        //added for ticket #339
                        if (obj.ReturnType != ReturnTypeEnum.RTV)
                        {
                            totalWarehouseDeliveries = Convert.ToInt32(obj.Deliveries.Sum(x => x.WarehouseDeliveries.Where(y => y.DeliveryStatus == DeliveryStatusEnum.Waiting
                                                       && y.ReleaseStatus == ReleaseStatusEnum.Waiting
                                                       && y.ItemId == itemId)
                                           .Sum(z => z.Quantity)));
                        }
                       
                     
                        

                        //get Total Released item and waiting to be receive item
                        totalReleasedItem = Convert.ToInt32(obj.Deliveries.Sum(x => x.WarehouseDeliveries.Where(y => (y.DeliveryStatus == DeliveryStatusEnum.Delivered || y.DeliveryStatus == DeliveryStatusEnum.Waiting)
                                                       && y.ReleaseStatus == ReleaseStatusEnum.Released
                                                       && y.ItemId == itemId)
                                               .Sum(z => z.Quantity)));



                    }
                    if (obj.PurchasedItems != null && obj.PurchasedItems.Count() > 0)
                    {
                        //get Total Item to return approve by main
                        var totalGoodQty = Convert.ToInt32(obj.PurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting
                        && p.ReleaseStatus == ReleaseStatusEnum.Waiting && p.ItemId == itemId).Sum(x => x.GoodQuantity));

                        var totalBrokenQty = Convert.ToInt32(obj.PurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting
                        && p.ReleaseStatus == ReleaseStatusEnum.Waiting && p.ItemId == itemId).Sum(x => x.BrokenQuantity));

                        if (obj.ReturnType == ReturnTypeEnum.Breakage)
                        {
                            totalBrokenQty += Convert.ToInt32(obj.PurchasedItems.Where(p => ((p.DeliveryStatus == DeliveryStatusEnum.Pending
                        && p.ReleaseStatus == ReleaseStatusEnum.Pending) || p.DeliveryStatus == DeliveryStatusEnum.WaitingForConfirmation) && p.ItemId == itemId).Sum(x => x.BrokenQuantity));
                        }

                        totalReturnQuantity += totalBrokenQty + totalGoodQty;
                        //Will reduct the total released items from total return quantity
                        if (totalBrokenQty != 0 || totalGoodQty != 0)
                        {
                            totalReturnQuantity = totalReturnQuantity - totalReleasedItem;
                        }
                    }
                }

                totalWarehouseDeliveries = totalWarehouseDeliveries + totalReturnQuantity ;
            }


            #endregion




            return forReleasing + (soldQty - deliveredQty) + (orderTransferQty - deliveredTransferQty) + totalWarehouseDeliveries;
        }

        #endregion

        #region Broken

        public int GetItemBrokenQuantityStore(int? itemId, int? storeId)
        {
            var total = Convert.ToInt32(
                                            this.stStock
                                                .Where(p =>
                                                            p.StoreId == storeId
                                                            && p.ItemId == itemId
                                                            && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                            && p.Broken == true
                                                            && p.STImportDetailId == null
                                                        )
                                                .Sum(y => y.OnHand)
                                        );

            return total;
        }

        public int GetItemBrokenRegisterStore(int? itemId, int? storeId)
        {
            var total = Convert.ToInt32(
                                            this.stStock
                                                .Where(p =>
                                                            p.StoreId == storeId
                                                            && p.ItemId == itemId
                                                            && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                            && p.Broken == true
                                                            && p.STImportDetailId !=null 
                                                        )
                                                .Sum(y => y.OnHand)
                                        );

            return total;
        }

        #endregion

        #region Available

        public int GetItemAvailableQuantityStore(int? itemId, int? storeId, bool deductReleasing = false)
        {
            var onHand = Convert.ToInt32(
                                            this.stStock
                                                .Where(p =>
                                                            p.StoreId == storeId
                                                            && p.ItemId == itemId
                                                            && (
                                                                p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                                || p.ReleaseStatus == ReleaseStatusEnum.Released
                                                            )
                                                            && p.Broken == false
                                                        )
                                                .Sum(y => y.OnHand)
                                        );

            var releasing = 0;
            if (deductReleasing)
            {
                releasing = this.GetItemForReleasingStore(itemId, storeId);
            }

            return onHand - releasing;
        }


        #endregion


        #endregion

        private class StoreChanges
        {
            public int? StoreId { get; set; }
            public int? ItemId { get; set; }
        }

        private class WarehouseChanges
        {
            public int? WarehouseId { get; set; }
            public int? ItemId { get; set; }
        }
    }
}

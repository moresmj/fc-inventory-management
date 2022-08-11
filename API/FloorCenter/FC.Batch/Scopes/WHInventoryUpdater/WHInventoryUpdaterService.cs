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

namespace FC.Batch.Scopes.WHInventoryUpdater
{

    public interface IWHInventoryUpdaterService : IScopedExecuteService
    {
    }

    internal class WHInventoryUpdaterService
        : IWHInventoryUpdaterService
    {
        private FC.Api.Helpers.DataContext _context;

        private DateTime? currentTime { get; set; }
        private DateTime? timeFrom { get; set; }


        public IQueryable<Item> item { get; set; }
        public IQueryable<STOrder> stOrder { get; set; }
        public IQueryable<STStock> stStock { get; set; }
        public IQueryable<WHStock> whStock { get; set; }

        public IQueryable<STSales> stSales { get; set; }


        public WHInventoryUpdaterService(
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

            //// Update Warehouse Stocks Summary
            //UpdateWTStockSummary();


            DbCommand cmd = _context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "SP_WarehouseStockSummaryUpdater";
            cmd.CommandType = CommandType.StoredProcedure;

            if (cmd.Connection.State != ConnectionState.Open)
            {
                cmd.Connection.Open();
            }
            cmd.ExecuteNonQuery();
            cmd.Connection.Dispose();
            cmd.Connection.Close();
        }

        private void UpdateWTStockSummary()
        {
            whStock = _context.WHStocks.AsNoTracking();

            int whStockSummaryCount = _context.WHStockSummary.Count();

            if (whStockSummaryCount == 0)
            {
                this.PopulateWarehouseStockSummary();
            }
            else
            {
                // Fetch all items that were changed during this [Current Time and 10 Minutes Before]
                List<WarehouseChanges> stocksHasChanges = _context.WHStocks.Where(p => (timeFrom <= p.DateCreated && currentTime.Value >= p.DateCreated) || (timeFrom <= p.DateUpdated && currentTime.Value >= p.DateUpdated) || (timeFrom <= p.ChangeDate && currentTime.Value >= p.ChangeDate))
                        .Select(p => new WarehouseChanges
                        {
                            WarehouseId = p.WarehouseId,
                            ItemId = p.ItemId
                        }).Distinct()
                        .ToList();
                // will check if changes is made on item table
               List<WarehouseChanges> updatedItem = _context.Items.Where(p => (timeFrom <= p.DateUpdated && currentTime.Value >= p.DateUpdated))
                    .Select(p => new WarehouseChanges
                    {
                        ItemId = p.Id,
                        WarehouseId = null
                    }).Distinct()
                    .ToList();

                if (updatedItem.Count > 0)
                {
                    var warehouseId = _context.Warehouses.Select(p => p.Id).ToList();

                    foreach (var item in updatedItem)
                    {
                        
                        var newList = new List<WarehouseChanges>();
                        for (int i = 0; i < warehouseId.Count(); i++)
                        {
                            var newItem = new WarehouseChanges();
                            newItem.WarehouseId = warehouseId[i];
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
                        List<WHStockSummary> records = (from y in _context.WHStocks.AsNoTracking()
                                                        where stocksHasChanges.Where(p => p.WarehouseId == y.WarehouseId && p.ItemId == y.ItemId).Count() > 0
                                                        group y.Item by y.ItemId into x
                                                        select new WHStockSummary
                                                        {
                                                            WarehouseId = stocksHasChanges[i].WarehouseId,
                                                            ItemId = x.Key,
                                                        }).ToList();

                        // Insert the record by List [Object == Database name]
                        _context.BulkInsertOrUpdate(this.GetInventoriesByWarehouseId(stocksHasChanges[i].WarehouseId, records, true), new BulkConfig { PreserveInsertOrder = true, SetOutputIdentity = true });
                    }
                    transaction.Commit();
                }
            }
        }


        public void PopulateWarehouseStockSummary()
        {
            List<int?> WarehouseIds = _context.WHStocks.AsNoTracking().Where(p => p.WarehouseId != null).Select(p => p.WarehouseId).Distinct().ToList();

            using (var transaction = _context.Database.BeginTransaction())
            {
                for (int i = 0; i < WarehouseIds.Count(); i++)
                {
                    List<WHStockSummary> records = (from y in _context.WHStocks.AsNoTracking()
                                                    where y.WarehouseId == WarehouseIds[i]
                                                    group y.Item by y.ItemId into x
                                                    select new WHStockSummary
                                                    {
                                                        WarehouseId = WarehouseIds[i],
                                                        ItemId = x.Key,
                                                    }).ToList();

                    if(records.Count == 0)
                    {
                        continue;
                    }
                    _context.BulkInsertOrUpdate(this.GetInventoriesByWarehouseId(WarehouseIds[i], records), new BulkConfig { PreserveInsertOrder = true, SetOutputIdentity = true });
                }
                transaction.Commit();
            }
        }

        public List<WHStockSummary> GetInventoriesByWarehouseId(int? warehouseId, List<WHStockSummary> records, bool isPopulated = false)
        {

            var items = new List<WHStockSummary>();

            for (int i = 0; i < records.Count(); i++)
            {
                var rec = records[i];

                // Record with ID means [Update] else [Insert]

                // Item Details
                var itemDetails = item.Where(p => p.Id == rec.ItemId).FirstOrDefault();
                if (isPopulated)
                {
                    rec.Id = _context.WHStockSummary.Where(p => p.WarehouseId == rec.WarehouseId && p.ItemId == rec.ItemId).Select(p => p.Id).FirstOrDefault();
                    rec.DateUpdated = DateTime.Now;
                }
                else
                {
                    rec.DateCreated = DateTime.Now;
                }


                rec.SerialNumber = itemDetails.SerialNumber;
                rec.Code = itemDetails.Code;
                rec.ItemName = itemDetails.Name;
                rec.SizeId = itemDetails.SizeId;
                rec.SizeName = itemDetails.Size.Name;
                rec.Tonality = itemDetails.Tonality;
                rec.Description = itemDetails.Description;


                var approvedOrderedItem = this.GetApprovedOrderedItemWarehouse(rec.ItemId, warehouseId);

                #region For Release

                var totalItemForReleasing = this.GetItemForReleasingWarehouse(rec.ItemId, warehouseId, approvedOrderedItem);

                rec.ForRelease = totalItemForReleasing;

                #endregion

                #region On-hand

                var totalItemAvailable = this.GetItemAvailableQuantityWarehouse(rec.ItemId, warehouseId, false);

                var totalItemReceivedBroken = this.GetItemBrokenQuantityWarehouse(rec.ItemId, warehouseId);

                var totalItemBreakage = this.GetItemBreakageQuantityWarehouse(rec.ItemId, warehouseId);

                totalItemAvailable = totalItemAvailable + totalItemBreakage;

                #endregion

                rec.OnHand = totalItemAvailable + totalItemReceivedBroken;

                rec.Broken = totalItemReceivedBroken + (totalItemBreakage * -1);

                rec.Available = totalItemAvailable - totalItemForReleasing;

                items.Add(rec);
            }

            return items.ToList();

        }

        #region Warehouse Inventory Methods

        private IQueryable<STOrder> GetApprovedOrderedItemWarehouse(int? itemId, int? warehouseId)
        {
            var record = stOrder
                                  .Include(p => p.OrderedItems)
                                  .Where(p => p.WarehouseId == warehouseId
                                              && p.RequestStatus == RequestStatusEnum.Approved).ToList();

            var orders = new List<STOrder>();
            for (int i = 0; i < record.Count(); i++)
            {
                if (record[i].OrderedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting
                                                && p.ReleaseStatus == ReleaseStatusEnum.Waiting
                                                && p.ItemId == itemId)
                                    .Count() > 0)
                {
                    orders.Add(record[i]);
                }
            }

            return orders.AsQueryable();
        }

        #region For Release

        public int GetItemForReleasingWarehouse(int? itemId, int? warehouseId, IQueryable<STOrder> orders = null)
        {
            if (orders == null)
            {
                orders = GetApprovedOrderedItemWarehouse(itemId, warehouseId);
            }

            var total = 0;

            if (orders != null && orders.Count() > 0)
            {
                var order = orders.ToList();
                for (int i = 0; i < order.Count(); i++)
                {
                    //  Check if order type is showroom stock order
                    if (order[i].OrderType == OrderTypeEnum.ShowroomStockOrder)
                    {
                        total = GetItemForReleasingWarehouse_ShowroomStockOrder(itemId, total, order[i]);
                    }
                    else if (order[i].OrderType == OrderTypeEnum.ClientOrder)
                    {
                        if (order[i].DeliveryType == DeliveryTypeEnum.Pickup)
                        {
                            total = GetItemForReleasingWarehouse_ClientOrder_Pickup(itemId, total, order[i]);
                        }
                        else if (order[i].DeliveryType == DeliveryTypeEnum.Delivery)
                        {
                            total = GetItemForReleasingWarehouse_ClientOrder_Delivery(itemId, total, order[i]);
                        }
                        else
                        {
                            total = GetItemForReleasingWarehouse_ClientOrder_ShowroomStockOrder(itemId, total, order[i]);
                        }
                    }
                }
            }

            return total;
        }

        private int GetItemForReleasingWarehouse_ShowroomStockOrder(int? itemId, int total, STOrder order)
        {
            //  Get deliveries
            var deliveries = _context.STDeliveries.AsNoTracking()
                                     .Include(p => p.ShowroomDeliveries)
                                     .Where(p => p.STOrderId == order.Id && p.IsRemainingForReceivingDelivery == false).ToList();

            total = GetTotalForReleasingWarehouse(itemId, total, order);

            var totalReleased = 0;

            //  Check if delivery schedule has been created
            if (deliveries != null && deliveries.Count() > 0)
            {
                for (int i = 0; i < deliveries.Count(); i++)
                {
                    //  Get total showroom delivery released quantity
                    totalReleased += Convert.ToInt32
                            (
                                deliveries[i].ShowroomDeliveries
                                        .Where(p => p.ItemId == itemId
                                                    && (/*p.ReleaseStatus == ReleaseStatusEnum.Pending || */p.ReleaseStatus == ReleaseStatusEnum.Released)
                                               )
                                            .Sum(p => p.Quantity)
                            );
                }
            }
            total = total - totalReleased;

            return total;
        }

        private static int GetTotalForReleasingWarehouse(int? itemId, int total, STOrder order)
        {
            if (order.WHDRNumber != null)
            {
                //  Get order approved quantity
                total += Convert.ToInt32(order.OrderedItems
                                              .Where(p => p.ItemId == itemId
                                                          && p.ReleaseStatus == ReleaseStatusEnum.Waiting)
                                              .Sum(p => p.ApprovedQuantity));
            }

            return total;
        }

        private int GetItemForReleasingWarehouse_ClientOrder_Pickup(int? itemId, int total, STOrder order)
        {

            var deliveries = _context.STDeliveries.AsNoTracking()
                                     .Include(p => p.ClientDeliveries)
                                     .Where(p => p.STOrderId == order.Id).ToList();

            total = GetTotalForReleasingWarehouse(itemId, total, order);

            var totalReleased = 0;


            totalReleased = GetTotalReleasedItemsWarehouse(deliveries, itemId);


            total = total - totalReleased;
            return total;
        }

        private int GetItemForReleasingWarehouse_ClientOrder_Delivery(int? itemId, int total, STOrder order)
        {
            //  Get deliveries
            var deliveries = _context.STDeliveries.AsNoTracking()
                                     .Include(p => p.ClientDeliveries)
                                     .Where(p => p.STOrderId == order.Id).ToList();

            var totalReleased = 0;
            total = GetTotalForReleasingWarehouse(itemId, total, order);

            totalReleased = GetTotalReleasedItemsWarehouse(deliveries, itemId);

            total = total - totalReleased;
            return total;
        }

        private int GetItemForReleasingWarehouse_ClientOrder_ShowroomStockOrder(int? itemId, int total, STOrder order)
        {
            return this.GetItemForReleasingWarehouse_ShowroomStockOrder(itemId, total, order);
        }

        private int GetTotalReleasedItemsWarehouse(List<STDelivery> deliveries, int? itemId)
        {
            var totalReleased = 0;

            //  Check if delivery schedule has been created
            if (deliveries != null && deliveries.Count() > 0)
            {
                for (int i = 0; i < deliveries.Count(); i++)
                {
                    //  Get total client delivery quantity for releasing
                    totalReleased += Convert.ToInt32
                            (
                                deliveries[i].ClientDeliveries
                                        .Where(p => p.ItemId == itemId
                                                    && (p.ReleaseStatus == ReleaseStatusEnum.Released)
                                               )
                                            .Sum(p => p.Quantity)
                            );
                }
            }

            return totalReleased;

        }

        #endregion

        #region Available

        public int GetItemAvailableQuantityWarehouse(int? itemId, int? warehouseId)
        {
            return this.GetItemAvailableQuantityWarehouse(itemId, warehouseId, true);
        }

        public int GetItemAvailableQuantityWarehouse(int? itemId, int? warehouseId, bool deductForRelease)
        {
            var onHand = Convert.ToInt32(
                                            whStock
                                                .Where(p =>
                                                            p.WarehouseId == warehouseId
                                                            && p.ItemId == itemId
                                                            && 
                                                            (p.DeliveryStatus == DeliveryStatusEnum.Delivered || 
                                                            (p.DeliveryStatus == DeliveryStatusEnum.Waiting && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                            )
                                                            && p.Broken == false
                                                        )
                                                .Sum(y => y.OnHand)
                                        );

            if (deductForRelease == true)
            {
                var approvedOrderedItem = this.GetApprovedOrderedItemWarehouse(itemId, warehouseId);

                var totalItemForReleasing = this.GetItemForReleasingWarehouse(itemId, warehouseId, approvedOrderedItem);

                return onHand - totalItemForReleasing;
            }

            return onHand;
        }

        #endregion

        #region Broken

        /// <summary>
        /// Receieved Items as Broken
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public int GetItemBrokenQuantityWarehouse(int? itemId, int? warehouseId)
        {
            var total = Convert.ToInt32(
                                            whStock
                                                .Where(p =>
                                                            p.WarehouseId == warehouseId
                                                            && p.ItemId == itemId
                                                            && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                            && p.Broken == true
                                                            && p.TransactionType != TransactionTypeEnum.PhysicalCount
                                                        )
                                                .Sum(y => y.OnHand)
                                        );

            return total;
        }



        /// <summary>
        /// Breakage Items from Physical count
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public int GetItemBreakageQuantityWarehouse(int? itemId, int? warehouseId)
        {
            var total = Convert.ToInt32(
                                            whStock
                                                .Where(p =>
                                                            p.WarehouseId == warehouseId
                                                            && p.ItemId == itemId
                                                            && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                            && p.Broken == true
                                                            && p.TransactionType == TransactionTypeEnum.PhysicalCount
                                                        )
                                                .Sum(y => y.OnHand)
                                        );

            return total;
        }

        #endregion

        #endregion


        private class WarehouseChanges
        {
            public int? WarehouseId { get; set; }
            public int? ItemId { get; set; }
        }









    }



}

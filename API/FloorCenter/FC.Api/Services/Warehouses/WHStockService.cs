using FC.Api.DTOs.Inventories;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Services.Warehouses
{
    public class WHStockService : IWHStockService
    {

        private DataContext _context;
        public IQueryable<STOrder> stOrder { get; set; }
        public IQueryable<WHStock> whStock { get; set; }
        public IQueryable<WHImportDetail> whImportDetail { get; set; }
        public IQueryable<WHImport> whImport { get; set; }


        public WHStockService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return this._context;
        }

        /// <summary>
        /// Insert whstock
        /// </summary>
        /// <param name="whStock">WHStock</param>
        public void InsertStock(WHStock whStock)
        {
            whStock.DateCreated = DateTime.Now;

            _context.WHStocks.Add(whStock);
            _context.SaveChanges();
        }


        #region Warehouse

        // Has Old Function
        public IEnumerable<object> GetWarehouseItemsByWarehouseId(int? warehouseId)
        {
            var items = _context.WHStockSummary.AsNoTracking().Where(p => p.WarehouseId == warehouseId)
                            .OrderByDescending(p => p.SerialNumber).ToList();

            var inactiveItems = _context.Items.Where(p => p.IsActive == false).Select(p => (int?)p.Id);

            items = items.Where(p => !inactiveItems.Contains(p.ItemId)).ToList();

            return items;
        }

        public IEnumerable<object> GetItemsWithReservation(int? warehouseId)
        {
            var items = _context.WHStockSummary
                                            .AsNoTracking()
                                            .Where(p => p.Reserved > 0 && p.WarehouseId == warehouseId)
                                            .OrderByDescending(p => p.SerialNumber).ToList();

            return items;
        }

        public IEnumerable<object> GetWarehouseItemsByWarehouseIdWithUniqueCode(int? warehouseId)
        {
            var items = _context.WHStockSummary
                                    .Include(p => p.Items)
                                       .AsNoTracking()
                                        .Where(p => p.WarehouseId == warehouseId)
                                            .Select(p => p.Code)
                                                .Distinct()
                                                   .ToList();
            return items;
        }

        // Has Old Function
        public IEnumerable<object> GetInventoriesByWarehouseId(InventorySearchDTO search)
        {
            List<WHStockSummary> records = _context.WHStockSummary
                                                .Include(p => p.Warehouses)
                                                .Include(p => p.Items)
                                                    .AsNoTracking().Where(p => p.WarehouseId == search.WarehouseId).ToList();

            var predicate = (!search.OnlyAvailableStocks && !search.IsOutOfStocks && !search.HasBroken) ? PredicateBuilder.True<WHStockSummary>() : PredicateBuilder.False<WHStockSummary>();

            if (search.OnlyAvailableStocks)
            {
                predicate = predicate.Or(p => p.Available > 0);
            }
            if (search.IsOutOfStocks)
            {
                predicate = predicate.Or(p => p.Available == 0);
            }
            if (search.HasBroken)
            {
                predicate = predicate.Or(p => p.Broken > 0);
            }

            return records.Where(predicate.Compile()).ToList();

        }

        public object GetWarehouseForReleasingDetails(int? ItemId, InventorySearchDTO search, AppSettings appSettings)
        {
            IEnumerable<STOrder> records = _context.STOrders
                                                    .Include(p => p.OrderedItems)
                                                    .AsNoTracking()
                                                    .Where(p => p.OrderedItems.Any(x => x.ItemId == ItemId
                                                                                   && ((x.ReleaseStatus == ReleaseStatusEnum.Waiting
                                                                                   && x.DeliveryStatus == DeliveryStatusEnum.Waiting)
                                                                                   ||
                                                                                   (x.ReleaseStatus == ReleaseStatusEnum.Pending
                                                                                   && x.DeliveryStatus == DeliveryStatusEnum.Pending))
                                                                                   )
                                                                                    && p.WarehouseId == search.WarehouseId 
                                                                                    && p.RequestStatus != RequestStatusEnum.Cancelled).OrderByDescending(p => p.Id);

            GetAllResponse response = null;

            if (search.ShowAll == false)
            {
                response = new GetAllResponse(records.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;
                }

                records = records.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                    .Take(appSettings.RecordDisplayPerPage).ToList();
            }
            else
            {
                response = new GetAllResponse(records.Count());

            }

           

            var forReleaseOrders = records.Select(p => new
            {     
                p.PODate,
                p.PONumber,
                p.WHDRNumber,
                Quantity = p.RequestStatus == RequestStatusEnum.Pending 
                            ? p.OrderedItems.Where(x => x.ItemId == ItemId).Select(c => c.RequestedQuantity).FirstOrDefault() 
                            : p.OrderedItems.Where(x => x.ItemId == ItemId).Select(c => c.ApprovedQuantity).FirstOrDefault()
            }).ToList();

            response.List.AddRange(forReleaseOrders);

            return response;

        }
        public object GetStoreForReleasingDetails(int? ItemId, InventorySearchDTO search, AppSettings appSettings)
        {
            IEnumerable<STOrder> records = _context.STOrders
                                                    .Include(p => p.OrderedItems)
                                                    .AsNoTracking()
                                                    .Where(p => p.OrderedItems.Any(x => x.ItemId == ItemId
                                                                                   && ((x.ReleaseStatus == ReleaseStatusEnum.Waiting
                                                                                   && x.DeliveryStatus == DeliveryStatusEnum.Waiting)
                                                                                   ||
                                                                                   (x.ReleaseStatus == ReleaseStatusEnum.Pending
                                                                                   && x.DeliveryStatus == DeliveryStatusEnum.Pending))
                                                                                   )
                                                                                    && p.WarehouseId == search.WarehouseId
                                                                                    && p.RequestStatus != RequestStatusEnum.Cancelled).OrderByDescending(p => p.Id);

            IEnumerable<STReturn> returnsRec = _context.STReturns
                                                       .Include(p => p.PurchasedItems)
                                                       .AsNoTracking()
                                                       .Where(p => p.PurchasedItems.Any(x => x.ItemId == ItemId
                                                                                       && ((x.ReleaseStatus == ReleaseStatusEnum.Waiting
                                                                                       && x.DeliveryStatus == DeliveryStatusEnum.Waiting)
                                                                                       ||
                                                                                       (x.ReleaseStatus == ReleaseStatusEnum.Pending
                                                                                       && x.DeliveryStatus == DeliveryStatusEnum.Pending))));
            // && p.WarehouseId == search.WarehouseId
            // && p.RequestStatus != RequestStatusEnum.Cancelled).OrderByDescending(p => p.Id);

       
         
            GetAllResponse response = null;
            
          

            if (search.ShowAll == false)
            {
                //STorders or STreturns
                if (returnsRec.Count() > 0)
                {
                    response = new GetAllResponse(returnsRec.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);
                }
                else if (records.Count() > 0)
                {
                    response = new GetAllResponse(records.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);
                }
                


                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;
                }
                    //STorders or STreturns
                    if (returnsRec.Count() > 0)
                    {
                        returnsRec = returnsRec.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                       .Take(appSettings.RecordDisplayPerPage).ToList();
                    }
                    else if (records.Count() > 0)
                    {
                        records = records.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                       .Take(appSettings.RecordDisplayPerPage).ToList();
                    }
               
            }
            else
            {

                    //STorders or STreturns
                    if (returnsRec.Count() > 0)
                    {
                        response = new GetAllResponse(returnsRec.Count());
                    }
                    else if (records.Count() > 0)
                    {
                        response = new GetAllResponse(records.Count());
                    }
                

            }


            //For STOrders
            var forReleaseOrders = records.Select(p => new
            {

                p.StoreId,
                p.OrderToStoreId,
                p.DateCreated,
                p.TransactionNo,
                p.TransactionType,
                p.SINumber,


                Quantity = p.RequestStatus == RequestStatusEnum.Pending
                            ? p.OrderedItems.Where(x => x.ItemId == ItemId).Select(c => c.RequestedQuantity).FirstOrDefault()
                            : p.OrderedItems.Where(x => x.ItemId == ItemId).Select(c => c.ApprovedQuantity).FirstOrDefault()
            })
            .Where(p => p.OrderToStoreId.Equals(search.StoreId))
            .ToList();
            //For STReturns
            var R_forReleaseOrders = returnsRec.Select(p => new
            {

                p.StoreId,
                p.DateCreated,
                p.TransactionNo,
                TransactionType =4,
                //p.SINumber,


                Quantity = p.RequestStatus == RequestStatusEnum.Pending
                                ? p.PurchasedItems.Where(x => x.ItemId == ItemId).Select(c => c.BrokenQuantity).FirstOrDefault()
                                : p.PurchasedItems.Where(x => x.ItemId == ItemId).Select(c => c.GoodQuantity).FirstOrDefault()
            })
         .Where(p => p.StoreId.Equals(search.StoreId))
         .ToList();

            //STorders or STreturns
            if (returnsRec.Count() >0)
            {
                response.List.AddRange(R_forReleaseOrders);
            }
            else if(records.Count() > 0)
            {
                response.List.AddRange(forReleaseOrders);
            }
            

            return response;

        }

        public object GetInventoriesByWarehouseIdPaged(InventorySearchDTO search, AppSettings appSettings)
        {
            IEnumerable<WHStockSummary> records = _context.WHStockSummary
                                                .Include(p => p.Warehouses)
                                                .Include(p => p.Items)
                                                    .AsNoTracking().Where(p => p.WarehouseId == search.WarehouseId).ToList();

            if (!string.IsNullOrEmpty(search.Keyword))
            {
                records = records.Where(p =>
                        p.SerialNumber.ToLower().Contains(search.Keyword.ToLower()) ||
                        p.Code.ToLower().Contains(search.Keyword.ToLower()) ||
                        p.ItemName.ToLower().Contains(search.Keyword.ToLower()) ||
                        p.SizeName.ToLower().Contains(search.Keyword.ToLower()) ||
                        p.Tonality.ToLower().Contains(search.Keyword.ToLower())

                  ).ToList();
            }

            var predicate = (!search.OnlyAvailableStocks && !search.IsOutOfStocks && !search.HasBroken) ? PredicateBuilder.True<WHStockSummary>() : PredicateBuilder.False<WHStockSummary>();

            if (search.OnlyAvailableStocks)
            {
                predicate = predicate.Or(p => p.Available > 0);
            }
            if (search.IsOutOfStocks)
            {
                predicate = predicate.Or(p => p.Available == 0);
            }
            if (search.HasBroken)
            {
                predicate = predicate.Or(p => p.Broken > 0);
            }

            records = records.Where(predicate.Compile()).ToList();
            GetAllResponse response = null;

            if (search.ShowAll == false)
            {
                response = new GetAllResponse(records.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;
                }

                records = records.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                    .Take(appSettings.RecordDisplayPerPage).ToList();
            }
            else
            {
                response = new GetAllResponse(records.Count());

            }

            response.List.AddRange(records);

            return response;

        }

        // Has Old Function
        public IEnumerable<object> GetInventoriesSummaryByWarehouseId(int? warehouseId)
        {
            List<WHStockSummary> records = _context.WHStockSummary.AsNoTracking()
                                .Include(p => p.Items)
                                    .Where(p => p.WarehouseId == warehouseId).ToList();

            var items = new List<InventoryInfo>();


            stOrder = _context.STOrders.AsNoTracking()
                .Include(p => p.OrderedItems);

            //GetLatestItemAdjustment
            whImportDetail = _context.WHImportDetails.AsNoTracking().OrderByDescending(t => t.DateCreated);
            whImport = _context.WHImports.AsNoTracking();

            for (int i = 0; i < records.Count(); i++)
            {
                var adj = this.GetLatestItemAdjustment(records[i].ItemId, warehouseId);

                var invInfo = new InventoryInfo();

                invInfo.ItemId = records[i].ItemId;
                invInfo.SerialNumber = records[i].SerialNumber;
                invInfo.Code = records[i].Code;
                invInfo.ItemName = records[i].ItemName;
                invInfo.SizeName = records[i].SizeName;
                invInfo.Tonality = records[i].Tonality;
                invInfo.Description = records[i].Description;
                invInfo.SRP = records[i].Items.SRP;
                invInfo.Remarks = records[i].Items.Remarks;
                invInfo.ImageName = records[i].Items.ImageName;
                invInfo.OnHand = records[i].OnHand;
                invInfo.ForRelease = records[i].ForRelease;
                invInfo.Available = records[i].Available;
                invInfo.Broken = records[i].Broken;

                if (adj != null)
                {
                    if (adj.Details != null)
                    {
                        invInfo.adjustment = this.GetDiscrepancy(adj.Details.Select(p => p.PhysicalCount).FirstOrDefault(), adj.Details.Select(p => p.SystemCount).FirstOrDefault());
                        invInfo.dateCreated = adj.Details.Select(p => p.DateCreated).FirstOrDefault();
                        invInfo.physicalCount = adj.Details.Select(p => p.PhysicalCount).FirstOrDefault();
                    }
                }

                items.Add(invInfo);

            }

            return items;

        }

        #endregion

        #region Main Inventory List

        public List<object> GetWarehouseInventories(InventorySearchDTO dto)
        {

            List<int?> WarehouseIds = new List<int?>();

            if (dto.WarehouseId.HasValue)
            {
                WarehouseIds.Add((int)dto.WarehouseId);
            }

            if (!string.IsNullOrEmpty(dto.Keyword))
            {
                dto.Keyword = dto.Keyword.ToLower();
                int value;
                if (int.TryParse(dto.Keyword, out value))
                {
                    dto.Keyword2 = value;
                }

                if (!dto.WarehouseId.HasValue)
                {
                    // Store id will be null if WHDeliveryDetailId has value
                    WarehouseIds = _context.WHStocks.AsNoTracking().Where(p => p.WarehouseId != null).Select(p => p.WarehouseId).Distinct().ToList();
                }
            }

            return this.GetListOfWarehouseInventories(WarehouseIds, dto);

        }

        public List<object> GetListOfWarehouseInventories(List<int?> warehouseIds, InventorySearchDTO dto)
        {
            List<object> records = new List<object>();

            var warehousesDetails = _context.Warehouses.AsNoTracking();

            for (int i = 0; i < warehouseIds.Count(); i++)
            {
                dto.WarehouseId = warehouseIds[i];
                List<WHStockSummary> inventory = (List<WHStockSummary>)this.GetInventoriesByWarehouseId(dto);

                var items = new List<InventoryInfo>();
                var wDetails = warehousesDetails.Where(p => p.Id == warehouseIds[i]).Select(p => p).FirstOrDefault();

                if (inventory != null)
                {
                    foreach (var rec in inventory)
                    {
                        items.Add(new InventoryInfo
                        {
                            ItemId = rec.ItemId,
                            SerialNumber = rec.SerialNumber,
                            Code = rec.Code,
                            ItemName = rec.ItemName,
                            SizeName = rec.SizeName,
                            Tonality = rec.Tonality,
                            Description = rec.Description,
                            SRP = rec.Items.SRP,
                            Remarks = rec.Items.Remarks,
                            ImageName = rec.Items.ImageName,
                            OnHand = rec.OnHand,
                            ForRelease = rec.ForRelease,
                            Available = rec.Available,
                            Broken = rec.Broken,
                            WarehouseName = wDetails.Name,
                            WarehouseAddress = (wDetails.Address == null) ? string.Empty : wDetails.Address
                        });
                    }

                    if (!string.IsNullOrEmpty(dto.Keyword))
                    {
                        if (dto.Keyword2.HasValue)
                        {
                            items = items.Where(p =>
                                        p.SerialNumber == dto.Keyword2.ToString() ||
                                        p.Code.ToLower().Contains(dto.Keyword) ||
                                        p.ItemName.ToLower().Contains(dto.Keyword) ||
                                        p.SizeName.ToLower().Contains(dto.Keyword) ||
                                        p.Tonality.ToLower().Contains(dto.Keyword) ||
                                        p.WarehouseName.ToLower().Contains(dto.Keyword)
                                    ).ToList();
                        }
                        else
                        {
                            items = items.Where(p =>
                                        p.Code.ToLower().Contains(dto.Keyword) ||
                                        p.ItemName.ToLower().Contains(dto.Keyword) ||
                                        p.SizeName.ToLower().Contains(dto.Keyword) ||
                                        p.Tonality.ToLower().Contains(dto.Keyword) ||
                                        p.WarehouseName.ToLower().Contains(dto.Keyword)
                                    ).ToList();
                        }
                    }

                    if (items.Count() > 0)
                    {
                        var record = new
                        {
                            WarehouseId = (int)warehouseIds[i],
                            WarehouseName = wDetails.Name,
                            Items = items,
                            ItemCount = items.Count(),
                            TotalItems = items.Sum(p => p.OnHand)
                        };

                        records.Add(record);
                    }

                }
            }
            return records;

        }

        #endregion


        public IQueryable<STOrder> GetApprovedOrderedItem(int? itemId, int? warehouseId)
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

        public int GetItemForReleasing(int? itemId, int? warehouseId, IQueryable<STOrder> orders = null)
        {
            if (orders == null)
            {
                orders = GetApprovedOrderedItem(itemId, warehouseId);
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
                        total = GetItemForReleasing_ShowroomStockOrder(itemId, total, order[i]);
                    }
                    else if (order[i].OrderType == OrderTypeEnum.ClientOrder)
                    {
                        if (order[i].DeliveryType == DeliveryTypeEnum.Pickup)
                        {
                            total = GetItemForReleasing_ClientOrder_Pickup(itemId, total, order[i]);
                        }
                        else if (order[i].DeliveryType == DeliveryTypeEnum.Delivery)
                        {
                            total = GetItemForReleasing_ClientOrder_Delivery(itemId, total, order[i]);
                        }
                        else
                        {
                            total = GetItemForReleasing_ClientOrder_ShowroomStockOrder(itemId, total, order[i]);
                        }
                    }
                }
            }

            return total;
        }

        private int GetItemForReleasing_ShowroomStockOrder(int? itemId, int total, STOrder order)
        {
            //  Get deliveries
            var deliveries = _context.STDeliveries.AsNoTracking()
                                     .Include(p => p.ShowroomDeliveries)
                                     .Where(p => p.STOrderId == order.Id).ToList();

            total = GetTotalForReleasing(itemId, total, order);

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

        private static int GetTotalForReleasing(int? itemId, int total, STOrder order)
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

        private int GetItemForReleasing_ClientOrder_Pickup(int? itemId, int total, STOrder order)
        {

            var deliveries = _context.STDeliveries.AsNoTracking()
                                     .Include(p => p.ClientDeliveries)
                                     .Where(p => p.STOrderId == order.Id).ToList();

            total = GetTotalForReleasing(itemId, total, order);

            var totalReleased = 0;

            ////  Check if delivery schedule has been created
            //if (deliveries != null && deliveries.Count() > 0)
            //{
            //    for (int i = 0; i < deliveries.Count(); i++)
            //    {
            //        //  Get total showroom delivery released quantity
            //        totalReleased += Convert.ToInt32
            //                (
            //                    deliveries[i].ClientDeliveries
            //                            .Where(p => p.ItemId == itemId
            //                                        && (p.ReleaseStatus == ReleaseStatusEnum.Released)
            //                                   )
            //                                .Sum(p => p.Quantity)
            //                );
            //    }
            //}
            //get Total Released items
            totalReleased = GetTotalReleasedItems(deliveries, itemId);


            total = total - totalReleased;
            return total;
        }

        private int GetItemForReleasing_ClientOrder_Delivery(int? itemId, int total, STOrder order)
        {
            //  Get deliveries
            var deliveries = _context.STDeliveries.AsNoTracking()
                                     .Include(p => p.ClientDeliveries)
                                     .Where(p => p.STOrderId == order.Id).ToList();

            var totalReleased = 0;
            total = GetTotalForReleasing(itemId, total, order);

            totalReleased = GetTotalReleasedItems(deliveries,itemId);

            total = total - totalReleased;
            return total;
        }

        private int GetItemForReleasing_ClientOrder_ShowroomStockOrder(int? itemId, int total, STOrder order)
        {
            return this.GetItemForReleasing_ShowroomStockOrder(itemId, total, order);
        }

        private int GetTotalReleasedItems(List<STDelivery> deliveries, int? itemId)
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

        /// <summary>
        /// Get item available quantity
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public int GetItemAvailableQuantity(int? itemId, int? warehouseId)
        {

            return this.GetItemAvailableQuantity(itemId, warehouseId, true);
        }
        
        public int GetItemAvailableQuantity(int? itemId, int? warehouseId, bool deductForRelease)
        {
            var onHand = _context.CustomQuantity.FromSql("SELECT dbo.FSWHGetOnhand({1}, {0}, {2}) as Total", itemId, warehouseId, deductForRelease).Sum(p => p.Total);
            //var onHand = Convert.ToInt32(
            //                                whStock
            //                                    .Where(p =>
            //                                                p.WarehouseId == warehouseId
            //                                                && p.ItemId == itemId
            //                                                && (
            //                                                    p.DeliveryStatus == DeliveryStatusEnum.Delivered
            //                                                    ||
            //                                                    (p.DeliveryStatus == DeliveryStatusEnum.Waiting
            //                                                     && p.ReleaseStatus == ReleaseStatusEnum.Released)
            //                                                )
            //                                                && p.Broken == false
            //                                            )
            //                                    .Sum(y => y.OnHand)
            //                            );

            //if(deductForRelease == true)
            //{
            //    var approvedOrderedItem = this.GetApprovedOrderedItem(itemId, warehouseId);
                
            //    var totalItemForReleasing = this.GetItemForReleasing(itemId, warehouseId, approvedOrderedItem);

            //    // fix to match warehouse inventory
            //    var totalBreakage = this.GetItemBreakageQuantity(itemId, warehouseId);
            //    onHand = onHand + totalBreakage;

            //    return onHand - totalItemForReleasing;
            //}

            return onHand;
        }

        #endregion

        #region Broken

        public int GetItemBrokenQuantity(int? itemId, int? warehouseId)
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

        public int GetItemBreakageQuantity(int? itemId, int? warehouseId)
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

        #region Discrepancy

        public int? GetDiscrepancy(int? pcount, int? systemcount)
        {
            return Convert.ToInt32(systemcount - pcount) < 0
                                                        ? Math.Abs(Convert.ToInt32(systemcount - pcount))
                                                        : -Convert.ToInt32(systemcount - pcount);
        }

        #endregion

        public WHImport GetLatestItemAdjustment(int? itemId, int? whId)
        {

            var import = new WHImport();

            var lastAdjustment = whImportDetail.Where(p => p.ItemId == itemId && p.AllowUpdate == true).FirstOrDefault();

            if (lastAdjustment != null)
            {
                import = whImport.Where(p => p.Id == lastAdjustment.WHImportId && p.WarehouseId == whId).Include(p => p.Details).FirstOrDefault();
                if (import != null)
                {
                    import.Details = import.Details.Where(p => p.ItemId == itemId).ToList();
                }

            }

            return import;
        }

        #region Old Functions

        // Old Function of fetching items by warehouse [Orders > Showroom Stock Order and etc.]
        public IEnumerable<object> GetWarehouseItemsByWarehouseIdOld(int? warehouseId)
        {
            List<InventoryInfo> records = (from y in _context.WHStocks.AsNoTracking()
                                           where y.WarehouseId == warehouseId
                                           group y.Item by y.ItemId into x
                                           select new InventoryInfo
                                           {
                                               ItemId = x.Key,
                                               SerialNumber = x.Select(c => c.SerialNumber).FirstOrDefault(),
                                               Code = x.Select(c => c.Code).FirstOrDefault(),
                                               ItemName = x.Select(c => c.Name).FirstOrDefault(),
                                               SizeName = x.Select(c => c.Size.Name).FirstOrDefault(),
                                               Tonality = x.Select(c => c.Tonality).FirstOrDefault()
                                           }).ToList();

            var items = new List<InventoryInfo>();

            stOrder = _context.STOrders.AsNoTracking()
                .Include(p => p.OrderedItems);

            whStock = _context.WHStocks.AsNoTracking();

            for (int i = 0; i < records.Count(); i++)
            {
                var totalItemAvailable = this.GetItemAvailableQuantity(records[i].ItemId, warehouseId, false);
                var totalItemForReleasing = this.GetItemForReleasing(records[i].ItemId, warehouseId);

                records[i].Available = totalItemAvailable - totalItemForReleasing;

                items.Add(records[i]);
            }


            return items;
        }

        // Old Warehouse Inventory
        public IEnumerable<object> GetInventoriesByWarehouseIdOld(InventorySearchDTO search)
        {
            IQueryable<InventoryInfo> records = from y in _context.WHStocks.AsNoTracking()
                                                where y.WarehouseId == search.WarehouseId
                                                group y.Item by y.ItemId into x
                                                select new InventoryInfo
                                                {
                                                    ItemId = x.Key,
                                                    SerialNumber = x.Select(c => c.SerialNumber).FirstOrDefault(),
                                                    Code = x.Select(c => c.Code).FirstOrDefault(),
                                                    ItemName = x.Select(c => c.Name).FirstOrDefault(),
                                                    SizeName = x.Select(c => c.Size.Name).FirstOrDefault(),
                                                    Tonality = x.Select(c => c.Tonality).FirstOrDefault(),
                                                    SRP = x.Select(c => c.SRP).FirstOrDefault(),
                                                    Description = x.Select(c => c.Description).FirstOrDefault(),
                                                    Remarks = x.Select(c => c.Remarks).FirstOrDefault()
                                                };

            var items = new List<InventoryInfo>();
            stOrder = _context.STOrders.AsNoTracking()
                            .Include(p => p.OrderedItems);

            whStock = _context.WHStocks.AsNoTracking();

            //GetItemAvailableQuantity
            whImportDetail = _context.WHImportDetails.AsNoTracking().OrderByDescending(t => t.DateCreated);
            whImport = _context.WHImports.AsNoTracking();

            foreach (var rec in records)
            {
                var approvedOrderedItem = this.GetApprovedOrderedItem(rec.ItemId, search.WarehouseId);

                #region For Release

                var totalItemForReleasing = this.GetItemForReleasing(rec.ItemId, search.WarehouseId, approvedOrderedItem);

                rec.ForRelease = totalItemForReleasing;

                #endregion

                #region On-hand

                var totalItemAvailable = this.GetItemAvailableQuantity(rec.ItemId, search.WarehouseId, false);

                var totalItemBroken = this.GetItemBrokenQuantity(rec.ItemId, search.WarehouseId);

                rec.OnHand = totalItemAvailable + totalItemBroken;

                #endregion

                rec.Broken = totalItemBroken;

                rec.Available = totalItemAvailable - totalItemForReleasing;

                items.Add(rec);
            }


            var predicate = (!search.OnlyAvailableStocks && !search.IsOutOfStocks && !search.HasBroken) ? PredicateBuilder.True<InventoryInfo>() : PredicateBuilder.False<InventoryInfo>();

            if (search.OnlyAvailableStocks)
            {
                predicate = predicate.Or(p => p.Available > 0);
            }
            if (search.IsOutOfStocks)
            {
                predicate = predicate.Or(p => p.Available == 0);
            }
            if (search.HasBroken)
            {
                predicate = predicate.Or(p => p.Broken > 0);
            }

            return items.Where(predicate.Compile()).ToList();

        }


        public IEnumerable<object> GetInventoriesSummaryByWarehouseIdOld(int? warehouseId)
        {
            List<InventoryInfo> records = (from y in _context.WHStocks.AsNoTracking()
                                           where y.WarehouseId == warehouseId
                                           group y.Item by y.ItemId into x
                                           select new InventoryInfo
                                           {
                                               ItemId = x.Key,
                                               SerialNumber = x.Select(c => c.SerialNumber).FirstOrDefault(),
                                               Code = x.Select(c => c.Code).FirstOrDefault(),
                                               ItemName = x.Select(c => c.Name).FirstOrDefault(),
                                               SizeName = x.Select(c => c.Size.Name).FirstOrDefault(),
                                               Tonality = x.Select(c => c.Tonality).FirstOrDefault(),
                                               SRP = x.Select(c => c.SRP).FirstOrDefault(),
                                               Description = x.Select(c => c.Description).FirstOrDefault(),
                                               Remarks = x.Select(c => c.Remarks).FirstOrDefault()
                                           }).ToList();

            var items = new List<InventoryInfo>();


            stOrder = _context.STOrders.AsNoTracking()
                .Include(p => p.OrderedItems);

            //GetLatestItemAdjustment
            whImportDetail = _context.WHImportDetails.AsNoTracking().OrderByDescending(t => t.DateCreated);
            whImport = _context.WHImports.AsNoTracking();

            for (int i = 0; i < records.Count(); i++)
            {
                var approvedOrderedItem = this.GetApprovedOrderedItem(records[i].ItemId, warehouseId);

                #region For Release

                // Get the last item adjustment by item ID
                var adj = this.GetLatestItemAdjustment(records[i].ItemId, warehouseId);

                if (adj != null)
                {
                    if (adj.Details != null)
                    {
                        records[i].adjustment = this.GetDiscrepancy(adj.Details.Select(p => p.PhysicalCount).FirstOrDefault(), adj.Details.Select(p => p.SystemCount).FirstOrDefault());
                        records[i].dateCreated = adj.Details.Select(p => p.DateCreated).FirstOrDefault();
                        records[i].physicalCount = adj.Details.Select(p => p.PhysicalCount).FirstOrDefault();
                    }

                }

                #endregion

                items.Add(records[i]);
            }

            return items;

        }


        public object GetInventoriesSummaryByWarehouseIdPaged(int? warehouseId, InventorySearchDTO search, AppSettings appSettings)
        {
            List<InventoryInfo> records = (from y in _context.WHStocks.AsNoTracking()
                                           where y.WarehouseId == warehouseId
                                           group y.Item by y.ItemId into x
                                           select new InventoryInfo
                                           {
                                               ItemId = x.Key,
                                               SerialNumber = x.Select(c => c.SerialNumber).FirstOrDefault(),
                                               Code = x.Select(c => c.Code).FirstOrDefault(),
                                               ItemName = x.Select(c => c.Name).FirstOrDefault(),
                                               SizeName = x.Select(c => c.Size.Name).FirstOrDefault(),
                                               Tonality = x.Select(c => c.Tonality).FirstOrDefault(),
                                               SRP = x.Select(c => c.SRP).FirstOrDefault(),
                                               Description = x.Select(c => c.Description).FirstOrDefault(),
                                               Remarks = x.Select(c => c.Remarks).FirstOrDefault()
                                           }).ToList();

            var items = new List<InventoryInfo>();

            GetAllResponse response = null;

            if(!string.IsNullOrEmpty(search.Keyword))
            {
                records = records.Where(p =>
                        p.SerialNumber.ToLower().Contains(search.Keyword.ToLower()) ||
                        p.Code.ToLower().Contains(search.Keyword.ToLower()) ||
                        p.ItemName.ToLower().Contains(search.Keyword.ToLower()) ||
                        p.SizeName.ToLower().Contains(search.Keyword.ToLower()) ||
                        p.Tonality.ToLower().Contains(search.Keyword.ToLower())

                  ).ToList();
            }



            if(search.ShowAll == false)
            {
                response = new GetAllResponse(records.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);

                if(search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                }

                records = records.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                    .Take(appSettings.RecordDisplayPerPage).ToList();
            }
            else
            {
                response = new GetAllResponse(records.Count());
            }


            stOrder = _context.STOrders.AsNoTracking()
                .Include(p => p.OrderedItems);

            //GetLatestItemAdjustment
            whImportDetail = _context.WHImportDetails.AsNoTracking().OrderByDescending(t => t.DateCreated);
            whImport = _context.WHImports.AsNoTracking();

            for (int i = 0; i < records.Count(); i++)
            {
                var approvedOrderedItem = this.GetApprovedOrderedItem(records[i].ItemId, warehouseId);

                #region For Release

                // Get the last item adjustment by item ID
                var adj = this.GetLatestItemAdjustment(records[i].ItemId, warehouseId);

                if (adj != null)
                {
                    if (adj.Details != null)
                    {
                        records[i].adjustment = this.GetDiscrepancy(adj.Details.Select(p => p.PhysicalCount).FirstOrDefault(), adj.Details.Select(p => p.SystemCount).FirstOrDefault());
                        records[i].dateCreated = adj.Details.Select(p => p.DateCreated).FirstOrDefault();
                        records[i].physicalCount = adj.Details.Select(p => p.PhysicalCount).FirstOrDefault();
                    }

                }

                #endregion

                items.Add(records[i]);
            }

            response.List.AddRange(items);

            return response;

        }

        #endregion

    }

}

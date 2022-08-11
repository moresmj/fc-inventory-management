using FC.Api.DTOs.Inventories;
using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Common;
using FC.Core.Domain.Items;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Users;
using FC.Core.Domain.Warehouses;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace FC.Api.Services.Stores
{
    public class STStockService : ISTStockService
    {

        private DataContext _context;

        public List<Store> stores { get; set; }
        public IQueryable<STOrder> stOrder { get; set; }
        public IQueryable<STStock> stStock { get; set; }
        public IQueryable<STSales> stSales { get; set; }
        public IQueryable<STReturn> stReturns { get; set; }
        public IQueryable<STImportDetail> stImportDetail { get; set; }
        public IQueryable<STImport> stImport { get; set; }

        public STStockService(DataContext context)
        {
            _context = context;
        }



        public void InsertSTStock(STStock stStock)
        {
            stStock.DateCreated = DateTime.Now;

            _context.STStocks.Add(stStock);
            _context.SaveChanges();
        }

        #region Store

        // Has Old Function
        public IEnumerable<object> GetInventoriesByStoreId(InventorySearchDTO search)
        {
            IQueryable<Item> item = _context.Items.AsNoTracking();

            IQueryable<STStockSummary> records = _context.STStockSummary.AsNoTracking()
                                                .Include(p => p.Stores)
                                                .Include(p => p.Items)
                                                    .Where(p => p.StoreId == search.StoreId);


            //var predicate = (!search.OnlyAvailableStocks && !search.IsOutOfStocks && !search.HasBroken) ? PredicateBuilder.True<STStockSummary>() : PredicateBuilder.False<STStockSummary>();

            //if (search.OnlyAvailableStocks)
            //{
            //    predicate = predicate.Or(p => p.Available > 0);
            //}
            //if (search.IsOutOfStocks)
            //{
            //    predicate = predicate.Or(p => p.Available == 0);
            //}
            //if (search.HasBroken)
            //{
            //    predicate = predicate.Or(p => p.Broken > 0);
            //}

            return records;
        }


        public object GetInventoriesByStoreIdPaged(InventorySearchDTO search, AppSettings appSettings)
        {
            IQueryable<Item> item = _context.Items.AsNoTracking();

            IQueryable<STStockSummary> records = _context.STStockSummary.AsNoTracking()
                                                .Include(p => p.Stores)
                                                .Include(p => p.Items)
                                                    .Where(p => p.StoreId == search.StoreId);


            if (!string.IsNullOrEmpty(search.Keyword))
            {
                records = records.Where(p =>
                        p.SerialNumber.ToLower().Contains(search.Keyword.ToLower()) ||
                        p.Code.ToLower().Contains(search.Keyword.ToLower()) ||
                        p.ItemName.ToLower().Contains(search.Keyword.ToLower()) ||
                        p.SizeName.ToLower().Contains(search.Keyword.ToLower()) ||
                        p.Tonality.ToLower().Contains(search.Keyword.ToLower())

                  );
            }


            var predicate = (!search.OnlyAvailableStocks && !search.IsOutOfStocks && !search.HasBroken) ? PredicateBuilder.True<STStockSummary>() : PredicateBuilder.False<STStockSummary>();

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
             List<STStockSummary> stStocks = new List<STStockSummary>();
            stStocks = records.Where(predicate.Compile()).ToList();
            GetAllResponse response = null;

            if (search.ShowAll == false)
            {
                response = new GetAllResponse(stStocks.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;
                }

                stStocks = stStocks.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                    .Take(appSettings.RecordDisplayPerPage).ToList();
            }
            else
            {
                response = new GetAllResponse(stStocks.Count());

            }

            response.List.AddRange(stStocks);

            return response;
        }



        public Object GetInvetoriesSummaryByStoreIdPaged(int? storeId, InventorySearchDTO search, AppSettings appSettings)
        {
            
            List<STStockSummary> records = _context.STStockSummary.AsNoTracking()
                                            .Include(p => p.Items)
                                                .Where(p => p.StoreId == storeId).ToList();

            var items = new List<InventoryInfo>();

            this.stStock = _context.STStocks;
            this.stImportDetail = _context.STImportDetails.AsNoTracking();
            this.stImport = _context.STImports.AsNoTracking();
            this.stSales = _context.STSales.AsNoTracking()
                                .Include(p => p.SoldItems)
                                .Include(p => p.Deliveries)
                                    .ThenInclude(p => p.ClientDeliveries)
                                .Where(p => p.StoreId == storeId
                                            && p.SalesType == SalesTypeEnum.SalesOrder
                                            && p.DeliveryType == DeliveryTypeEnum.Delivery);

            GetAllResponse response = null;

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

           

            for (int i = 0; i < records.Count(); i++)
            {
                var adj = this.GetLatestItemAdjustment(records[i].ItemId, storeId);

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

            response.List.AddRange(items);





            return response;
        }

        // Has Old Function
        public IEnumerable<object> GetInventoriesSummaryByStoreId(int? storeId)
        {

            List<STStockSummary> records = _context.STStockSummary.AsNoTracking()
                                            .Include(p => p.Items)
                                                .Where(p => p.StoreId == storeId).ToList();

            var items = new List<InventoryInfo>();

            this.stStock = _context.STStocks;
            this.stOrder  = _context.STOrders.AsNoTracking().Include(p => p.OrderedItems);
            this.stImportDetail = _context.STImportDetails.AsNoTracking();
            this.stImport = _context.STImports.AsNoTracking();
            this.stSales = _context.STSales.AsNoTracking()
                                .Include(p => p.SoldItems)
                                .Include(p => p.Deliveries)
                                    .ThenInclude(p => p.ClientDeliveries)
                                .Where(p => p.StoreId == storeId
                                            && p.SalesType == SalesTypeEnum.SalesOrder
                                            && p.DeliveryType == DeliveryTypeEnum.Delivery); ;

            for (int i = 0; i < records.Count(); i++)
            {
                var adj = this.GetLatestItemAdjustment(records[i].ItemId, storeId);

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

        #region Main Store Inventory List

        public List<object> GetStoreInventories(InventorySearchDTO dto)
        {

            List<int?> storeIds = new List<int?>();

            if (dto.StoreId.HasValue)
            {
                storeIds.Add((int)dto.StoreId);
            }

            if (!string.IsNullOrEmpty(dto.Keyword))
            {
                dto.Keyword = dto.Keyword.ToLower();
                int value;
                if (int.TryParse(dto.Keyword, out value))
                {
                    dto.Keyword2 = value;
                }

                if (!dto.StoreId.HasValue)
                {
                    // Store id will be null if WHDeliveryDetailId has value
                    storeIds = _context.STStocks.Where(p => p.StoreId != null).Select(p => p.StoreId).Distinct().ToList();
                }
            }

            return this.GetListOfStoreInventories(storeIds, dto);

        }

        public List<object> GetListOfStoreInventories(List<int?> storeIds, InventorySearchDTO dto)
        {
            List<object> records = new List<object>();

            var storesDetails = _context.Stores.AsNoTracking().Where(p => storeIds.Contains(p.Id)).AsQueryable();
            var companies = _context.Companies.AsNoTracking().AsQueryable();

            for (int i = 0; i < storeIds.Count(); i++)
            {
                dto.StoreId = storeIds[i];

                IQueryable<STStockSummary> inventory = (IQueryable<STStockSummary>)this.GetInventoriesByStoreId(dto);

                var items = new List<InventoryInfo>();
                var storeDetails = storesDetails.Where(p => p.Id == storeIds[i]).Select(p => p).FirstOrDefault();
                var companyDetails = companies.Where(p => p.Id == storeDetails.CompanyId).Select(p => p).FirstOrDefault();

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
                            StoreName = storeDetails.Name,
                            StoreAddress = (storeDetails.Address == null) ? string.Empty : storeDetails.Address,
                            CompanyName = companyDetails.Name
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
                                        p.StoreName.ToLower().Contains(dto.Keyword) ||
                                        p.StoreAddress.ToLower().Contains(dto.Keyword) ||
                                        p.CompanyName.ToLower().Contains(dto.Keyword)
                                    ).ToList();
                        }
                        else
                        {
                            items = items.Where(p =>
                                        p.Code.ToLower().Contains(dto.Keyword) ||
                                        p.ItemName.ToLower().Contains(dto.Keyword) ||
                                        p.SizeName.ToLower().Contains(dto.Keyword) ||
                                        p.Tonality.ToLower().Contains(dto.Keyword) ||
                                        p.StoreName.ToLower().Contains(dto.Keyword) ||
                                        p.StoreAddress.ToLower().Contains(dto.Keyword) ||
                                        p.CompanyName.ToLower().Contains(dto.Keyword)
                                    ).ToList();
                        }
                    }

                    if (items.Count() > 0)
                    {
                        var record = new
                        {
                            StoreId = (int)storeIds[i],
                            StoreName = storeDetails.Name,
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


        public object GetListOfStoreInventoriesPaged(int? storeIds, InventorySearchDTO dto, AppSettings appSettings)
        {
            List<object> records = new List<object>();

            var storeDetails = _context.Stores.AsNoTracking().Where(p => p.Id == storeIds).FirstOrDefault();
            var companies = _context.Companies.AsNoTracking().AsQueryable();

            


            IQueryable<STStockSummary> inventory = (IQueryable<STStockSummary>)this.GetInventoriesByStoreId(dto);

            var items = new List<InventoryInfo>();
            var companyDetails = companies.Where(p => p.Id == storeDetails.CompanyId).Select(p => p).FirstOrDefault();

            if (inventory != null)
            {
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
                                    p.StoreName.ToLower().Contains(dto.Keyword) ||
                                    p.StoreAddress.ToLower().Contains(dto.Keyword) ||
                                    p.CompanyName.ToLower().Contains(dto.Keyword)
                                ).ToList();
                    }
                    else
                    {
                        items = items.Where(p =>
                                    p.Code.ToLower().Contains(dto.Keyword) ||
                                    p.ItemName.ToLower().Contains(dto.Keyword) ||
                                    p.SizeName.ToLower().Contains(dto.Keyword) ||
                                    p.Tonality.ToLower().Contains(dto.Keyword) ||
                                    p.StoreName.ToLower().Contains(dto.Keyword) ||
                                    p.StoreAddress.ToLower().Contains(dto.Keyword) ||
                                    p.CompanyName.ToLower().Contains(dto.Keyword)
                                ).ToList();
                    }
                }

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
                        StoreName = storeDetails.Name,
                        StoreAddress = (storeDetails.Address == null) ? string.Empty : storeDetails.Address,
                        CompanyName = companyDetails.Name
                    });
                }

               

                if (items.Count() > 0)
                {
                    var record = new
                    {
                        StoreId = storeDetails.Id,
                        StoreName = storeDetails.Name,
                        Items = items,
                        ItemCount = items.Count(),
                        TotalItems = items.Sum(p => p.OnHand)
                    };

                    records.Add(record);
                }

            }

            return records;

        }

        #endregion

        #region Releasing

        public int GetItemForReleasing(int? itemId, int? storeId)
        {
            this.stOrder = _context.STOrders.AsNoTracking().Include(p => p.OrderedItems);
            this.stores =  _context.Stores.AsNoTracking().ToList();

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

                if (order != null && (order.StoreId.HasValue && order.OrderToStoreId.HasValue))
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
            GetTransferRecords(itemId, storeId, out orderTransferQty, out deliveredTransferQty);

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

                totalWarehouseDeliveries = totalWarehouseDeliveries + totalReturnQuantity;
            }


            #endregion
            #region OLD Code

            //var forReleasing = Math.Abs(Convert.ToInt32(
            //                                            this.stStock
            //                                                .Where(p => p.StoreId == storeId
            //                                                            && p.ItemId == itemId
            //                                                            && (
            //                                                                    p.DeliveryStatus == DeliveryStatusEnum.Waiting
            //                                                                    || p.DeliveryStatus == DeliveryStatusEnum.Pending
            //                                                               )
            //                                                            && (
            //                                                                    p.ReleaseStatus == ReleaseStatusEnum.Pending
            //                                                                    || p.ReleaseStatus == ReleaseStatusEnum.Waiting
            //                                                                )
            //                                                      )
            //                                                .Sum(p => p.OnHand)
            //                   ));


            //#region Sales Order
            //var sales = this.stSales
            //                    .Include(p => p.SoldItems)
            //                    .Include(p => p.Deliveries)
            //                        .ThenInclude(p => p.ClientDeliveries)
            //                    .Where(p => p.StoreId == storeId
            //                                && p.SalesType == SalesTypeEnum.SalesOrder
            //                                && p.DeliveryType == DeliveryTypeEnum.Delivery);

            //var soldQty = 0;
            //var deliveredQty = 0;


            //soldQty = Convert.ToInt32(sales.Sum(p => p.SoldItems.Where(x => x.ItemId == itemId).Sum(y => y.Quantity)));

            //deliveredQty = Convert.ToInt32(sales.Sum(p => p.Deliveries.Sum(x => x.ClientDeliveries.Where(y => y.ItemId == itemId
            //                                                            && y.DeliveryStatus == DeliveryStatusEnum.Delivered
            //                                                            && y.ReleaseStatus == ReleaseStatusEnum.Released)
            //                                                .Sum(z => z.Quantity))));

            ////foreach (var sale in sales)
            ////{
            ////    soldQty += Convert.ToInt32(
            ////                                    sale.SoldItems
            ////                                        .Where(p => p.ItemId == itemId)
            ////                                        .Sum(p => p.Quantity)
            ////                );


            ////    foreach (var delivery in sale.Deliveries)
            ////    {
            ////        deliveredQty += Convert.ToInt32(
            ////                                            delivery.ClientDeliveries
            ////                                                .Where(p => p.ItemId == itemId
            ////                                                            && p.DeliveryStatus == DeliveryStatusEnum.Delivered
            ////                                                            && p.ReleaseStatus == ReleaseStatusEnum.Released)
            ////                                                .Sum(p => p.Quantity
            ////                                        )
            ////                        );

            ////    }

            ////}
            //#endregion

            //#region Transfer

            //int orderTransferQty, deliveredTransferQty;
            //GetTransferRecords(itemId, storeId, out orderTransferQty, out deliveredTransferQty);

            //#endregion

            //#region RTV

            //var objSTReturns = _context.STReturns
            //                           .Include(p => p.Deliveries)
            //                                .ThenInclude(p => p.WarehouseDeliveries)
            //                           .Where(p => p.StoreId == storeId
            //                                       && ((p.ReturnType == ReturnTypeEnum.RTV
            //                                       && p.RequestStatus == RequestStatusEnum.Approved) || (p.ReturnType == ReturnTypeEnum.Breakage
            //                                       && p.RequestStatus == RequestStatusEnum.Pending)));

            //var totalWarehouseDeliveries = 0;

            //if (objSTReturns != null && objSTReturns.Count() > 0)
            //{
            //    foreach (var obj in objSTReturns)
            //    {
            //        if (obj.Deliveries != null && obj.Deliveries.Count() > 0)
            //        {
            //            totalWarehouseDeliveries = Convert.ToInt32(obj.Deliveries.Sum(x => x.WarehouseDeliveries.Where(y => y.DeliveryStatus == DeliveryStatusEnum.Waiting
            //                                            && y.DeliveryStatus == DeliveryStatusEnum.Waiting
            //                                            && y.ItemId == itemId)
            //                                .Sum(z => z.Quantity)));
            //        }
            //    }
            //}


            #endregion




            return forReleasing + (soldQty - deliveredQty) + (orderTransferQty - deliveredTransferQty) + totalWarehouseDeliveries;
        }

        public int GetItemForReleasing2(int? itemId, int? storeId)
        {

            var forReleasing = Math.Abs(Convert.ToInt32(
                                                    this.stStock
                                                            .Where(p => p.StoreId == storeId
                                                                        && p.ItemId == itemId
                                                                        && (
                                                                                p.DeliveryStatus == DeliveryStatusEnum.Waiting
                                                                                || p.DeliveryStatus == DeliveryStatusEnum.Pending
                                                                           )
                                                                        && (
                                                                                p.ReleaseStatus == ReleaseStatusEnum.Pending
                                                                                || p.ReleaseStatus == ReleaseStatusEnum.Waiting
                                                                            )
                                                                  )
                                                            .Sum(p => p.OnHand)
                               ));


            #region Sales Order
            var sales = this.stSales
                                .Include(p => p.SoldItems)
                                .Include(p => p.Deliveries)
                                    .ThenInclude(p => p.ClientDeliveries)
                                .Where(p => p.StoreId == storeId
                                            && p.SalesType == SalesTypeEnum.SalesOrder
                                            && p.DeliveryType == DeliveryTypeEnum.Delivery);

            var soldQty = 0;
            var deliveredQty = 0;

            for (int i = 0; i < sales.Count(); i++)
            {

                var sale = sales.ToList();
                soldQty += Convert.ToInt32(
                                                sale[i].SoldItems
                                                    .Where(p => p.ItemId == itemId)
                                                    .Sum(p => p.Quantity)
                            );


                foreach (var delivery in sale[i].Deliveries)
                {
                    deliveredQty += Convert.ToInt32(
                                                        delivery.ClientDeliveries
                                                            .Where(p => p.ItemId == itemId
                                                                        && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                                        && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                            .Sum(p => p.Quantity
                                                    )
                                    );

                }
            }
            #endregion

            #region Transfer

            int orderTransferQty, deliveredTransferQty;
            GetTransferRecords(itemId, storeId, out orderTransferQty, out deliveredTransferQty);

            #endregion

            #region RTV

            var objSTReturns = this.stReturns
                                       .Include(p => p.Deliveries)
                                            .ThenInclude(p => p.WarehouseDeliveries)
                                       .Where(p => p.StoreId == storeId
                                                   && ((p.ReturnType == ReturnTypeEnum.RTV
                                                   && p.RequestStatus == RequestStatusEnum.Approved) || (p.ReturnType == ReturnTypeEnum.Breakage
                                                   && p.RequestStatus == RequestStatusEnum.Pending)));

            var totalWarehouseDeliveries = 0;
            if (objSTReturns != null && objSTReturns.Count() > 0)
            {

                for (int i = 0; i < objSTReturns.Count(); i++)
                {
                    var obj = objSTReturns.ToList();
                    if (obj[i].Deliveries != null && obj[i].Deliveries.Count() > 0)
                    {
                        for (int x = 0; x < obj[i].Deliveries.Count(); x++)
                        {
                            var del = obj[i].Deliveries.ToList();
                            var totQty = Convert.ToInt32(
                                            del[x].WarehouseDeliveries
                                            .Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting
                                                        && p.DeliveryStatus == DeliveryStatusEnum.Waiting
                                                        && p.ItemId == itemId)
                                            .Sum(p => p.Quantity)
                                         );

                            totalWarehouseDeliveries += totQty;
                        }
                        //foreach (var del in obj[i].Deliveries)
                        //{
                        //    var totQty = Convert.ToInt32(
                        //                    del.WarehouseDeliveries
                        //                    .Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting
                        //                                && p.DeliveryStatus == DeliveryStatusEnum.Waiting
                        //                                && p.ItemId == itemId)
                        //                    .Sum(p => p.Quantity)
                        //                 );

                        //    totalWarehouseDeliveries += totQty;

                        //}
                    }
                }
            }

            //if (objSTReturns != null && objSTReturns.Count() > 0)
            //{
            //    foreach (var obj in objSTReturns)
            //    {
            //        if (obj.Deliveries != null && obj.Deliveries.Count() > 0)
            //        {
            //            foreach (var del in obj.Deliveries)
            //            {
            //                var totQty = Convert.ToInt32(
            //                                del.WarehouseDeliveries
            //                                .Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting
            //                                            && p.DeliveryStatus == DeliveryStatusEnum.Waiting
            //                                            && p.ItemId == itemId)
            //                                .Sum(p => p.Quantity)
            //                             );

            //                totalWarehouseDeliveries += totQty;

            //            }
            //        }
            //    }
            //}


            #endregion




            return forReleasing + (soldQty - deliveredQty) + (orderTransferQty - deliveredTransferQty) + totalWarehouseDeliveries;
        }

        #endregion

        #region Broken

        public int GetItemBrokenQuantity(int? itemId, int? storeId)
        {
            var total = Convert.ToInt32(
                                            this.stStock
                                                .Where(p =>
                                                            p.StoreId == storeId
                                                            && p.ItemId == itemId
                                                            && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                            && p.Broken == true
                                                        )
                                                .Sum(y => y.OnHand)
                                        );

            return total;
        }

        #endregion

        #region Available

        public int GetItemAvailableQuantity(int? itemId, int? storeId, bool deductReleasing = false)
        {
            //var onHand = Convert.ToInt32(
            //                                this.stStock
            //                                    .Where(p =>
            //                                                p.StoreId == storeId
            //                                                && p.ItemId == itemId
            //                                                && (
            //                                                    p.DeliveryStatus == DeliveryStatusEnum.Delivered
            //                                                    || p.ReleaseStatus == ReleaseStatusEnum.Released
            //                                                )
            //                                                && p.Broken == false
            //                                            )
            //                                    .Sum(y => y.OnHand)
            //                            );

            //var releasing = 0;
            //if (deductReleasing)
            //{
            //    releasing = this.GetItemForReleasing(itemId, storeId);
            //}
            
            // Getting onhand using SQL function for faster results
            var onHand = _context.CustomQuantity.FromSql("SELECT dbo.FSSTGetOnhand({1}, {0}, {2}) as Total", itemId, storeId, deductReleasing).Sum(p => p.Total);

            return onHand;
        }

        public IEnumerable<object> GetStoreWithItemAvailable(int? storeId, ICollection<STTransferDetail> items)
        {
            //  Get all stores
            var stores = _context.Stores.Where(p => p.Id != storeId);

            var itemIdArr = items.Select(p => p.ItemId).ToArray();

            var records = new List<object>();

            foreach (var store in stores)
            {
                var storeStocks = _context.STStocks.Where(p => p.StoreId == store.Id && itemIdArr.Contains(p.ItemId)).GroupBy(p => p.ItemId);

                //  Check if store stocks and requested item count is equal
                if (storeStocks.Count() == items.Count)
                {
                    bool storeHasAllTheRequestedItems = true;
                    foreach (var stock in storeStocks)
                    {
                        var reqQty = items.Where(p => p.ItemId == stock.Key).Select(p => p.Quantity).FirstOrDefault();
                        if (reqQty != null)
                        {
                            //  Check if requested qty to transfer is less than or equal to 
                            //  item's available qty
                            if (reqQty <= this.GetItemAvailableQuantity(stock.Key, store.Id, true))
                            {
                                continue;
                            }
                            else
                            {
                                storeHasAllTheRequestedItems = false;
                                break;
                            }
                        }
                        else
                        {
                            storeHasAllTheRequestedItems = false;
                            break;
                        }
                    }

                    if (storeHasAllTheRequestedItems)
                    {
                        var currentStoreCompanyId = _context.Stores.Where(p => p.Id == storeId).Select(p => p.CompanyId).FirstOrDefault();
                        var storeInfo = _context.Stores.Where(p => p.Id == store.Id).FirstOrDefault();
                        if (storeInfo != null)
                        {
                            records.Add(new
                            {
                                StoreId = storeInfo.Id,
                                StoreName = (storeInfo.CompanyId == currentStoreCompanyId) ? storeInfo.Name : String.Concat(storeInfo.Name, " ( InterCompany )"),
                                IsInterBranch = (storeInfo.CompanyId == currentStoreCompanyId) ? true : false
                            });
                        }
                    }
                }
            }

            if (records.Count() == 0)
            {
                return null;
            }

            return records;
        }

        #endregion



        public int? GetDiscrepancy(int? pcount, int? systemcount)
        {
            return Convert.ToInt32(systemcount - pcount) < 0
                                                        ? Math.Abs(Convert.ToInt32(systemcount - pcount))
                                                        : -Convert.ToInt32(systemcount - pcount);
        }


        public STImport GetLatestItemAdjustment(int? itemId, int? storeId)
        {

            var import = new STImport();

            var lastAdjustment = stImportDetail.Where(p => p.ItemId == itemId && p.AllowUpdate == true).OrderByDescending(t => t.DateCreated).FirstOrDefault();

            if (lastAdjustment != null)
            {
                import = this.stImport.Where(p => p.Id == lastAdjustment.STImportId && p.StoreId == storeId).
                    Include(p => p.Details)
                    .FirstOrDefault();
                if (import != null)
                {
                    import.Details = import.Details?.Where(p => p.ItemId == itemId).ToList();
                }

            }

            return import;
        }




        private void GetTransferRecords(int? itemId, int? storeId, out int orderTransferQty, out int deliveredTransferQty)
        {

            #region old code
            //var orderTransfer = _context.STOrders
            //                            .Include(p => p.OrderedItems)
            //                            .Where(p => p.OrderToStoreId == storeId
            //                                        && p.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder);

            //orderTransferQty = 0;
            //deliveredTransferQty = 0;

            //foreach (var order in orderTransfer)
            //{
            //    if (order.DeliveryType != DeliveryTypeEnum.Pickup)
            //    {
            //        //  Get deliveries
            //        var deliveries = _context.STDeliveries
            //                                 .Include(p => p.ClientDeliveries)
            //                                 .Include(p => p.ShowroomDeliveries)
            //                                 .Where(p => p.STOrderId == order.Id);

            //        //  Check if delivery schedule has been created
            //        if (deliveries != null && deliveries.Count() > 0)
            //        {
            //            //foreach (var delivery in deliveries)
            //            //{

            //            //    if (order.DeliveryType == DeliveryTypeEnum.ShowroomPickup)
            //            //    {
            //            //        //  Get total showroom delivery quantity for releasing
            //            //        orderTransferQty += Convert.ToInt32
            //            //                (
            //            //                    delivery.ShowroomDeliveries
            //            //                            .Where(p => p.ItemId == itemId
            //            //                                        && (p.ReleaseStatus == ReleaseStatusEnum.Pending
            //            //                                            || p.ReleaseStatus == ReleaseStatusEnum.Waiting)
            //            //                                   )
            //            //                                .Sum(p => p.Quantity)
            //            //                );
            //            //    }
            //            //    else if(order.DeliveryType == DeliveryTypeEnum.Delivery)
            //            //    {
            //            //        //  Get total showroom delivery quantity for releasing
            //            //        orderTransferQty += Convert.ToInt32
            //            //                (
            //            //                    delivery.ClientDeliveries
            //            //                            .Where(p => p.ItemId == itemId
            //            //                                        && (p.ReleaseStatus == ReleaseStatusEnum.Pending
            //            //                                            || p.ReleaseStatus == ReleaseStatusEnum.Waiting)
            //            //                                   )
            //            //                                .Sum(p => p.Quantity)
            //            //                );
            //            //    }
            //            //}
            //        }
            //        else
            //        {
            //            //  Get order approved quantity
            //            orderTransferQty += Convert.ToInt32(order.OrderedItems
            //                                                     .Where(p => p.ItemId == itemId
            //                                                                 && p.ReleaseStatus == ReleaseStatusEnum.Waiting)
            //                                                     .Sum(p => p.ApprovedQuantity));
            //        }
            //    }
            //}

            #endregion old code

            stores = _context.Stores.AsNoTracking().ToList();
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

        /// <summary>
        /// Returns the list of items available for sales
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public IEnumerable<object> GetInventoriesByStoreIdForSales(int? storeId, InventorySearchDTO dto)
        {

            IList<InventoryInfo> records = (from y in _context.STStocks.AsNoTracking()
                                            where y.StoreId == storeId
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

            stStock = _context.STStocks.AsNoTracking();
            stSales = _context.STSales.AsNoTracking();
            stReturns = _context.STReturns.AsNoTracking();
            for (int i = 0; i < records.Count(); i++)
            {

                var forRelease = this.GetItemForReleasing(records[i].ItemId, storeId);
                var available = this.GetItemAvailableQuantity(records[i].ItemId, storeId);
                var broken = this.GetItemBrokenQuantity(records[i].ItemId, storeId);

                records[i].OnHand = forRelease + available + broken;

                records[i].Available = available - forRelease;

                items.Add(records[i]);


            }

            // Get All avaiable Items on inventory
            items = items.Where(p => p.OnHand > 0).ToList();

            if (dto.Id.HasValue)
            {
                items = items.Where(p => p.ItemId == dto.Id).ToList();
            }

            if (!string.IsNullOrEmpty(dto.SerialNumber))
            {
                items = items.Where(p => p.SerialNumber == dto.SerialNumber).ToList();
            }


            return items;
        }

        public IEnumerable<object> GetInventoriesByStoreIdForSalesFromSTStockSummary(int? storeId, InventorySearchDTO dto)
        {

            IQueryable<STStockSummary> records = _context.STStockSummary.Where(p => p.StoreId == storeId).AsNoTracking().OrderByDescending(p => p.SerialNumber);

            var inactiveItemId = _context.Items.Where(p => p.IsActive == false).Select(p => (int?)p.Id);

            records = records.Where(p => !inactiveItemId.Contains(p.ItemId));

            // Get All avaiable Items on inventory
            records = records.Where(p => p.OnHand > 0);

            if (dto.Id.HasValue)
            {
                records = records.Where(p => p.ItemId == dto.Id);
            }

            if (!string.IsNullOrEmpty(dto.SerialNumber))
            {
                records = records.Where(p => p.SerialNumber == dto.SerialNumber);
            }

            return records;
        }

        /// <summary>
        /// Returns the list of items available for sales
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public IEnumerable<object> GetInventoriesByStoreIdForSalesDropdown(int? storeId, InventorySearchDTO dto)
        {

            IQueryable<Item> item = _context.Items.AsNoTracking();

            List<InventoryInfo> records = (from y in _context.STStocks.AsNoTracking()
                                           where y.StoreId == storeId
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
                                               Remarks = x.Select(c => c.Remarks).FirstOrDefault(),
                                               ImageName = item.Where(p => p.Id == x.Key).Select(p => p.ImageName).FirstOrDefault()
                                           }).ToList();

            var items = new List<InventoryInfo>();

            stOrder = _context.STOrders.AsNoTracking()
                .Include(p => p.OrderedItems);

            stStock = _context.STStocks.AsNoTracking();

            //GetItemAvailableQuantity
            this.stImportDetail = _context.STImportDetails.AsNoTracking().OrderByDescending(t => t.DateCreated);
            stImport = _context.STImports.AsNoTracking();

            stSales = _context.STSales;


            for (int i = 0; i < records.Count(); i++)
            {
                var totalItemForReleasing = this.GetItemForReleasing(records[i].ItemId, storeId);

                records[i].ForRelease = totalItemForReleasing;

                var totalItemAvailable = this.GetItemAvailableQuantity(records[i].ItemId, storeId, false);

                var totalItemBroken = this.GetItemBrokenQuantity(records[i].ItemId, storeId);

                records[i].Broken = totalItemBroken;

                records[i].OnHand = totalItemAvailable + totalItemBroken;

                records[i].Available = totalItemAvailable - totalItemForReleasing;

                records[i].ImportDetails = this.GetLatestItemAdjustment(records[i].ItemId, storeId);

                items.Add(records[i]);
            }


            // Get All avaiable Items on inventory
            items = items.Where(p => p.OnHand > 0).ToList();

            return items;
        }

        public IEnumerable<object> GetInventoriesByStoreIdForSalesDropdownFromSTStockSummary(int? storeId, InventorySearchDTO dto)
        {
            IEnumerable<STStockSummary> records = _context.STStockSummary.Where(p => p.StoreId == storeId).AsNoTracking().OrderByDescending(p => p.SerialNumber);

            // Get All avaiable Items on inventory
            records = records.Where(p => p.OnHand > 0);

            return records;
        }




        public object GetStoresWithItem(InventorySearchDTO search)
        {
            var currentStoreDetails = _context.Stores.Where(p => p.Id == search.StoreId).FirstOrDefault();
            //Stores Having the item.

            if (!string.IsNullOrEmpty(search.SerialNumber))
            {
                // Get item id of item based on serial number.
                search.Id = _context.Items.Where(p => p.SerialNumber == search.SerialNumber).Select(p => p.Id).FirstOrDefault();
            }

            var storeIds = new List<int?>();
            //remove 1/17/2020 as per sir bert request https://redmine.blotocol.com/issues/1791
            //added for dealers will filter the inventory of stores under dealer account 
            //if (search.UserType == UserTypeEnum.Dealer)
            //{
            //    storeIds = _context.StoreDealerAssignment.Where(p => p.UserId == search.UserId && p.StoreId != search.StoreId).Select(p => p.StoreId).ToList();

            //}
            //else
            //{
            storeIds = _context.STStocks.AsNoTracking().Where(p => p.StoreId != search.StoreId && p.ItemId == search.Id).Select(p => p.StoreId).Distinct().ToList();
            //}
           
            this.stOrder = _context.STOrders.AsNoTracking();
            this.stStock = _context.STStocks.AsNoTracking();
            this.stSales = _context.STSales.AsNoTracking();

            List<object> Stores = new List<object>();
            for (int i = 0; i < storeIds.Count(); i++)
            {
                var Available = this.GetItemAvailableQuantity(search.Id, storeIds[i], true);

                if (Available > 0)
                {

                    var storeDetails = _context.Stores.Where(p => p.Id == storeIds[i]).FirstOrDefault();
                    var IsInterBranch = (currentStoreDetails.CompanyId == storeDetails.CompanyId) ? true : false;

                    var obj = new
                    {
                        StoreId = storeDetails.Id,
                        CompanyId = storeDetails.CompanyId,
                        StoreName = (!IsInterBranch) ? storeDetails.Name + " (InterCompany)" : storeDetails.Name,
                        IsInterBranch = IsInterBranch,
                        Available
                    };

                    Stores.Add(obj);
                }
            }

            if (Stores.Count() > 0)
            {
                // added is active flg task #1521
                var itemDetails = _context.Items
                                            .Include(p => p.Size)
                                            .Where(p => p.Id == search.Id && p.IsActive == true).FirstOrDefault();

                return new
                {
                    Id = itemDetails.Id,
                    SerialNumber = itemDetails.SerialNumber,
                    Code = itemDetails.Code,
                    ItemName = itemDetails.Name,
                    SizeName = itemDetails.Size.Name,
                    Tonality = itemDetails.Tonality,
                    Stores
                };
            }

            return null;
        }








        public bool IsStoreHasAllItems(int? storeId, IQueryable<STTransferDetail> items)
        {
            bool storeHasAllTheRequestedItems = true;

            var itemIdArr = items.Select(p => p.ItemId).ToArray();

            var storeStocks = _context.STStocks.Where(p => p.StoreId == storeId && itemIdArr.Contains(p.ItemId)).GroupBy(p => p.ItemId);

            //  Check if store stocks and requested item count is equal
            if (storeStocks.Count() == items.Count())
            {
                foreach (var stock in storeStocks)
                {
                    var reqQty = items.Where(p => p.ItemId == stock.Key).Select(p => p.Quantity).FirstOrDefault();
                    if (reqQty != null)
                    {
                        //  Check if requested qty to transfer is less than or equal to 
                        //  item's available qty
                        if (reqQty <= this.GetItemAvailableQuantity(stock.Key, storeId, true))
                        {
                            continue;
                        }
                        else
                        {
                            storeHasAllTheRequestedItems = false;
                            break;
                        }
                    }
                    else
                    {
                        storeHasAllTheRequestedItems = false;
                        break;
                    }
                }
            }
            else
            {
                storeHasAllTheRequestedItems = false;
            }

            return storeHasAllTheRequestedItems;
        }

        public void UpdateSTStock(STStock stStock)
        {
            stStock.DateUpdated = DateTime.Now;

            _context.STStocks.Update(stStock);
            _context.SaveChanges();
        }

        #region Old Functions

        public IEnumerable<object> GetInventoriesByStoreIdOld(InventorySearchDTO search)
        {
            IQueryable<Item> item = _context.Items.AsNoTracking();

            List<InventoryInfo> records = (from y in _context.STStocks.AsNoTracking()
                                           where y.StoreId == search.StoreId
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
                                               Remarks = x.Select(c => c.Remarks).FirstOrDefault(),
                                               ImageName = item.Where(p => p.Id == x.Key).Select(p => p.ImageName).FirstOrDefault()
                                           }).ToList();

            var items = new List<InventoryInfo>();

            stOrder = _context.STOrders.AsNoTracking()
                .Include(p => p.OrderedItems);

            stStock = _context.STStocks.AsNoTracking();

            //GetItemAvailableQuantity
            this.stImportDetail = _context.STImportDetails.AsNoTracking().OrderByDescending(t => t.DateCreated);
            stImport = _context.STImports.AsNoTracking();

            stSales = _context.STSales;


            for (int i = 0; i < records.Count(); i++)
            {
                var totalItemForReleasing = this.GetItemForReleasing(records[i].ItemId, search.StoreId);

                records[i].ForRelease = totalItemForReleasing;

                var totalItemAvailable = this.GetItemAvailableQuantity(records[i].ItemId, search.StoreId, false);

                var totalItemBroken = this.GetItemBrokenQuantity(records[i].ItemId, search.StoreId);

                records[i].Broken = totalItemBroken;

                records[i].OnHand = totalItemAvailable + totalItemBroken;

                records[i].Available = totalItemAvailable - totalItemForReleasing;

                records[i].ImportDetails = this.GetLatestItemAdjustment(records[i].ItemId, search.StoreId);

                items.Add(records[i]);
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

        public IEnumerable<object> GetInventoriesSummaryByStoreIdOld(int? storeId)
        {

            List<InventoryInfo> records = (from y in _context.STStocks
                                           where y.StoreId == storeId
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
                                               Remarks = x.Select(c => c.Remarks).FirstOrDefault(),
                                               ImageName = _context.Items.Where(p => p.Id == x.Key).Select(p => p.ImageName).FirstOrDefault()

                                           }).ToList();

            var items = new List<InventoryInfo>();

            this.stStock = _context.STStocks;
            this.stImportDetail = _context.STImportDetails.AsNoTracking();
            this.stImport = _context.STImports.AsNoTracking();
            this.stSales = _context.STSales.AsNoTracking()
                                .Include(p => p.SoldItems)
                                .Include(p => p.Deliveries)
                                    .ThenInclude(p => p.ClientDeliveries)
                                .Where(p => p.StoreId == storeId
                                            && p.SalesType == SalesTypeEnum.SalesOrder
                                            && p.DeliveryType == DeliveryTypeEnum.Delivery); ;

            for (int i = 0; i < records.Count(); i++)
            {
                var totalItemForReleasing = this.GetItemForReleasing(records[i].ItemId, storeId);

                records[i].ForRelease = totalItemForReleasing;

                // Get the last item adjustment by item ID
                var adj = this.GetLatestItemAdjustment(records[i].ItemId, storeId);

                if (adj != null)
                {
                    if (adj.Details != null)
                    {
                        records[i].adjustment = this.GetDiscrepancy(adj.Details.Select(p => p.PhysicalCount).FirstOrDefault(), adj.Details.Select(p => p.SystemCount).FirstOrDefault());
                        records[i].dateCreated = adj.Details.Select(p => p.DateCreated).FirstOrDefault();
                        records[i].physicalCount = adj.Details.Select(p => p.PhysicalCount).FirstOrDefault();
                    }

                }
                items.Add(records[i]);
            }


            return items;
        }

        #endregion

    }

}

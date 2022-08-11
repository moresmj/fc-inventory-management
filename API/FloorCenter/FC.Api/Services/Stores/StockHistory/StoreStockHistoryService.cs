using FC.Api.DTOs.Inventories;
using FC.Api.DTOs.StockHistory;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Services.Stores.StockHistory
{
    public class StoreStockHistoryService : IStoreStockHistoryService
    {

        private DataContext _context;

        // Used by Item History[Store and Main] and Inventory Monitoring[Main]
        private StockHistoryUserEnum StockHistoryUser { get; set; }

        private List<Store> stores { get; set; }
        private List<STOrder> orders { get; set; }

        public StoreStockHistoryService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return this._context;
        }

        public IEnumerable<StockHistoryDTO> GetMainSelectedStoreItemHistory(InventorySearchDTO dto)
        {
            return this.GetByItemIdAndStoreId((int)dto.Id, dto.StoreId);
        }

        public IEnumerable<StockHistoryDTO> GetInventoryStockHistory(InventorySearchDTO dto, InventoryMonitoringTypeEnum type)
        {
            this.StockHistoryUser = StockHistoryUserEnum.InventoryMonitoring;

            List<STStock> stocks = (_context.STStocks.AsNoTracking()
                                                 .Include(p => p.Item).
                                                    ThenInclude(p => p.Size)
                                                 .Include(p => p.STShowroomDelivery)
                                                 .Include(p => p.STSalesDetail)
                                                 .Include(p => p.STOrderDetail)
                                                 .Include(p => p.STClientDelivery)
                                                 .Include(p => p.STClientReturn)
                                                 .Include(p => p.WHDeliveryDetail)
                                                 .Include(p => p.STImportDetail)
                                                 .Where(p => p.StoreId == dto.StoreId && (p.IsDeliveryTransfer == true ? p.ReleaseStatus == ReleaseStatusEnum.Released : p.DeliveryStatus == DeliveryStatusEnum.Delivered)).OrderBy(p => p.Id)).ToList();
            //added condition for ticket #421



            if (dto.DateFrom.HasValue)
            {
                stocks = stocks.Where(p => dto.DateFrom.Value.AddSeconds(-1) <= p.DateCreated).ToList();
            }

            if (dto.DateTo.HasValue)
            {
                stocks = stocks.Where(p => dto.DateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59) >= p.DateCreated).ToList();

       
            }


            // Advanced Search - Inventory Monitoring
            if (!string.IsNullOrEmpty(dto.SerialNumber))
            {
                stocks = stocks.Where(p => p.Item.SerialNumber == dto.SerialNumber).ToList();
            }

            if (!string.IsNullOrEmpty(dto.Code))
            {
                stocks = stocks.Where(p => p.Item.Code.ToLower().Contains(dto.Code.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(dto.ItemName))
            {
                stocks = stocks.Where(p => p.Item.Name.ToLower().Contains(dto.ItemName.ToLower())).ToList();
            }

            if (dto.SizeId.HasValue)
            {
                stocks = stocks.Where(p => p.Item.Size.Id == dto.SizeId).ToList();
            }

            if (!string.IsNullOrEmpty(dto.Tonality))
            {
                stocks = stocks.Where(p => p.Item.Tonality.ToLower().Contains(dto.Tonality.ToLower())).ToList();
            }

            if (type == InventoryMonitoringTypeEnum.Incoming)
            {
                stocks = stocks.Where(p => p.OnHand > 0).ToList();
            }
            else if (type == InventoryMonitoringTypeEnum.Outgoing)
            {
                stocks = stocks.Where(p => p.OnHand < 0).ToList();
            }


            var records = this.GetStockHistoryRecords(stocks.ToList(), dto.StoreId);


            return records;
        }


        public object GetInventoryStockHistoryPaged(InventorySearchDTO search, InventoryMonitoringTypeEnum type, AppSettings appSettings)
        {
            this.StockHistoryUser = StockHistoryUserEnum.InventoryMonitoring;

            IQueryable<STStock> stocks = (_context.STStocks.AsNoTracking()
                                                 .Include(p => p.Item).
                                                    ThenInclude(p => p.Size)
                                                 .Include(p => p.STShowroomDelivery)
                                                 .Include(p => p.STSalesDetail)
                                                 .Include(p => p.STOrderDetail)
                                                 .Include(p => p.STClientDelivery)
                                                 .Include(p => p.STClientReturn)
                                                 .Include(p => p.WHDeliveryDetail)
                                                 .Include(p => p.STImportDetail)
                                                 .Where(p => p.StoreId == search.StoreId && (p.IsDeliveryTransfer == true ? p.ReleaseStatus == ReleaseStatusEnum.Released : p.DeliveryStatus == DeliveryStatusEnum.Delivered)).OrderBy(p => p.Id));
            //added condition for ticket #421



            if (search.DateFrom.HasValue)
            {
                stocks = stocks.Where(p => search.DateFrom.Value.AddSeconds(-1) <= p.DateCreated);
            }

            if (search.DateTo.HasValue)
            {
                stocks = stocks.Where(p => search.DateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59) >= p.DateCreated);


            }


            // Advanced Search - Inventory Monitoring
            if (!string.IsNullOrEmpty(search.SerialNumber))
            {
                stocks = stocks.Where(p => p.Item.SerialNumber == search.SerialNumber);
            }

            if (!string.IsNullOrEmpty(search.Code))
            {
                stocks = stocks.Where(p => p.Item.Code.ToLower().Contains(search.Code.ToLower()));
            }

            if (!string.IsNullOrEmpty(search.ItemName))
            {
                stocks = stocks.Where(p => p.Item.Name.ToLower().Contains(search.ItemName.ToLower()));
            }

            if (search.SizeId.HasValue)
            {
                stocks = stocks.Where(p => p.Item.Size.Id == search.SizeId);
            }

            if (!string.IsNullOrEmpty(search.Tonality))
            {
                stocks = stocks.Where(p => p.Item.Tonality.ToLower().Contains(search.Tonality.ToLower()));
            }

            if (type == InventoryMonitoringTypeEnum.Incoming)
            {
                stocks = stocks.Where(p => p.OnHand > 0);
            }
            else if (type == InventoryMonitoringTypeEnum.Outgoing)
            {
                stocks = stocks.Where(p => p.OnHand < 0);
            }

            GetAllResponse response = null;

            if (search.ShowAll == false)
            {
                response = new GetAllResponse(stocks.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;
                }

                stocks = stocks.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                    .Take(appSettings.RecordDisplayPerPage);
            }
            else
            {
                response = new GetAllResponse(stocks.Count());

            }

        

            var records = this.GetStockHistoryRecords(stocks.ToList(), search.StoreId);

            response.List.AddRange(records);

            return response;
        }


        public IEnumerable<StockHistoryDTO> GetByItemIdAndStoreId(int id, int? storeId)
        {
            this.StockHistoryUser = StockHistoryUserEnum.ItemHistory;

            List<STStock> stocks = _context.STStocks.AsNoTracking()
                                                 .Include(p => p.STShowroomDelivery)
                                                 .Include(p => p.STSalesDetail)
                                                 .Include(p => p.STOrderDetail)
                                                 .Include(p => p.STClientDelivery)
                                                 .Include(p => p.STClientReturn)
                                                 .Include(p => p.WHDeliveryDetail)
                                                 .Include(p => p.STImportDetail)
                                                 .Where(p => p.StoreId == storeId
                                                             && p.ItemId == id && (p.IsDeliveryTransfer == true ? p.ReleaseStatus == ReleaseStatusEnum.Released : p.DeliveryStatus == DeliveryStatusEnum.Delivered)).ToList();

            return this.GetStockHistoryRecords(stocks, storeId);
        }

        public List<StockHistoryDTO> GetStockHistoryRecords(List<STStock> stocks, int? storeId)
        {

            var info = _context.Stores
                   .Where(p => p.Id == storeId)
                   .Include(p => p.Warehouse)
                   .FirstOrDefault();



            var records = new List<StockHistoryDTO>();


            var stockList = stocks.ToList();

            orders = _context.STOrders.AsNoTracking()
                                    .Include(p => p.Warehouse)
                                    .Include(p => p.Store).ToList();
            // Get All Stores
            stores = _context.Stores.AsNoTracking()
                                    .Include(p => p.Company).ToList();


            for (int i = 0; i < stocks.Count(); i++)
            {
                var stock = stocks[i];

                var itemInfo = _context.Items.Where(p => p.Id == stock.ItemId).FirstOrDefault();

                var history = new StockHistoryDTO
                {
                    Code = itemInfo.Code,
                    Description = itemInfo.Description,
                    Tonality = itemInfo.Tonality,
                    SizeName = _context.Sizes.Where(p => p.Id == itemInfo.SizeId).Select(p => p.Name).FirstOrDefault(),
                    Stock = stock.OnHand,
                    Origin = info.Name,
                    TransactionDate = (stock.DateUpdated != null) ? stock.DateUpdated : stock.DateCreated,
                    Broken = stock.Broken
                };

                if (stock.STShowroomDeliveryId.HasValue)
                {
                    // FromSupplier - FromInterBranch
                    Get_Info_From_STShowroomDelivery(stock, history);
                }
                else if (stock.STSalesDetailId.HasValue)
                {
                    // FromSupplier
                    history.FromSupplier = history.Stock;
                    Get_Info_From_STSales(stock, info, history);
                    
                }
                else if (stock.STOrderDetailId.HasValue)
                {
                    // FromSupplier - FromInterBranch
                    Get_Info_From_STOrder(stock, history);
                }
                else if (stock.STClientDeliveryId.HasValue)
                {
                    // FromSupplier
                    history.FromSupplier = history.Stock;
                    Get_Info_From_STClientDelivery(stock, history);
           
                    if (stock.ReleaseStatus == ReleaseStatusEnum.Waiting)
                    {
                        continue;
                    }
                }
                else if (stock.STClientReturnId.HasValue)
                {
                    // FromSalesReturns
                    history.FromSalesReturns = history.Stock;
                    Get_Info_From_STReturn(stock, history);
                }
                else if (stock.WHDeliveryDetailId.HasValue)
                {
                    // RTV
                    history.RTV = history.Stock;
                    Get_Info_From_WHDelivery(stock, history);
                }
                else if (stock.STImportDetailId.HasValue)
                {
                    // Adjustment
                    history.Adjustment = history.Stock;
                    Get_Info_From_STImport(stock, history);
                }

                records.Add(history);

            }

            records = records.OrderByDescending(p => p.TransactionDate).ToList();
            return records;
        }


        private void Get_Info_From_STShowroomDelivery(STStock stock, StockHistoryDTO history)
        {

            var obj = _context.STDeliveries.AsNoTracking()
                              .Include(p => p.Store)
                              .Where(p => p.Id == stock.STShowroomDelivery.STDeliveryId)
                              .FirstOrDefault();

            if (obj != null)
            {
                history.DRNumber = obj.DRNumber;
                history.DeliveryDate = obj.ApprovedDeliveryDate;
                history.ReleaseDate = obj.ReleaseDate;
                history.ORNumber = obj.ORNumber;
                history.SINumber = obj.SINumber;

                var order = orders
                                    .Where(p => p.Id == obj.STOrderId)
                                    .FirstOrDefault();


                if (order != null)
                {
                    if (order.OrderType == OrderTypeEnum.ShowroomStockOrder)
                    {
                        history.Origin = order.Warehouse.Code;
                        history.Destination = order.Store.Name;

                        //for incoming showroom stock order whdrnumber will be displayed added for ticket #220
                        history.DRNumber = order.WHDRNumber;


                        if (order.Warehouse.Vendor)
                        {
                            history.FromOtherSupplier = history.Stock;
                            //added for ticket #286 display drnumber if vendor
                            history.DRNumber = obj.DRNumber;
                        }
                        else
                        {
                            history.FromSupplier = history.Stock;
                        }


                    }
                    else if (order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                    {

                        var objStoreOrigin = _context.Stores
                                                     .Where(p => p.Id == order.OrderToStoreId)
                                                     .FirstOrDefault();

                        var isInterBranch = (order.Store.CompanyId == objStoreOrigin.CompanyId);
                        var isRequestee = (stock.StoreId != order.StoreId);

                        if (objStoreOrigin != null)
                        {
                            history.Origin = objStoreOrigin.Name;
                            history.Destination = order.Store.Name;
                        }

                        if (isInterBranch)
                        {
                            history.FromInterBranch = history.Stock;
                        }
                        else
                        {
                            history.FromInterCompany = history.Stock;
                        }

                        // Requestor : [SINumber = null] : 
                        history.SINumber = (!isRequestee) ? null : obj.SINumber;


                        if (stock.OnHand > 0)
                        {
                            if (!isRequestee)
                            {
                                if (isInterBranch)
                                {
                                    history.DRNumber = order.ORNumber;
                                    history.SINumber = null;
                                }
                                else
                                {
                                    history.DRNumber = order.WHDRNumber;
                                    history.SINumber = order.SINumber;
                                }
                            }
                        }
                        else
                        {
                            if (isInterBranch)
                            {
                                history.DRNumber = order.ORNumber;
                                history.SINumber = null;
                            }
                            else
                            {
                                history.DRNumber = order.WHDRNumber;
                                history.SINumber = order.SINumber;
                            }
                        }

                    }
                    else
                    {
                        history.Origin = order.Warehouse.Code;
                        history.Destination = order.Store.Name;
                        if (order.Warehouse.Vendor)
                        {
                            history.FromOtherSupplier = history.Stock;
                        }
                        else
                        {
                            history.FromSupplier = history.Stock;
                        }
                    }


                    history.TransactionNo = order.TransactionNo;
                    history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), order.OrderType));
                    history.PONumber = order.PONumber;
                    history.PODate = order.PODate;
                }

            }

        }

        private void Get_Info_From_STSales(STStock stock, Store store, StockHistoryDTO history)
        {
            var obj = _context.STSales.AsNoTracking()
                              .Where(p => p.Id == stock.STSalesDetail.STSalesId)
                              .FirstOrDefault();

            if (obj != null)
            {
                history.ORNumber = obj.ORNumber;
                history.DRNumber = obj.DRNumber;
                history.SINumber = obj.SINumber;
                history.ReleaseDate = obj.ReleaseDate;
                history.SalesDate = obj.SalesDate;
                history.TransactionNo = obj.TransactionNo;
                history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(SalesTypeEnum), obj.SalesType));

                var order = orders.Where(p => p.Id == obj.STOrderId).FirstOrDefault();

                if(obj.SalesType == SalesTypeEnum.Releasing && obj.DeliveryType == DeliveryTypeEnum.Pickup)
                {
                    history.TransactionDate = obj.ReleaseDate;
                }


                if (obj.SalesType == SalesTypeEnum.Interbranch || obj.SalesType == SalesTypeEnum.Intercompany)
                {
                    history.Origin = store.Name;
                    history.Destination = obj.ClientName;
                    history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(SalesTypeEnum), SalesTypeEnum.ClientOrder));

                    if (StockHistoryUser == StockHistoryUserEnum.InventoryMonitoring)
                    {
                        if (order.DeliveryType == DeliveryTypeEnum.Pickup)
                        {
                            history.SINumber = order.ClientSINumber;
                        }

                        history.FromSupplier = null;
                    }

                    var FromStoreCompany = stores.Where(p => p.Id == order.StoreId).Select(p => p.Company).FirstOrDefault();
                    var ToStoreCompany = stores.Where(p => p.Id == order.OrderToStoreId).Select(p => p.Company).FirstOrDefault();

                    var isInterBranch = (FromStoreCompany.Id == ToStoreCompany.Id);
                    var isRequestee = (order.StoreId != store.Id);

                    if (isRequestee)
                    {
                        if (isInterBranch)
                        {
                            history.FromInterBranch = stock.OnHand;
                        }
                        else
                        {
                            history.FromInterCompany = stock.OnHand;
                        }
                    }
                    else
                    {
                        history.FromSupplier = stock.OnHand;
                    }

                }
                else
                {
                    if (obj.SalesType.HasValue)
                    {
                        //history.Destination = EnumExtensions.SplitName(Enum.GetName(typeof(SalesTypeEnum), obj.SalesType));
                        history.Destination = obj.ClientName;
                    }
                }



                if (order != null)
                {
                    history.PONumber = order.PONumber;
                    history.PODate = order.PODate;
                }

            }

        }

        private void Get_Info_From_STOrder(STStock stock, StockHistoryDTO history)
        {
            var obj = _context.STOrders.AsNoTracking()
                              .Where(p => p.Id == stock.STOrderDetail.STOrderId && p.OrderedItems.Where(x => x.DeliveryStatus == DeliveryStatusEnum.Delivered).Count() > 0)
                              .FirstOrDefault();




            if (obj != null)
            {
                var IsInterBranch = this.IsInterBranchOrder(obj.StoreId, obj.OrderToStoreId);
                var isRequestee = (stock.StoreId != obj.StoreId);

                if (obj.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder && obj.DeliveryType == DeliveryTypeEnum.Pickup)
                {
                    // Interbrach : ORNumber : InterCompany : WHDRNmuber
                    history.DRNumber = (IsInterBranch) ? obj.ORNumber : obj.WHDRNumber;
                }

                history.PODate = obj.PODate;
                history.PONumber = obj.PONumber;
                history.ORNumber = obj.ORNumber;
                history.SINumber = obj.SINumber;
                history.ReleaseDate = obj.ReleaseDate;
                history.TransactionNo = obj.TransactionNo;
                history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), obj.OrderType));
                if (obj.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                {

                    if (StockHistoryUser == StockHistoryUserEnum.InventoryMonitoring)
                    {
                        // Requestor : [SINumber = null]
                        history.SINumber = (!isRequestee) ? null : obj.SINumber;
                    }

                    if (IsInterBranch)
                    {
                        history.FromInterBranch = history.Stock;
                    }
                    else
                    {
                        history.FromInterCompany = history.Stock;
                    }



                    history.Origin = stores.Where(p => p.Id == obj.OrderToStoreId).Select(p => p.Name).FirstOrDefault();
                    history.Destination = stores.Where(p => p.Id == obj.StoreId).Select(p => p.Name).FirstOrDefault();
                    history.ClientSINumber = obj.ClientSINumber;

                }
                else
                {
                    history.FromSupplier = history.Stock;
                    history.Origin = obj.Warehouse.Code;
                    history.Destination = obj.Store.Name;
                }


            }

        }

        private void Get_Info_From_STClientDelivery(STStock stock, StockHistoryDTO history)
        {
            var obj = _context.STDeliveries.AsNoTracking()
                              .Where(p => p.Id == stock.STClientDelivery.STDeliveryId)
                              .FirstOrDefault();

            if (obj != null)
                {
                history.DRNumber = obj.DRNumber;
                history.DeliveryDate = obj.ApprovedDeliveryDate;
                history.ReleaseDate = obj.ReleaseDate;
                history.ORNumber = obj.ORNumber;
                history.SINumber = obj.SINumber;

                if (obj.STOrderId.HasValue)
                {
                    var order = orders
                                        .Where(p => p.Id == obj.STOrderId)
                                        .FirstOrDefault();



                    if (order != null)
                    {
                        history.TransactionNo = order.TransactionNo;
                        history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), order.OrderType));
                        history.PONumber = order.PONumber;
                        history.PODate = order.PODate;

                        //if order number is incoming whdrnumber will be displayed ticket #220
                        if(history.Stock > 0)
                        {
                            history.DRNumber = order.WHDRNumber;
                        }
                        if (order.Warehouse != null)
                        {
                            if (order.Warehouse.Vendor)
                            {
                                history.FromOtherSupplier = history.Stock;
                                history.FromSupplier = null;
                            }
                        }


                        if (order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                        {
                            var storeCompany = stores.Where(p => p.Id == order.StoreId).FirstOrDefault();
                            var orderToStoreCompany = stores.Where(p => p.Id == order.StoreId).FirstOrDefault();

                            var isInterbranch = this.IsInterBranchOrder(order.StoreId, order.OrderToStoreId);
                            var isRequestee = (order.StoreId != stock.StoreId);


                            if (stock.OnHand < 0)    // Outgoing
                            {
                                if (isInterbranch)
                                {
                                    history.FromInterBranch = stock.OnHand;
                                }
                                else
                                {
                                    history.FromInterCompany = stock.OnHand;
                                }
 
                                history.FromSupplier = null;

                                // Requestor : [SINumber = null]
                                history.SINumber = (stock.StoreId == obj.StoreId) ? null : obj.SINumber;
                            }
                            else
                            {
                                history.Origin = stores.Where(p => p.Id == order.OrderToStoreId).Select(p => p.Name).FirstOrDefault();
                                history.Destination = stores.Where(p => p.Id == order.StoreId).Select(p => p.Name).FirstOrDefault();

                                if (isInterbranch)
                                {
                                    history.FromInterBranch = history.Stock;
                                }
                                else
                                {
                                    history.FromInterCompany = history.Stock;
                                }
                                history.FromSupplier = null;
                                history.FromOtherSupplier = null;
                            }

                            // https://redmine.blotocol.com/issues/25

                            if (StockHistoryUser == StockHistoryUserEnum.ItemHistory)
                            {
                                if (isInterbranch)
                                {
                                    history.DRNumber = order.ORNumber;
                                    history.SINumber = null;
                                }
                                else
                                {
                                    history.DRNumber = order.WHDRNumber;
                                    history.SINumber = order.SINumber;
                                }
                            }
                            else
                            {
                                if (stock.OnHand > 0)
                                {
                                    if (!isRequestee)
                                    {
                                        history.DRNumber = (isInterbranch) ? order.ORNumber : order.WHDRNumber;
                                    }
                                }
                                else
                                {
                                    if (isRequestee)
                                    {
                                        history.DRNumber = (isInterbranch) ? order.ORNumber : order.WHDRNumber;
                                        history.SINumber = (isInterbranch) ?  null : order.SINumber;
                                    }
                                }
                            }


                        }
                        else
                        {
                            history.Origin = _context.Warehouses.Where(p => p.Id == order.WarehouseId).Select(p => p.Code).FirstOrDefault();
                            history.Destination = stores.Where(p => p.Id == order.StoreId).Select(p => p.Name).FirstOrDefault();
                        }
                    }
                }
                else if (obj.STSalesId.HasValue)
                {
                    var sales = _context.STSales.AsNoTracking()
                                        .Where(p => p.Id == obj.STSalesId)
                                        .FirstOrDefault();

                    if (sales != null)
                    {
                        history.TransactionNo = sales.TransactionNo;
                        history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(SalesTypeEnum), SalesTypeEnum.SalesOrder));
                        history.DRNumber = sales.DRNumber;
                        history.ORNumber = sales.ORNumber;
                        history.SINumber = sales.SINumber;
                        history.SalesDate = sales.SalesDate;
                        history.Destination = sales.ClientName;
                    }

                }
            }
        }

        private void Get_Info_From_STReturn(STStock stock, StockHistoryDTO history)
        {
            var obj = _context.STReturns.AsNoTracking().Include(p => p.Store)
                              .Where(p => p.Id == stock.STClientReturn.STReturnId)
                              .FirstOrDefault();
            if (obj != null)
            {
                history.TransactionNo = obj.TransactionNo;
                history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), ReturnTypeEnum.ClientReturn));


                //return dr number will not be displayed if incoming added for ticket #220
                if (history.Stock < 0)
                {
                    history.DRNumber = obj.ReturnDRNumber;
                }
                
                history.DeliveryDate = obj.ApprovedDeliveryDate;
                history.Destination = obj.Store.Name;

                var sales = _context.STSales
                                        .Where(p => p.Id == obj.STSalesId)
                                        .FirstOrDefault();
                history.SINumber = sales.SINumber;
                history.Origin = sales.ClientName;
            }
        }

        private void Get_Info_From_WHDelivery(STStock stock, StockHistoryDTO history)
        {
            var obj = _context.WHDeliveries.AsNoTracking()
                              .Include(p => p.Store)
                              .Where(p => p.Id == stock.WHDeliveryDetail.WHDeliveryId)
                              .FirstOrDefault();
            if (obj != null)
            {
                history.DRNumber = obj.DRNumber;
                history.ReleaseDate = obj.ReleaseDate;

                if (obj.Store != null)
                {
                    history.Origin = obj.Store.Name;
                }

                var objSTReturn = _context.STReturns.AsNoTracking()
                                          .Include(p => p.Warehouse)
                                          .Where(p => p.Id == obj.STReturnId)
                                          .FirstOrDefault();
                if (objSTReturn != null)
                {
                    history.TransactionNo = objSTReturn.TransactionNo;
                    history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), objSTReturn.ReturnType));
                    history.Destination = objSTReturn.Warehouse.Code;
                }
            }
        }

        private void Get_Info_From_STImport(STStock stock, StockHistoryDTO history)
        {
            var obj = _context.STImports.AsNoTracking()
                              .Where(p => p.Id == stock.STImportDetail.STImportId)
                              .FirstOrDefault();
            if (obj != null)
            {
                history.TransactionNo = obj.TransactionNo;
                history.Transaction = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), TransactionTypeEnum.PhysicalCount));
            }

        }

        private bool IsInterBranchOrder(int? storeFrom, int? storeTo)
        {
            var storeCompanyId = stores.Where(p => p.Id == storeFrom).Select(p => p.CompanyId).FirstOrDefault();
            var orderToStoreCompanyId = stores.Where(p => p.Id == storeTo).Select(p => p.CompanyId).FirstOrDefault();

            return (storeCompanyId == orderToStoreCompanyId);
        }

    }
}

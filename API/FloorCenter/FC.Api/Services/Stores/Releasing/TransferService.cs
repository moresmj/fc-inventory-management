using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FC.Api.DTOs.Store.Releasing;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;

namespace FC.Api.Services.Stores.Releasing
{
    public class TransferService : ITransferService
    {
        private DataContext _context;

        public TransferService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return this._context;
        }

        public IEnumerable<object> GetAll(SearchTransfer search)
        {
            IQueryable<STOrder> query = _context.STOrders
                                                .Include(p => p.OrderedItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                .Include(p => p.Deliveries)
                                                    .ThenInclude(p => p.ClientDeliveries)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                .Include(p => p.Deliveries)
                                                    .ThenInclude(p => p.ShowroomDeliveries)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                .Where
                                                (p =>
                                                    p.OrderToStoreId == search.StoreId
                                                    && p.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                    && p.RequestStatus == RequestStatusEnum.Approved
                                                );

            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }

            var retList = new List<object>();
            var stores = _context.Stores.AsNoTracking().ToList();

            foreach (var x in query.OrderByDescending(p => p.Id))
            {
                var interBranch = false;

                var strCompany = stores.Where(p => p.Id == x.StoreId).Select(p => p.CompanyId).FirstOrDefault();
                var orderToStrCompany = stores.Where(p => p.Id == x.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault();

                //will identify if interbranch or intercompany
                interBranch = (strCompany == orderToStrCompany);
                if (x.Deliveries != null && x.Deliveries.Count() > 0)
                {
                    foreach (var del in x.Deliveries.OrderByDescending(p => p.Id))
                    {





                        if (del.ApprovedDeliveryDate.HasValue)
                        {
                            if (del.ClientDeliveries != null && del.ClientDeliveries.Count() > 0)
                            {
                                retList.Add(new
                                {
                                    del.Id,
                                    x.TransactionNo,
                                    x.TransactionType,
                                    TransactionTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), x.TransactionType)),
                                    x.DeliveryType,
                                    DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                                    x.PONumber,
                                    x.PODate,
                                    SINumber = x.SINumber,
                                    ORNumber = x.ORNumber,
                                    DRNumber = x.WHDRNumber,
                                    del.ClientName,
                                    del.ContactNumber,
                                    del.Address1,
                                    del.Address2,
                                    del.Address3,
                                    del.Remarks,
                                    del.ReleaseDate,
                                    IsReleased = del.ClientDeliveries
                                                    .Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                              && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                    .Count() != 0,
                                    SoldItems = del.ClientDeliveries.Select(p => new
                                    {
                                        p.Item.Code,
                                        ItemName = p.Item.Name,
                                        SizeName = p.Item.Size.Name,
                                        p.Item.Tonality,
                                        p.Quantity,
                                        p.DeliveryStatus,
                                        DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus)),
                                        p.ReleaseStatus,
                                        ReleaseStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReleaseStatusEnum), p.ReleaseStatus)),
                                        p.Id
                                    }).OrderBy(p => p.Id),
                                    del.ApprovedDeliveryDate,
                                    del.DriverName,
                                    del.PlateNumber,
                                    interBranch
                                });
                            }
                            else if (del.ShowroomDeliveries != null && del.ShowroomDeliveries.Count() > 0)
                            {
                                var IsInterBranch = false;
                                var TransferHeader = "";

                                if (x.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                                {
                                    var storeCompany = stores.Where(p => p.Id == x.StoreId).Select(p => p.CompanyId).FirstOrDefault();
                                    var orderToStoreCompany = stores.Where(p => p.Id == x.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault();

                                    // Returns true or false
                                    IsInterBranch = (storeCompany == orderToStoreCompany);

                                    // Use this header if OrderType = InterBrancOrInterCompany and DeliveryType = ShowroomPickup
                                    TransferHeader = (IsInterBranch) ? "TOR No.:" : "Branch DR No.:";
                                }

                                retList.Add(new
                                {
                                    del.Id,
                                    x.TransactionNo,
                                    x.TransactionType,
                                    TransactionTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), x.TransactionType)),
                                    x.DeliveryType,
                                    DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                                    x.PONumber,
                                    x.PODate,
                                    SINumber = x.SINumber,
                                    ORNumber = x.ORNumber,
                                    DRNumber = x.WHDRNumber,
                                    ClientName = _context.Stores.Where(p => p.Id == x.StoreId).Select(p => p.Name).FirstOrDefault(),
                                    ContactNumber = _context.Stores.Where(p => p.Id == x.StoreId).Select(p => p.ContactNumber).FirstOrDefault(),
                                    Address1 = _context.Stores.Where(p => p.Id == x.StoreId).Select(p => p.Address).FirstOrDefault(),
                                    Address2 = string.Empty,
                                    Address3 = string.Empty,
                                    del.Remarks,
                                    Remarks2 = x.Remarks,
                                    del.ReleaseDate,
                                    IsReleased = del.ShowroomDeliveries
                                                    .Where(p => p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                    .Count() != 0,
                                    SoldItems = del.ShowroomDeliveries.Select(p => new
                                    {
                                        p.Item.Code,
                                        ItemName = p.Item.Name,
                                        SizeName = p.Item.Size.Name,
                                        p.Item.Tonality,
                                        p.Quantity,
                                        p.DeliveryStatus,
                                        DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus)),
                                        p.ReleaseStatus,
                                        ReleaseStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReleaseStatusEnum), p.ReleaseStatus)),
                                        p.Id,
                                    }).OrderBy(p => p.Id),
                                    del.ApprovedDeliveryDate,
                                    del.DriverName,
                                    del.PlateNumber,
                                    TORNumber = x.ORNumber,
                                    x.WHDRNumber,
                                    IsInterBranch,
                                    TransferHeader,
                                    interBranch
                                });
                            }
                        }
                    }
                }
                else
                {
                    if (x.DeliveryType != DeliveryTypeEnum.Pickup || x.ClientSINumber == null)
                    {
                        continue;
                    }


                    var obj = new
                    {

                        x.Id,
                        x.TransactionNo,
                        x.TransactionType,
                        TransactionTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), x.TransactionType)),
                        x.DeliveryType,
                        DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                        x.PONumber,
                        x.PODate,
                        SINumber = x.SINumber,
                        ORNumber = x.ORNumber,
                        DRNumber = x.WHDRNumber,
                        x.ClientName,
                        x.ContactNumber,
                        x.Address1,
                        x.Address2,
                        x.Address3,
                        x.Remarks,
                        x.ReleaseDate,
                        interBranch,
                        x.ClientSINumber,
                        IsReleased = x.OrderedItems
                                      .Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                  && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                      .Count() != 0,
                        SoldItems = x.OrderedItems.Select(p => new
                        {
                            p.Id,
                            p.Item.Code,
                            ItemName = p.Item.Name,
                            SizeName = p.Item.Size.Name,
                            p.Item.Tonality,
                            Quantity = p.ApprovedQuantity,
                            Remarks = p.ApprovedRemarks,
                            p.DeliveryStatus,
                            DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus)),
                            p.ReleaseStatus,
                            ReleaseStatusStr = (p.ReleaseStatus == null)
                                                ? null
                                                : EnumExtensions.SplitName(Enum.GetName(typeof(ReleaseStatusEnum), p.ReleaseStatus))
                        }).OrderByDescending(p => p.Id)
                    };

                    retList.Add(obj);
                }

            }

            return retList;
        }



        public object GetAllPaged(SearchTransfer search, AppSettings appSettings)
        {
            IQueryable<STOrder> query = _context.STOrders
                                                .Include(p => p.OrderedItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                .Include(p => p.Deliveries)
                                                    .ThenInclude(p => p.ClientDeliveries)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                .Include(p => p.Deliveries)
                                                    .ThenInclude(p => p.ShowroomDeliveries)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                .Where
                                                (p =>
                                                    p.OrderToStoreId == search.StoreId
                                                    && p.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                    && p.RequestStatus == RequestStatusEnum.Approved
                                                );

            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }



            query = query.Where(p => 
                    (
                         p.Deliveries != null && p.Deliveries.Count() > 0
                         &&
                         (p.Deliveries.Any(x => x.ApprovedDeliveryDate.HasValue
                         && ((x.ClientDeliveries != null && x.ClientDeliveries.Count() > 0)
                         || (x.ShowroomDeliveries != null && x.ShowroomDeliveries.Count() > 0)))
                         )
                    )
                        || ((p.DeliveryType == DeliveryTypeEnum.Pickup || p.ClientSINumber != null))

                    ).OrderByDescending(p => p.Id);

            GetAllResponse response = null;
            if (search.ShowAll == false)
            {
                response = new GetAllResponse(query.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                //check if currentpage is greater than totalpage
                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;


                }

                query = query.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                            .Take(appSettings.RecordDisplayPerPage);


            }
            else
            {
                response = new GetAllResponse(query.Count());
            }

            var formattedList = this.FormatTransferList(query);

            response.List.AddRange(formattedList);

            return response;
        }


        private List<Object> FormatTransferList(IQueryable<STOrder> query)
        {
            var retList = new List<object>();
            var stores = _context.Stores.AsNoTracking().ToList();

            foreach (var x in query.OrderByDescending(p => p.Id))
            {
                var interBranch = false;

                var strCompany = stores.Where(p => p.Id == x.StoreId).Select(p => p.CompanyId).FirstOrDefault();
                var orderToStrCompany = stores.Where(p => p.Id == x.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault();

                //will identify if interbranch or intercompany
                interBranch = (strCompany == orderToStrCompany);
                if (x.Deliveries != null && x.Deliveries.Count() > 0)
                {
                    foreach (var del in x.Deliveries.OrderByDescending(p => p.Id))
                    {





                        if (del.ApprovedDeliveryDate.HasValue)
                        {
                            if (del.ClientDeliveries != null && del.ClientDeliveries.Count() > 0)
                            {
                                retList.Add(new
                                {
                                    del.Id,
                                    x.TransactionNo,
                                    x.TransactionType,
                                    TransactionTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), x.TransactionType)),
                                    x.DeliveryType,
                                    DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                                    x.PONumber,
                                    x.PODate,
                                    SINumber = x.SINumber,
                                    ORNumber = x.ORNumber,
                                    DRNumber = x.WHDRNumber,
                                    del.ClientName,
                                    del.ContactNumber,
                                    del.Address1,
                                    del.Address2,
                                    del.Address3,
                                    del.Remarks,
                                    del.ReleaseDate,
                                    IsReleased = del.ClientDeliveries
                                                    .Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                              && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                    .Count() != 0,
                                    SoldItems = del.ClientDeliveries.Select(p => new
                                    {
                                        p.Item.Code,
                                        ItemName = p.Item.Name,
                                        SizeName = p.Item.Size.Name,
                                        p.Item.Tonality,
                                        p.Quantity,
                                        p.DeliveryStatus,
                                        DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus)),
                                        p.ReleaseStatus,
                                        ReleaseStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReleaseStatusEnum), p.ReleaseStatus)),
                                        p.Id
                                    }).OrderBy(p => p.Id),
                                    del.ApprovedDeliveryDate,
                                    del.DriverName,
                                    del.PlateNumber,
                                    interBranch
                                });
                            }
                            else if (del.ShowroomDeliveries != null && del.ShowroomDeliveries.Count() > 0)
                            {
                                var IsInterBranch = false;
                                var TransferHeader = "";

                                if (x.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                                {
                                    var storeCompany = stores.Where(p => p.Id == x.StoreId).Select(p => p.CompanyId).FirstOrDefault();
                                    var orderToStoreCompany = stores.Where(p => p.Id == x.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault();

                                    // Returns true or false
                                    IsInterBranch = (storeCompany == orderToStoreCompany);

                                    // Use this header if OrderType = InterBrancOrInterCompany and DeliveryType = ShowroomPickup
                                    TransferHeader = (IsInterBranch) ? "TOR No.:" : "Branch DR No.:";
                                }

                                retList.Add(new
                                {
                                    del.Id,
                                    x.TransactionNo,
                                    x.TransactionType,
                                    TransactionTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), x.TransactionType)),
                                    x.DeliveryType,
                                    DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                                    x.PONumber,
                                    x.PODate,
                                    SINumber = x.SINumber,
                                    ORNumber = x.ORNumber,
                                    DRNumber = x.WHDRNumber,
                                    ClientName = _context.Stores.Where(p => p.Id == x.StoreId).Select(p => p.Name).FirstOrDefault(),
                                    ContactNumber = _context.Stores.Where(p => p.Id == x.StoreId).Select(p => p.ContactNumber).FirstOrDefault(),
                                    Address1 = _context.Stores.Where(p => p.Id == x.StoreId).Select(p => p.Address).FirstOrDefault(),
                                    Address2 = string.Empty,
                                    Address3 = string.Empty,
                                    del.Remarks,
                                    Remarks2 = x.Remarks,
                                    del.ReleaseDate,
                                    IsReleased = del.ShowroomDeliveries
                                                    .Where(p => p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                    .Count() != 0,
                                    SoldItems = del.ShowroomDeliveries.Select(p => new
                                    {
                                        p.Item.Code,
                                        ItemName = p.Item.Name,
                                        SizeName = p.Item.Size.Name,
                                        p.Item.Tonality,
                                        p.Quantity,
                                        p.DeliveryStatus,
                                        DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus)),
                                        p.ReleaseStatus,
                                        ReleaseStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReleaseStatusEnum), p.ReleaseStatus)),
                                        p.Id,
                                    }).OrderBy(p => p.Id),
                                    del.ApprovedDeliveryDate,
                                    del.DriverName,
                                    del.PlateNumber,
                                    TORNumber = x.ORNumber,
                                    x.WHDRNumber,
                                    IsInterBranch,
                                    TransferHeader,
                                    interBranch
                                });
                            }
                        }
                    }
                }
                else
                {
                    if (x.DeliveryType != DeliveryTypeEnum.Pickup || x.ClientSINumber == null)
                    {
                        continue;
                    }


                    var obj = new
                    {

                        x.Id,
                        x.TransactionNo,
                        x.TransactionType,
                        TransactionTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), x.TransactionType)),
                        x.DeliveryType,
                        DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                        x.PONumber,
                        x.PODate,
                        SINumber = x.SINumber,
                        ORNumber = x.ORNumber,
                        DRNumber = x.WHDRNumber,
                        x.ClientName,
                        x.ContactNumber,
                        x.Address1,
                        x.Address2,
                        x.Address3,
                        x.Remarks,
                        x.ReleaseDate,
                        interBranch,
                        x.ClientSINumber,
                        IsReleased = x.OrderedItems
                                      .Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                  && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                      .Count() != 0,
                        SoldItems = x.OrderedItems.Select(p => new
                        {
                            p.Id,
                            p.Item.Code,
                            ItemName = p.Item.Name,
                            SizeName = p.Item.Size.Name,
                            p.Item.Tonality,
                            Quantity = p.ApprovedQuantity,
                            Remarks = p.ApprovedRemarks,
                            p.DeliveryStatus,
                            DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus)),
                            p.ReleaseStatus,
                            ReleaseStatusStr = (p.ReleaseStatus == null)
                                                ? null
                                                : EnumExtensions.SplitName(Enum.GetName(typeof(ReleaseStatusEnum), p.ReleaseStatus))
                        }).OrderByDescending(p => p.Id)
                    };

                    retList.Add(obj);
                }

            }


            return retList;
        }




        public STOrder GetPickupOrderByIdAndStoreId(int id, int? storeId)
        {
            var record = _context.STOrders
                                 .Include(p => p.OrderedItems)
                                 .Where
                                 (p => p.OrderToStoreId == storeId
                                       && p.Id == id
                                       && p.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                       && p.DeliveryType == DeliveryTypeEnum.Pickup
                                       && p.RequestStatus == RequestStatusEnum.Approved
                                 )
                                 .FirstOrDefault();

            return record;

        }

        public void UpdatePickupOrder(ISTStockService stockService, STOrder param)
        {
            param.DateUpdated = DateTime.Now;
            param.ReleaseDate = DateTime.Now;
            foreach (var orderItem in param.OrderedItems)
            {
                //  Mark all ordered items as delivered and released
                //  set date released to date today

                orderItem.DeliveryStatus = DeliveryStatusEnum.Delivered;
                orderItem.ReleaseStatus = ReleaseStatusEnum.Released;
                orderItem.DateReleased = DateTime.Now;
                orderItem.DateUpdated = DateTime.Now;

                //  Get stock to be deducted
                var stockFromStore = _context.STStocks.Where(p => p.StoreId == param.OrderToStoreId
                                                       && p.STOrderDetailId == orderItem.Id
                                                       && p.ItemId == orderItem.ItemId)
                                            .FirstOrDefault();

                if (stockFromStore != null)
                {
                    //  Mark record as released and delivered
                    stockFromStore.ReleaseStatus = ReleaseStatusEnum.Released;
                    stockFromStore.DeliveryStatus = DeliveryStatusEnum.Delivered;
                    stockFromStore.DateUpdated = DateTime.Now;

                    stockService.UpdateSTStock(stockFromStore);

                    //  Add stock to store who requested the transfer
                    //  and deduct it later
                    var stockToStore = new STStock
                    {
                        ItemId = orderItem.ItemId,
                        OnHand = orderItem.ApprovedQuantity,
                        STOrderDetailId = orderItem.Id,
                        StoreId = param.StoreId,
                        DeliveryStatus = DeliveryStatusEnum.Delivered,
                        ReleaseStatus = ReleaseStatusEnum.Released,
                    };

                    stockService.InsertSTStock(stockToStore);
                }
                _context.STOrderDetails.Update(orderItem);


            }

            if(param.DeliveryType == DeliveryTypeEnum.Pickup)
            {
                param.OrderStatus = OrderStatusEnum.Completed;
            }
            
            _context.STOrders.Update(param);
            _context.SaveChanges();

            AddSalesRecordPickupOrder(stockService, param);
        }

        private void AddSalesRecordPickupOrder(ISTStockService stockService, STOrder param)
        {

            var sales = new STSales
            {
                STOrderId = param.Id,
                StoreId = param.StoreId,
                OrderToStoreId = param.OrderToStoreId,
                ClientName = param.ClientName,
                ContactNumber = param.ContactNumber,
                Address1 = param.Address1,
                Address2 = param.Address2,
                Address3 = param.Address3,
                ReleaseDate = DateTime.Now,
                DeliveryType = param.DeliveryType,
                SoldItems = new List<STSalesDetail>(),
                SINumber = param.ClientSINumber
            };

            sales.SalesType = new STSalesService(_context).GetSalesType(param);


            foreach(var orderDetail in param.OrderedItems)
            {
                sales.SoldItems.Add(new STSalesDetail
                {
                    ItemId = orderDetail.ItemId,
                    Quantity = orderDetail.ApprovedQuantity,
                    DeliveryStatus = DeliveryStatusEnum.Delivered
                });
            }

            if(sales.SoldItems.Count > 0)
            {
                new STSalesService(_context).InsertSalesForTransfer(sales);

                foreach(var soldItem in sales.SoldItems)
                {
                    var stockToStore = new STStock
                    {
                        ItemId = soldItem.ItemId,
                        OnHand = -soldItem.Quantity,
                        STSalesDetailId = soldItem.Id,
                        StoreId = param.StoreId,
                        DeliveryStatus = DeliveryStatusEnum.Delivered,
                        ReleaseStatus = ReleaseStatusEnum.Released
                    };

                    stockService.InsertSTStock(stockToStore);
                }
            }

        }


       

        public STDelivery GetDeliveryOrderByIdAndStoreId(int id, int? storeId)
        {
            var record = _context.STDeliveries
                                 .Include(p => p.ClientDeliveries)
                                 .Where(p => p.DeliveryFromStoreId == storeId
                                             && p.Id == id
                                             && p.ApprovedDeliveryDate.HasValue
                                        )
                                 .FirstOrDefault();

            if (record != null)
            {
                var totalNotDelivered = record.ClientDeliveries
                                              .Where(p => p.DeliveryStatus != DeliveryStatusEnum.Delivered
                                                          && p.ReleaseStatus != ReleaseStatusEnum.Released
                                              )
                                              .Count();

                if (totalNotDelivered == 0)
                {
                    return null;
                }
            }

            return record;
        }
        
        public void UpdateDeliveryOrder(ISTStockService stockService, STDelivery param)
        {

            //  Get order record
            var order = _context.STOrders
                                 .Include(p => p.OrderedItems)
                                 .Where(p => p.Id == param.STOrderId
                                             && p.OrderToStoreId == param.DeliveryFromStoreId
                                       )
                                 .FirstOrDefault();

            if (order != null)
            {
                param.Delivered = DeliveryStatusEnum.Waiting;
                param.DateUpdated = DateTime.Now;
                param.ReleaseDate = DateTime.Now;

                foreach (var delivery in param.ClientDeliveries)
                {
                    delivery.DeliveryStatus = DeliveryStatusEnum.Delivered;
                    delivery.ReleaseStatus = ReleaseStatusEnum.Released;
                    delivery.DateUpdated = DateTime.Now;

                    STStock stStock = null;

                    if(order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                    {
                        stStock = _context.STStocks
                                          .Where(p => p.STClientDeliveryId == delivery.Id
                                                      && p.ItemId == delivery.ItemId)
                                          .FirstOrDefault();
                        //adding flag to reflect on invetory history 
                        if(order.DeliveryType == DeliveryTypeEnum.Delivery)
                        {
                            stStock.IsDeliveryTransfer = true;
                        }
                    }
                    else
                    {
                        stStock = _context.STStocks
                                          .Where(p => p.STOrderDetailId == delivery.STOrderDetailId
                                                      && p.ItemId == delivery.ItemId)
                                          .FirstOrDefault();
                    }
                    
                    if (stStock != null)
                    {
                        if (stStock.ReleaseStatus != ReleaseStatusEnum.Released)
                        {
                            //  Assign release status
                            stStock.ReleaseStatus = ReleaseStatusEnum.Released;
                            stStock.DateUpdated = DateTime.Now;

                            _context.STStocks.Update(stStock);
                            _context.SaveChanges();
                        }
                    }

                }

                _context.STDeliveries.Update(param);
                _context.SaveChanges();

                MarkOrderedItemsAsDeliveredAndReleasedDeliveryOrder(order);

            }
            
        }

        private void MarkOrderedItemsAsDeliveredAndReleasedDeliveryOrder(STOrder order)
        {
            foreach (var detail in order.OrderedItems)
            {
                var totalDeliveredQty = Convert.ToInt32(
                                            _context.STClientDeliveries
                                                .Where(p => p.STOrderDetailId == detail.Id
                                                           && p.ItemId == detail.ItemId
                                                           && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                           && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                .Sum(p => p.Quantity)
                                        );

                //  Check if approved quantity and total delivered qty are equal
                if (detail.ApprovedQuantity == totalDeliveredQty)
                {
                    //  Mark record as delivered and released
                    detail.DeliveryStatus = DeliveryStatusEnum.Delivered;
                    detail.ReleaseStatus = ReleaseStatusEnum.Released;
                    detail.DateReleased = DateTime.Now;

                    _context.STOrderDetails.Update(detail);
                    _context.SaveChanges();

                    var stStock = _context.STStocks
                                    .Where(p => p.STOrderDetailId == detail.Id
                                                && p.ItemId == detail.ItemId
                                                && p.StoreId == order.OrderToStoreId).FirstOrDefault();
                    if (stStock != null)
                    {
                        stStock.DeliveryStatus = DeliveryStatusEnum.Delivered;
                        stStock.DateUpdated = DateTime.Now;

                        _context.STStocks.Update(stStock);
                        _context.SaveChanges();
                    }

                }
            }

            if (order.OrderedItems != null && order.OrderedItems.Count > 0)
            {
                //  Check if all ordered items are delivered and released
                if (order.OrderedItems.Where(p => p.DeliveryStatus != DeliveryStatusEnum.Delivered
                                                  && p.ReleaseStatus != ReleaseStatusEnum.Released)
                                        .Count() == 0)
                {
                    order.ReleaseDate = DateTime.Now;
                    order.DateUpdated = DateTime.Now;

                    _context.STOrders.Update(order);
                    _context.SaveChanges();
                }

            }
        }



        public STDelivery GetShowroomPickupOrderByIdAndStoreId(int id, int? storeId)
        {
            var record = _context.STDeliveries
                                 .Include(p => p.ShowroomDeliveries)
                                 .Where(p => p.DeliveryFromStoreId == storeId
                                             && p.Id == id
                                             && p.ApprovedDeliveryDate.HasValue
                                        )
                                 .FirstOrDefault();

            if (record != null)
            {
                var totalNotDelivered = record.ShowroomDeliveries
                                              .Where(p => p.DeliveryStatus != DeliveryStatusEnum.Delivered
                                                          && p.ReleaseStatus != ReleaseStatusEnum.Released
                                              )
                                              .Count();

                if (totalNotDelivered == 0)
                {
                    return null;
                }
            }

            return record;
        }

        public string UpdateShowroomPickupOrder(ISTStockService stockService, STDelivery param)
        {
            //  Get order record
            var order = _context.STOrders
                                 .Where(p => p.Id == param.STOrderId)
                                 .FirstOrDefault();
            if (order != null)
            {

                param.DateUpdated = DateTime.Now;
                param.ReleaseDate = DateTime.Now;

                foreach (var delivery in param.ShowroomDeliveries)
                {
                    delivery.DateUpdated = DateTime.Now;
                    delivery.ReleaseStatus = ReleaseStatusEnum.Released;

                    STStock stStock = null;

                    if(order.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                    {
                        stStock = _context.STStocks
                                          .Where(p => p.STShowroomDeliveryId == delivery.Id
                                                      && p.ItemId == delivery.ItemId
                                                      && p.ReleaseStatus != ReleaseStatusEnum.Released)
                                          .FirstOrDefault();

                        //adding flag to reflect on invetory history 
                        if (order.DeliveryType == DeliveryTypeEnum.ShowroomPickup)
                        {
                            stStock.IsDeliveryTransfer = true;
                        }
                    }
                    else
                    {
                        stStock = _context.STStocks
                                          .Where(p => p.STOrderDetailId == delivery.STOrderDetailId
                                                      && p.ItemId == delivery.ItemId
                                                      && p.ReleaseStatus != ReleaseStatusEnum.Released)
                                          .FirstOrDefault();
                    }

                    if (stStock != null)
                    {
                        //  Assign release status
                        stStock.ReleaseStatus = ReleaseStatusEnum.Released;
                        stStock.DateUpdated = DateTime.Now;

                        _context.STStocks.Update(stStock);
                        _context.SaveChanges();
                    }

                    var orderDetail = _context.STOrderDetails
                                              .Where(p => p.Id == delivery.STOrderDetailId
                                                          && p.ItemId == delivery.ItemId
                                                          && p.ReleaseStatus != ReleaseStatusEnum.Released)
                                              .FirstOrDefault();

                    if (orderDetail != null)
                    {
                        var deliveryTotal = _context.STShowroomDeliveries.Where(p => p.STOrderDetailId == orderDetail.Id
                                                                                        && p.ReleaseStatus == ReleaseStatusEnum.Released
                                                                                        && p.ItemId == delivery.ItemId).Sum(p => p.Quantity);

                        // Will only be set to released when all deliveries quantity tallied with the approved quantity
                        if (deliveryTotal == orderDetail.ApprovedQuantity)
                        {
                            //  Assign release status
                            orderDetail.ReleaseStatus = ReleaseStatusEnum.Released;
                            orderDetail.DateReleased = DateTime.Now;
                        }
                        
                        orderDetail.DateUpdated = DateTime.Now;

                        _context.STOrderDetails.Update(orderDetail);
                        _context.SaveChanges();
                    }

                }

                _context.STDeliveries.Update(param);
                _context.SaveChanges();

                return order.TransactionNo;
            }

            return null;
        }

    }
}

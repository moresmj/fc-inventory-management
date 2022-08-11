using AutoMapper;
using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Services.Stores
{
    public class STSalesService : ISTSalesService
    {

        private DataContext _context;

        public STSalesService(DataContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Insert sales
        /// </summary>
        /// <param name="sales">STSales</param>
        public void InsertSales(STSales sales, bool deductStock = false, bool isSamedaySales = false)
        {
            var totalRecordCount = Convert.ToInt32(this._context.STSales.Count() + 1).ToString();
            sales.TransactionNo = string.Format("ST{0}", totalRecordCount.PadLeft(6, '0'));
            sales.DateCreated = DateTime.Now;
            sales.SalesDate = DateTime.Now;

            //added for optimization and better filtering
            sales.OrderStatus = OrderStatusEnum.Incomplete;

            foreach (var detail in sales.SoldItems)
            {
                detail.DateCreated = DateTime.Now;
                if (!detail.DeliveryStatus.HasValue)
                {
                    detail.DeliveryStatus = DeliveryStatusEnum.Pending;
                }
            }

            _context.STSales.Add(sales);
            _context.SaveChanges();

            //added for ticket #252 once salesorder is created it will update stocks
            var stStocks =  _context.STStocks;

            foreach(var items in sales.SoldItems)
            {
                if(stStocks.Where(p => p.ItemId == items.ItemId && p.StoreId == sales.StoreId).Count() > 0)
                {
                    var stock = stStocks.Where(p => p.ItemId == items.ItemId && p.StoreId == sales.StoreId)?.Last();

                    if(stock != null)
                    {
                        stock.ChangeDate = DateTime.Now;
           
                        stStocks.Update(stock);
                        _context.SaveChanges();
                    
                    
                    }

                }

            }

            if (deductStock)
            {
                // For Release status [Sales Order - Pickup] is moved upon creating delivery pick up.
                //added same day sales param to seperate same day sales
                deductStock = (sales.DeliveryType == DeliveryTypeEnum.Pickup && isSamedaySales == false) ? false : true;
            }

            if (deductStock)
            {
                var stockService = new STStockService(_context);
                foreach (var detail in sales.SoldItems)
                {
                    var stSTock = new STStock
                    {
                        ItemId = detail.ItemId,
                        OnHand = -detail.Quantity,
                        STSalesDetailId = detail.Id,
                        StoreId = sales.StoreId
                    };

                    if (
                        sales.SalesType == SalesTypeEnum.SalesOrder
                        || sales.SalesType == SalesTypeEnum.Interbranch
                       )
                    {
                        stSTock.DeliveryStatus = DeliveryStatusEnum.Waiting;
                        stSTock.ReleaseStatus = ReleaseStatusEnum.Waiting;
                    }
                    else
                    {
                        stSTock.DeliveryStatus = DeliveryStatusEnum.Delivered;
                        stSTock.ReleaseStatus = ReleaseStatusEnum.Released;
                    }

                    stockService.InsertSTStock(stSTock);
                }
            }

        }

        internal object GetSalesForReleasingById(object id, object storeId)
        {
            throw new NotImplementedException();
        }

        public void InsertSalesForTransfer(STSales sales)
        {
            var totalRecordCount = Convert.ToInt32(this._context.STSales.Count() + 1).ToString();
            sales.TransactionNo = string.Format("ST{0}", totalRecordCount.PadLeft(6, '0'));
            sales.DateCreated = DateTime.Now;

            foreach (var detail in sales.SoldItems)
            {
                detail.DateCreated = DateTime.Now;
            }

            _context.STSales.Add(sales);
            _context.SaveChanges();
        }

        /// <summary>
        /// Get all sales
        /// </summary>
        /// <param name="search">Search parameters</param>
        /// <returns>STSales</returns>
        public IEnumerable<STSales> GetAllSalesForReleasing(SearchSalesForReleasing search)
        {
            IQueryable<STSales> query = _context.STSales
                                            .Where(
                                                    p =>
                                                        p.StoreId == search.StoreId
                                                        &&
                                                        (
                                                            p.SalesType == SalesTypeEnum.ClientOrder
                                                            && p.DeliveryType == DeliveryTypeEnum.ShowroomPickup
                                                        )
                                                        ||
                                                        (
                                                            p.SalesType == SalesTypeEnum.SalesOrder
                                                            ||
                                                            p.SalesType == SalesTypeEnum.Releasing
                                                        )
                                                  )
                                                .Include(p => p.Order)
                                                .Include(p => p.SoldItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size);

            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.Order.PONumber.ToLower() == search.PONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.SINumber))
            {
                query = query.Where(p => p.SINumber.ToLower() == search.SINumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ORNumber))
            {
                query = query.Where(p => p.ORNumber.ToLower() == search.ORNumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                query = query.Where(p => p.DRNumber.ToLower() == search.DRNumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ClientName))
            {
                query = query.Where(p => p.ClientName.ToLower() == search.ClientName.ToLower());
            }



            //query = query.Where(p => p.Order.OrderType == Core.Domain.Common.OrderTypeEnum.ClientOrder && p.Order.DeliveryType == Core.Domain.Common.DeliveryTypeEnum.ShowroomPickup);

            return query;
        }

        public IEnumerable<object> GetAllSalesForReleasing(SearchSalesForReleasing search, IMapper mapper)
        {
            IQueryable<STSales> list = _context.STSales
                                            .Where(
                                                    p =>
                                                        p.StoreId == search.StoreId
                                                        &&
                                                        (
                                                            p.SalesType == SalesTypeEnum.ClientOrder
                                                            && p.DeliveryType == DeliveryTypeEnum.ShowroomPickup
                                                        )
                                                        ||
                                                        (
                                                            p.SalesType == SalesTypeEnum.Interbranch
                                                            && p.OrderToStoreId == search.StoreId
                                                        )
                                                        ||
                                                        (
                                                            p.SalesType == SalesTypeEnum.SalesOrder
                                                            ||
                                                            p.SalesType == SalesTypeEnum.Releasing
                                                        )
                                                  );

            var sales = new List<STSalesDTO>();

            foreach (var sale in list)
            {
                var mappedSale = mapper.Map<STSalesDTO>(sale);

                var order = _context.STOrders.Where(p => p.Id == sale.STOrderId).FirstOrDefault();
                if (order != null)
                {
                    var mappedOrder = mapper.Map<STOrderDTO>(order);
                    mappedSale.Order = mappedOrder;
                }

                var deliveries = _context.STDeliveries
                                         .Include(p => p.ClientDeliveries)
                                            .ThenInclude(p => p.Item)
                                                .ThenInclude(p => p.Size)
                                         .Where(p => p.STSalesId == mappedSale.Id);
                if (deliveries != null && deliveries.Count() > 0)
                {
                    var mappedDeliveries = mapper.Map<List<STDeliveryDTO>>(deliveries);
                    mappedSale.Deliveries = mappedDeliveries;
                }

                sales.Add(mappedSale);
            }

            var query = sales.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.Order.PONumber.ToLower() == search.PONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.SINumber))
            {
                query = query.Where(p => p.SINumber.ToLower() == search.SINumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ORNumber))
            {
                query = query.Where(p => p.ORNumber.ToLower() == search.ORNumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                query = query.Where(p => p.DRNumber.ToLower() == search.DRNumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ClientName))
            {
                query = query.Where(p => p.ClientName.ToLower() == search.ClientName.ToLower());
            }


            var retList = new List<object>();
            foreach (var x in query)
            {
                if (x.Deliveries != null && x.Deliveries.Count() > 0)
                {
                    foreach (var del in x.Deliveries)
                    {
                        if (del.ClientDeliveries != null && del.ClientDeliveries.Count() > 0)
                        {
                            if (del.ApprovedDeliveryDate.HasValue)
                            {
                                retList.Add(new
                                {
                                    del.Id,
                                    x.TransactionNo,
                                    del.SINumber,
                                    del.ORNumber,
                                    del.DRNumber,
                                    x.ReleaseDate,
                                    del.ClientName,
                                    del.Address1,
                                    del.Address2,
                                    del.Address3,
                                    x.SalesType,
                                    SalesTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(SalesTypeEnum), x.SalesType)),
                                    x.SalesAgent,
                                    del.Remarks,
                                    del.ContactNumber,
                                    x.DeliveryType,
                                    DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
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
                                    }),
                                    del.ApprovedDeliveryDate,
                                    del.DriverName,
                                    del.PlateNumber
                                });
                            }
                        }
                    }
                }
                else
                {
                    var obj = new
                    {
                        x.Id,
                        x.TransactionNo,
                        x.SINumber,
                        x.ORNumber,
                        x.DRNumber,
                        x.ReleaseDate,
                        x.ClientName,
                        x.Address1,
                        x.Address2,
                        x.Address3,
                        x.SalesType,
                        SalesTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(SalesTypeEnum), x.SalesType)),
                        x.SalesAgent,
                        x.Remarks,
                        x.ContactNumber,
                        x.DeliveryType,
                        DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                        SoldItems = ((x.SalesType == SalesTypeEnum.SalesOrder || x.SalesType == SalesTypeEnum.Interbranch) && x.DeliveryType == DeliveryTypeEnum.Delivery)
                                                ? null
                                                : _context.STSalesDetails.Where(p => p.STSalesId == x.Id).Select(p => new
                                                {
                                                    p.Item.Code,
                                                    ItemName = p.Item.Name,
                                                    SizeName = p.Item.Size.Name,
                                                    p.Item.Tonality,
                                                    p.Quantity
                                                })

                    };

                    if (obj.SoldItems != null)
                    {
                        retList.Add(obj);
                    }
                }
            }



            return retList;

        }

        internal SalesTypeEnum? GetSalesType(STOrder param)
        {
            var storeRequestFrom = _context.Stores.Where(p => p.Id == param.StoreId).FirstOrDefault();
            var storeRequestTo = _context.Stores.Where(p => p.Id == param.OrderToStoreId).FirstOrDefault();

            if (storeRequestFrom != null && storeRequestTo != null)
            {
                if (storeRequestFrom.CompanyId == storeRequestTo.CompanyId)
                {
                    return SalesTypeEnum.Interbranch;
                }

                return SalesTypeEnum.Intercompany;
            }

            return null;
        }

        public STSales GetSalesForReleasingById(int? id, int? storeId)
        {

            var record = _context.STSales
                            .Where(p => p.Id == id
                                        && (
                                                p.StoreId == storeId
                                                ||
                                                p.OrderToStoreId == storeId
                                            )
                                        && p.DeliveryType != DeliveryTypeEnum.Delivery)
                                .Include(p => p.Order)
                                .Include(p => p.SoldItems)
                                    .ThenInclude(p => p.Item)
                                        .ThenInclude(p => p.Size)
                             .FirstOrDefault();

            return record;
        }

        public void UpdateSalesForReleasing(ISTStockService stockService, STSales param)
        {
            var obj = this.GetSalesForReleasingById(param.Id, param.StoreId);
            if (obj != null)
            {
                obj.SINumber = param.SINumber;
                obj.ORNumber = param.ORNumber;
                obj.DRNumber = param.DRNumber;
                obj.Remarks = param.Remarks;
                obj.ClientName = param.ClientName;
                obj.Address1 = param.Address1;
                obj.Address2 = param.Address2;
                obj.Address3 = param.Address3;
                obj.ContactNumber = param.ContactNumber;
                obj.DateUpdated = DateTime.Now;
                obj.SalesAgent = param.SalesAgent;

                //  Check if sales is client order and if its showroom pickup or if its 
                //  for pickup
                if ((obj.SalesType == SalesTypeEnum.ClientOrder && obj.DeliveryType == DeliveryTypeEnum.ShowroomPickup)
                    || (obj.DeliveryType == DeliveryTypeEnum.Pickup))
                {
                    //  Set release date to date today
                    obj.ReleaseDate = DateTime.Now;
                }

                _context.STSales.Update(obj);
                _context.SaveChanges();

                foreach(var soldItem in obj.SoldItems)
                {
                    if(obj.SalesType == SalesTypeEnum.Interbranch && obj.DeliveryType == DeliveryTypeEnum.Pickup)
                    {
                        soldItem.DeliveryStatus = DeliveryStatusEnum.Delivered;
                        soldItem.DateUpdated = DateTime.Now;

                        _context.STSalesDetails.Update(soldItem);
                        _context.SaveChanges();
                    }


                    var stStock = _context.STStocks.Where(p => p.StoreId == obj.StoreId
                                                               && p.STSalesDetailId == soldItem.Id
                                                               && p.ItemId == soldItem.ItemId)
                                                    .FirstOrDefault();

                    if(stStock != null)
                    {
                        UpdateStoreStocks(stStock);
                    }
                    else
                    {
                        stStock = _context.STStocks.Where(p => p.StoreId == obj.OrderToStoreId
                                                               && p.STSalesDetailId == soldItem.Id
                                                               && p.ItemId == soldItem.ItemId)
                                                    .FirstOrDefault();

                        if(stStock != null)
                        {
                            UpdateStoreStocks(stStock);
                        }
                    }

                }
            }
        }

        private void UpdateStoreStocks(STStock stStock)
        {
            stStock.ReleaseStatus = ReleaseStatusEnum.Released;
            stStock.DeliveryStatus = DeliveryStatusEnum.Delivered;
            stStock.DateUpdated = DateTime.Now;

            _context.STStocks.Update(stStock);
            _context.SaveChanges();
        }

        public DataContext DataContext()
        {
            return this._context;
        }

        /// <summary>
        /// Get all sales orders
        /// </summary>
        /// <param name="search">Search parameters</param>
        /// <returns>STSales</returns>
        public object GetAllSalesOrders(SearchSalesOrder search)
        {
            IQueryable<STSales> query = _context.STSales
                                                .Where(p => 
                                                            p.StoreId == search.StoreId
                                                            //&& p.DeliveryType == DeliveryTypeEnum.Delivery should be displayed on Sales Orders even it is pickup mode
                                                            && p.SalesType == SalesTypeEnum.SalesOrder
                                                      ).OrderByDescending(p => p.Id)
                                                      .Include(p => p.Deliveries)
                                                        .ThenInclude(p => p.ClientDeliveries)
                                                      .Include(p => p.SoldItems);

            if (!string.IsNullOrWhiteSpace(search.SINumber))
            {
                query = query.Where(p => p.SINumber.ToLower() == search.SINumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ORNumber))
            {
                query = query.Where(p => p.ORNumber.ToLower() == search.ORNumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                query = query.Where(p => p.DRNumber.ToLower() == search.DRNumber.ToLower());
            }

            if (search.SalesDateFrom.HasValue)
            {
                query = query.Where(p => search.SalesDateFrom.Value <= p.SalesDate);
            }

            if (search.SalesDateTo.HasValue)
            {
                query = query.Where(p => search.SalesDateTo.Value >= p.SalesDate);
            }


            if (!string.IsNullOrWhiteSpace(search.ClientName))
            {
                query = query.Where(p => p.ClientName.ToLower() == search.ClientName.ToLower());
            }
       

          

            var records = from x in query
                          select new
                          {
                              x.Id,
                              x.TransactionNo,
                              x.SINumber,
                              x.ORNumber,
                              x.DRNumber,
                              x.DeliveryType,
                              DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                              x.SalesDate,
                              x.ClientName,
                              x.Deliveries,
                              OrderStatus = (x.Deliveries.Count > 0) ? 
                                                //check if the delivered quantity match the quantity of items sold
                                               (x.Deliveries.Where(y => y.Delivered == DeliveryStatusEnum.Delivered).Select(p => p.ClientDeliveries.Sum(q => q.Quantity)).Sum() == x.SoldItems.Sum(p => p.Quantity)) ?
                                                OrderStatusEnum.Completed : OrderStatusEnum.Incomplete
                                            : OrderStatusEnum.Incomplete,

                              OrderStatusStr = (x.Deliveries.Count > 0) ?
                                                        //check if the delivered quantity match the quantity of items sold
                                                        (x.Deliveries.Where(y => y.Delivered == DeliveryStatusEnum.Delivered).Select(p => p.ClientDeliveries.Sum(q => q.Quantity)).Sum() == x.SoldItems.Sum(p => p.Quantity)) ?
                                                        EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), OrderStatusEnum.Completed)) :
                                                        EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), OrderStatusEnum.Incomplete))
                                                  : EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), OrderStatusEnum.Incomplete)),
                              TestStatus = x.OrderStatus,
                              TestStatusStr = x.OrderStatus.HasValue ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), x.OrderStatus)) : "",
                              SoldItems = x.SoldItems.Select(p => new
                              {
                                  p.STSalesId,
                                  p.Id,
                                  itemId = p.Item.Id,
                                  p.Quantity,
                                  p.DeliveryStatus,
                                  DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus))
                              }).OrderBy(p => p.Id)
                          };
            var filteredRecords = new List<object>();


            if (search.OrderStatus != null)
            {
                foreach(var rec in records)
                {
                    if(search.OrderStatus.Contains(rec.OrderStatus))
                    {
                        filteredRecords.Add(rec);
                    }
                }
                return filteredRecords;
            }



            return records;

        }



        public object GetAllSalesOrdersPaged(SearchSalesOrder search, AppSettings appSettings)
        {
            IQueryable<STSales> query = _context.STSales
                                                .Where(p =>
                                                            p.StoreId == search.StoreId
                                                            //&& p.DeliveryType == DeliveryTypeEnum.Delivery should be displayed on Sales Orders even it is pickup mode
                                                            && p.SalesType == SalesTypeEnum.SalesOrder
                                                      ).OrderByDescending(p => p.Id)
                                                      .Include(p => p.Deliveries)
                                                        .ThenInclude(p => p.ClientDeliveries)
                                                      .Include(p => p.SoldItems);

            if (!string.IsNullOrWhiteSpace(search.SINumber))
            {
                query = query.Where(p => p.SINumber.ToLower() == search.SINumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ORNumber))
            {
                query = query.Where(p => p.ORNumber.ToLower() == search.ORNumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                query = query.Where(p => p.DRNumber.ToLower() == search.DRNumber.ToLower());
            }

            if (search.SalesDateFrom.HasValue)
            {
                query = query.Where(p => search.SalesDateFrom.Value <= p.SalesDate);
            }

            if (search.SalesDateTo.HasValue)
            {
                query = query.Where(p => search.SalesDateTo.Value >= p.SalesDate);
            }


            if (!string.IsNullOrWhiteSpace(search.ClientName))
            {
                query = query.Where(p => p.ClientName.ToLower() == search.ClientName.ToLower());
            }

            if (search.OrderStatus != null)
            {
                query = query.Where(p => search.OrderStatus.Contains(p.OrderStatus));
            }


            var records = from x in query
                          select new
                          {
                              x.Id,
                              x.TransactionNo,
                              x.SINumber,
                              x.ORNumber,
                              x.DRNumber,
                              x.DeliveryType,
                              DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                              x.SalesDate,
                              x.ClientName,
                              x.Deliveries,
                              OrderStatus = (x.Deliveries.Count > 0) ?
                                               //check if the delivered quantity match the quantity of items sold
                                               (x.Deliveries.Where(y => y.Delivered == DeliveryStatusEnum.Delivered).Select(p => p.ClientDeliveries.Sum(q => q.Quantity)).Sum() == x.SoldItems.Sum(p => p.Quantity)) ?
                                                OrderStatusEnum.Completed : OrderStatusEnum.Incomplete
                                            : OrderStatusEnum.Incomplete,

                              OrderStatusStr = (x.Deliveries.Count > 0) ?
                                                        //check if the delivered quantity match the quantity of items sold
                                                        (x.Deliveries.Where(y => y.Delivered == DeliveryStatusEnum.Delivered).Select(p => p.ClientDeliveries.Sum(q => q.Quantity)).Sum() == x.SoldItems.Sum(p => p.Quantity)) ?
                                                        EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), OrderStatusEnum.Completed)) :
                                                        EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), OrderStatusEnum.Incomplete))
                                                  : EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), OrderStatusEnum.Incomplete)),
                              SoldItems = x.SoldItems.Select(p => new
                              {
                                  p.STSalesId,
                                  p.Id,
                                  itemId = p.Item.Id,
                                  p.Quantity,
                                  p.DeliveryStatus,
                                  DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus))
                              }).OrderBy(p => p.Id)
                          };


            GetAllResponse response = null;
            if (search.ShowAll == false)
            {
                response = new GetAllResponse(records.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                //check if currentpage is greater than totalpage
                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;


                }

                records = records.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                            .Take(appSettings.RecordDisplayPerPage);



            }
            else
            {
                response = new GetAllResponse(records.Count());
            }



            response.List.AddRange(records);


            return response;

        }




    }
}

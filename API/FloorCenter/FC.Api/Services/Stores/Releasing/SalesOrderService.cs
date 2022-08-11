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
    public class SalesOrderService : ISalesOrderService
    {
        private DataContext _context;

        public SalesOrderService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return this._context;
        }

        public IEnumerable<object> GetAll(SearchSalesOrder search)
        {
            IQueryable<STSales> query = _context.STSales
                                                .Include(p => p.SoldItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                .Include(p => p.Deliveries)
                                                    .ThenInclude(p => p.ClientDeliveries)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                .Where
                                                (p =>
                                                    p.StoreId == search.StoreId
                                                    && p.SalesType == SalesTypeEnum.SalesOrder
                                                    && (p.DeliveryType == DeliveryTypeEnum.Pickup || p.DeliveryType == DeliveryTypeEnum.Delivery)
                                                    && p.Deliveries.Count() > 0
                                                );


            if (!string.IsNullOrWhiteSpace(search.SINumber))
            {
                query = query.Where(p => p.SINumber.ToLower() == search.SINumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ORNumber))
            {
                query = query.Where(p => p.ORNumber.ToLower() == search.ORNumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ClientName))
            {
                query = query.Where(p => p.ClientName.ToLower() == search.ClientName.ToLower());
            }


            var retList = new List<object>();
            foreach (var x in query.OrderByDescending(p => p.Id))
            {
                if (x.Deliveries != null && x.Deliveries.Count() > 0)
                {
                    foreach (var del in x.Deliveries.OrderByDescending(p => p.Id))
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
                                    del.ReleaseDate,
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
                                        p.Id,
                                        p.Item.Code,
                                        ItemName = p.Item.Name,
                                        SizeName = p.Item.Size.Name,
                                        p.Item.Tonality,
                                        p.Quantity,
                                        p.DeliveryStatus,
                                        DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus)),
                                        p.ReleaseStatus,
                                        ReleaseStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReleaseStatusEnum), p.ReleaseStatus)),
                                    }).OrderBy(p => p.Id),
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
                    if (x.DeliveryType == DeliveryTypeEnum.Pickup)
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
                            SoldItems = x.SoldItems.Select(p => new
                            {
                                p.Item.Code,
                                ItemName = p.Item.Name,
                                SizeName = p.Item.Size.Name,
                                p.Item.Tonality,
                                p.Quantity
                            }),
                            ApprovedDeliveryDate = String.Empty

                        };

                        retList.Add(obj);
                    }
                }

            }

            return retList;
        }

        public STSales GetSalesRecordByIdAndStoreId(int? id, int? storeId)
        {
            var record = _context.STSales
                                .Include(p => p.Order)
                                .Include(p => p.SoldItems)
                                    .ThenInclude(p => p.Item)
                                        .ThenInclude(p => p.Size)
                                 .Where
                                 (p => p.StoreId == storeId
                                       && p.Id == id
                                       && p.SalesType == SalesTypeEnum.SalesOrder
                                       && p.DeliveryType == DeliveryTypeEnum.Pickup
                                 )
                                .FirstOrDefault();

            return record;
        }



        public object GetAllPaged(SearchSalesOrder search, AppSettings appSettings)
        {
            IQueryable<STSales> query = _context.STSales
                                                .Include(p => p.SoldItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                .Include(p => p.Deliveries)
                                                    .ThenInclude(p => p.ClientDeliveries)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                .Where
                                                (p =>
                                                    p.StoreId == search.StoreId
                                                    && p.SalesType == SalesTypeEnum.SalesOrder
                                                    && (p.DeliveryType == DeliveryTypeEnum.Pickup || p.DeliveryType == DeliveryTypeEnum.Delivery)
                                                    && p.Deliveries.Count() > 0
                                                );


            if (!string.IsNullOrWhiteSpace(search.SINumber))
            {
                query = query.Where(p => p.SINumber.ToLower() == search.SINumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ORNumber))
            {
                query = query.Where(p => p.ORNumber.ToLower() == search.ORNumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ClientName))
            {
                query = query.Where(p => p.ClientName.ToLower() == search.ClientName.ToLower());
            }

           


            var deliveryItems = query.Where(p =>
                                     ((p.Deliveries != null
                                     && p.Deliveries.Count() > 0) && p.Deliveries.Any(c => (c.ClientDeliveries != null
                                     && c.ClientDeliveries.Count() > 0
                                     && c.ApprovedDeliveryDate.HasValue)))
                                   
                                     )
                                     .SelectMany(sales => sales.Deliveries.Select(del => new
                                    {
                                        del.Id,
                                        sales.TransactionNo,
                                        del.SINumber,
                                        del.ORNumber,
                                        del.DRNumber,
                                        del.ReleaseDate,
                                        del.ClientName,
                                        del.Address1,
                                        del.Address2,
                                        del.Address3,
                                        sales.SalesType,
                                        SalesTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(SalesTypeEnum), sales.SalesType)),
                                        sales.SalesAgent,
                                        del.Remarks,
                                        sales.ContactNumber,
                                        sales.DeliveryType,
                                        DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), sales.DeliveryType)),
                                        SoldItems = del.ClientDeliveries.Select(clientDel => new
                                        {
                                            clientDel.Id,
                                            clientDel.Item.Code,
                                            ItemName = clientDel.Item.Name,
                                            SizeName = clientDel.Item.Size.Name,
                                            clientDel.Item.Tonality,
                                            clientDel.Quantity,
                                            clientDel.DeliveryStatus,
                                            DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), clientDel.DeliveryStatus)),
                                            clientDel.ReleaseStatus,
                                            ReleaseStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReleaseStatusEnum), clientDel.ReleaseStatus)),
                                        }),
                                        del.ApprovedDeliveryDate,
                                        del.DriverName,
                                        del.PlateNumber

                                    }
                                    ));


            deliveryItems = deliveryItems.Where(p => p.ApprovedDeliveryDate.HasValue).OrderByDescending(p => p.Id);

            if (search.ReleaseStatus != null && search.ReleaseStatus.Count() == 1)
            {
                if (search.ReleaseStatus[0] == ReleaseStatusEnum.Waiting)
                {
                    deliveryItems = deliveryItems.Where(p => p.ReleaseDate == null);

                }
                else if (search.ReleaseStatus[0] == ReleaseStatusEnum.Released)
                {
                    deliveryItems = deliveryItems.Where(p => p.ReleaseDate.HasValue);
                }
            }



            GetAllResponse response = null;
            if (search.ShowAll == false)
            {
                response = new GetAllResponse(deliveryItems.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;
                }

                deliveryItems = deliveryItems
                                                .Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                                    .Take(appSettings.RecordDisplayPerPage);
            }
            else
            {
                response = new GetAllResponse(deliveryItems.Count());
            }

         



            response.List.AddRange(deliveryItems);

            return response;
        }

        public STDelivery GetDeliveryRecordByIdAndStoreId(int? id, int? storeId)
        {
            var record = _context.STDeliveries
                                 .Include(p => p.ClientDeliveries)
                                 .Where(p => p.StoreId == storeId
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

        public string UpdateDelivery(ISTStockService stockService, STDelivery param)
        {
            param.Delivered = DeliveryStatusEnum.Waiting;
            param.DateUpdated = DateTime.Now;
            param.ReleaseDate = DateTime.Now;


            DeliveryTypeEnum? deliveryType = _context.STSales.AsNoTracking().Where(p => p.Id == param.STSalesId).Select(p => p.DeliveryType).FirstOrDefault();
            List<STStock> stStocks = _context.STStocks.AsNoTracking().ToList();

            foreach (var delivery in param.ClientDeliveries)
            {
                delivery.DateUpdated = DateTime.Now;
                delivery.DeliveryStatus = DeliveryStatusEnum.Delivered;
                delivery.ReleaseStatus = ReleaseStatusEnum.Released;

                if (deliveryType == DeliveryTypeEnum.Pickup)
                {
                    var stStock = _context.STStocks.Where(p => p.StoreId == param.StoreId
                                               && p.STClientDeliveryId == delivery.Id
                                               && p.ItemId == delivery.ItemId)
                                    .FirstOrDefault();

                    stStock.DeliveryStatus = DeliveryStatusEnum.Delivered;
                    stStock.ReleaseStatus = ReleaseStatusEnum.Released;


                    stockService.UpdateSTStock(stStock);
                }
                else
                {
                    var stStock = new STStock
                    {
                        StoreId = param.StoreId,
                        STClientDeliveryId = delivery.Id,
                        DeliveryStatus = DeliveryStatusEnum.Delivered,
                        ReleaseStatus = ReleaseStatusEnum.Released,
                        ItemId = delivery.ItemId,
                        OnHand = -delivery.Quantity
                    };

                    stockService.InsertSTStock(stStock);
                }
            }
        
            var sales = _context.STSales
                                .Include(p => p.SoldItems)
                                .Where(p => p.Id == param.STSalesId 
                                            && p.StoreId == param.StoreId
                                      )
                                .FirstOrDefault();

            if(sales.DeliveryType == DeliveryTypeEnum.Pickup)
            {
                param.Delivered = DeliveryStatusEnum.Delivered;
            }

            _context.STDeliveries.Update(param);
            _context.SaveChanges();
            if (sales != null)
            {
                foreach (var detail in sales.SoldItems)
                {
                    var totalDeliveredQty = Convert.ToInt32(
                                                _context.STClientDeliveries
                                                    .Where(p => p.STSalesDetailId == detail.Id
                                                               && p.ItemId == detail.ItemId
                                                               && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                               && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                    .Sum(p => p.Quantity)
                                            );

                    if (detail.Quantity == totalDeliveredQty)
                    {
                        detail.DeliveryStatus = DeliveryStatusEnum.Delivered;
                        detail.DateUpdated = DateTime.Now;

                        sales.ReleaseDate = DateTime.Now;


                        _context.STSales.Update(sales);
                        _context.SaveChanges();
                    }
                }


                if (sales.DeliveryType == DeliveryTypeEnum.Pickup)
                {
                   

                    var stDel = _context.STDeliveries
                                        .Include(p => p.ClientDeliveries)
                                        .Where(p => p.Delivered == DeliveryStatusEnum.Delivered &&
                                                    p.STSalesId == sales.Id)
                                                    .SelectMany(p => p.ClientDeliveries);
         

                    if (stDel != null)
                    {
                        int? delQuantity = 0;
                        //foreach (var cDel in stDel)
                        //{
                        //    delQuantity += Convert.ToInt32(
                        //                                    cDel.ClientDeliveries
                        //                                    .Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered
                        //                                             && p.ReleaseStatus == ReleaseStatusEnum.Released)
                        //                                    .Sum(p => p.Quantity)
                        //                                    );
                        //}

                        delQuantity = Convert.ToInt32(
                                                            stDel
                                                            .Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                                     && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                            .Sum(p => p.Quantity)
                                                            );


                        var soldQty = sales.SoldItems.Sum(p => p.Quantity);

                        if ((soldQty == delQuantity) && delQuantity != 0)
                        {

                            //added for optimization
                            sales.OrderStatus = OrderStatusEnum.Completed;

                            sales.DateUpdated = DateTime.Now;
                            _context.STSales.Update(sales);
                            _context.SaveChanges();
                        }
                    }



                }

                return sales.TransactionNo;
            } else {
                return null;
            }
        }

        public void UpdateSalesRecord(ISTStockService stockService, STSales param)
        {
            param.ReleaseDate = DateTime.Now;
            param.DateUpdated = DateTime.Now;

            foreach (var soldItem in param.SoldItems)
            {

                soldItem.DeliveryStatus = DeliveryStatusEnum.Delivered;
                soldItem.DateUpdated = DateTime.Now;

                var stStock = _context.STStocks.Where(p => p.StoreId == param.StoreId
                                                           && p.STSalesDetailId == soldItem.Id
                                                           && p.ItemId == soldItem.ItemId)
                                                .FirstOrDefault();

                if (stStock != null)
                {
                    stStock.ReleaseStatus = ReleaseStatusEnum.Released;
                    stStock.DeliveryStatus = DeliveryStatusEnum.Delivered;

                    stockService.UpdateSTStock(stStock);
                }

            }

            _context.STSales.Update(param);
            _context.SaveChanges();

        }
    }
}

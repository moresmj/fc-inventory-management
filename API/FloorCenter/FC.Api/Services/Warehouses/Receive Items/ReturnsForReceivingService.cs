using FC.Api.DTOs.Warehouse.Receive_Items;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Services.Warehouses.Receive_Items
{
    public class ReturnsForReceivingService : IReturnsForReceivingService
    {

        private DataContext _context;

        public ReturnsForReceivingService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return _context;
        }

        public IEnumerable<object> GetAll(SearchReturnsForReceiving search)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.Store)
                                                 .Include(p => p.Deliveries)
                                                    .ThenInclude(p => p.WarehouseDeliveries)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                 .Where
                                                 (p =>
                                                    p.WarehouseId == search.WarehouseId
                                                    && ((p.RequestStatus == RequestStatusEnum.Approved && p.ReturnType == ReturnTypeEnum.RTV)
                                                    || (p.RequestStatus == RequestStatusEnum.Pending && p.ReturnType == ReturnTypeEnum.Breakage))
                                                 ).OrderByDescending(p => p.Id);

            if(!string.IsNullOrWhiteSpace(search.ReturnFormNumber))
            {
                query = query.Where(p => p.ReturnFormNumber.ToLower() == search.ReturnFormNumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }
            
            var records = new List<object>();

            foreach (var q in query)
            {

                if (!string.IsNullOrWhiteSpace(search.DRNumber))
                {
                    q.Deliveries = q.Deliveries.Where(p => p.DRNumber.ToLower() == search.DRNumber.ToLower()).ToList();
                }

                if(search.StoreId.HasValue)
                {
                    q.Deliveries = q.Deliveries.Where(p => p.StoreId == search.StoreId).ToList();
                }

                if (q.Deliveries != null && q.Deliveries.Count() > 0)
                {
                    foreach (var delivery in q.Deliveries)
                    {
                        var warehouseDeliveries = delivery.WarehouseDeliveries
                                                          .Where(p => search.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                          ? p.DeliveryStatus == DeliveryStatusEnum.Delivered 
                                                          : p.DeliveryStatus == DeliveryStatusEnum.Waiting
                                                                      && p.ReleaseStatus == ReleaseStatusEnum.Released).ToList();
                        if(warehouseDeliveries != null && warehouseDeliveries.Count() > 0)
                        {
                            records.Add(new
                            {
                                delivery.Id,
                                delivery.DRNumber,
                                delivery.DeliveryDate,
                                q.TransactionNo,
                                q.ReturnFormNumber,
                                ReturnedBy = q.Store.Name,
                                q.ReturnType,
                                ReturnTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), q.ReturnType)),
                                delivery.ApprovedDeliveryDate,
                                delivery.DriverName,
                                delivery.PlateNumber,
                                Deliveries = warehouseDeliveries.Select(p => new
                                {
                                    p.Item.SerialNumber,
                                    p.Item.Code,
                                    ItemName = p.Item.Name,
                                    SizeName = p.Item.Size.Name,
                                    p.Item.Tonality,
                                    p.Quantity,
                                    p.DeliveryStatus
                                }),
                                isReceived = warehouseDeliveries.Count() == warehouseDeliveries.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered).Count() ? true : false
                            });
                        }
                    }
                }
            }

            return records;
            
        }

        public object GetReturnsForReceivingByIdAndWarehouseId(int id, int? warehouseId)
        {
            //  Get delivery record
            var delivery = _context.WHDeliveries
                                   .Include(p => p.WarehouseDeliveries)
                                        .ThenInclude(p => p.Item)
                                            .ThenInclude(p => p.Size)
                                   .Where(p => p.Id == id
                                               && p.ApprovedDeliveryDate.HasValue)
                                   .FirstOrDefault();

            if (delivery != null)
            {
                //  Get return record
                var retRecord = _context.STReturns
                                        .Include(p => p.Store)
                                        .Where(p => p.WarehouseId == warehouseId
                                                    && ((p.RequestStatus == RequestStatusEnum.Approved
                                                    && p.ReturnType == ReturnTypeEnum.RTV) || (p.RequestStatus == RequestStatusEnum.Pending
                                                    && p.ReturnType == ReturnTypeEnum.Breakage))
                                                    && p.Id == delivery.STReturnId)
                                        .FirstOrDefault();

                if(retRecord != null)
                {
                    var warehouseDeliveries = delivery.WarehouseDeliveries
                                                          .Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting
                                                                      && p.ReleaseStatus == ReleaseStatusEnum.Released).ToList();

                    if (warehouseDeliveries != null && warehouseDeliveries.Count > 0)
                    {
                        return new
                        {
                            delivery.Id,
                            retRecord.TransactionNo,
                            retRecord.ReturnFormNumber,
                            retRecord.Remarks,
                            retRecord.RequestStatus,
                            requestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), retRecord.RequestStatus)),
                            delivery.DRNumber,
                            ReturnedBy = retRecord.Store.Name,
                            delivery.ApprovedDeliveryDate,
                            Deliveries = warehouseDeliveries.Select(p => new
                            {
                                p.Id,
                                p.WHDeliveryId,
                                p.STPurchaseReturnId,
                                p.ItemId,
                                p.Item.Code,
                                p.Item.SerialNumber,
                                ItemName = p.Item.Name,
                                SizeName = p.Item.Size.Name,
                                p.Item.Tonality,
                                p.Quantity
                            })
                        };
                    }
                }
            }

            return null;
            
        }

        public void ReceiveReturns(ReceiveReturnsDTO param)
        {
            var delivery = _context.WHDeliveries.Where(p => p.Id == param.Id).FirstOrDefault();
            if(delivery != null)
            {

                foreach(var deliveredItems in param.DeliveredItems)
                {
                    var obj = _context.WHDeliveryDetails
                                      .Where(p => p.Id == deliveredItems.Id
                                                  && p.ItemId == deliveredItems.ItemId
                                                  && p.WHDeliveryId == deliveredItems.WHDeliveryId
                                                  && p.DeliveryStatus == DeliveryStatusEnum.Waiting)
                                       .FirstOrDefault();

                    if(obj != null)
                    {
                        obj.ReceivedGoodQuantity = deliveredItems.GoodQuantity;
                        obj.ReceivedBrokenQuantity = deliveredItems.BrokenQuantity;
                        obj.ReceivedRemarks = deliveredItems.ReceivedRemarks;
                        obj.DeliveryStatus = DeliveryStatusEnum.Delivered;
                        _context.WHDeliveryDetails.Update(obj);
                        _context.SaveChanges();


                        var objSTStock = _context.STStocks
                                                 .Where(p => p.WHDeliveryDetailId == deliveredItems.Id
                                                             && p.ItemId == deliveredItems.ItemId)
                                                 .FirstOrDefault();
                        if(objSTStock != null)
                        {
                            objSTStock.DeliveryStatus = DeliveryStatusEnum.Delivered;
                            objSTStock.DateUpdated = DateTime.Now;


                            _context.STStocks.Update(objSTStock);
                            _context.SaveChanges();
                        }


                        var purchaseReturn = _context.STPurchaseReturns
                                                     .Where(p => p.Id == obj.STPurchaseReturnId
                                                                 && p.ItemId == obj.ItemId)
                                                     .FirstOrDefault();

                        if(purchaseReturn !=  null)
                        {

                            var warehouseDeliveries = _context.WHDeliveryDetails
                                                              .Where(p => p.STPurchaseReturnId == obj.STPurchaseReturnId
                                                                          && p.ItemId == obj.ItemId)
                                                              .ToList();

                            if (warehouseDeliveries != null)
                            {

                                //  Get warehouse delivered records
                                var wDeliveredItems = warehouseDeliveries.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                                                    && p.ReleaseStatus == ReleaseStatusEnum.Released);

                                var totalGoodDelivered = Convert.ToInt32(wDeliveredItems.Sum(p => p.ReceivedGoodQuantity));
                                var totalBrokenDelivered = Convert.ToInt32(wDeliveredItems.Sum(p => p.ReceivedBrokenQuantity));


                                //  Get total released not yet delivered items
                                var totalReleased = Convert.ToInt32(warehouseDeliveries.Where(p => p.DeliveryStatus != DeliveryStatusEnum.Delivered
                                                                                   && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                                       .Sum(p => p.Quantity));


                                //re added for ticket 426
                                if (
                                    (totalGoodDelivered + totalBrokenDelivered + totalReleased)
                                    ==
                                    (purchaseReturn.GoodQuantity + purchaseReturn.BrokenQuantity)
                                   )
                                {
                                    //  Mark record as delivered
                                    purchaseReturn.DeliveryStatus = DeliveryStatusEnum.Delivered;
                                    purchaseReturn.ActualBrokenQuantity = totalBrokenDelivered;
                                    purchaseReturn.DateUpdated = DateTime.Now;

                                    _context.STPurchaseReturns.Update(purchaseReturn);
                                    _context.SaveChanges();
                                }
                            }
                        }

                        SaveRecordToWHStock(param, obj);

                    }

                }

                this.CheckReturnIfCompleted(delivery.STReturnId);


               
            }
        }


        private void CheckReturnIfCompleted(int? STReturnId)
        {
            var stReturns = _context.STReturns
                   .Include(p => p.PurchasedItems)
                   .Include(p => p.Deliveries)
                       .ThenInclude(p => p.WarehouseDeliveries)
                   .Where(p => p.Id == STReturnId).FirstOrDefault();


            var pQuantity = stReturns.PurchasedItems?.Sum(p => p.GoodQuantity) + stReturns.PurchasedItems?.Sum(p => p.BrokenQuantity);
            var dQuantity = stReturns.Deliveries?.Sum(p => p.WarehouseDeliveries.Sum(z => z.ReceivedGoodQuantity)) + stReturns.Deliveries?.Sum(p => p.WarehouseDeliveries.Sum(z => z.ReceivedBrokenQuantity));

            if (pQuantity == dQuantity)
            {
                stReturns.OrderStatus = OrderStatusEnum.Completed;
                stReturns.DateUpdated = DateTime.Now;

                _context.STReturns.Update(stReturns);
                _context.SaveChanges();

            }
        }

        private void SaveRecordToWHStock(ReceiveReturnsDTO param, WHDeliveryDetail obj)
        {

            var service = new WHStockService(_context);

            if (obj.ReceivedGoodQuantity.HasValue && obj.ReceivedGoodQuantity > 0)
            {
                var whStock = new WHStock
                {
                    WarehouseId = param.WarehouseId,
                    WHDeliveryDetailId = obj.Id,
                    ItemId = obj.ItemId,
                    OnHand = obj.ReceivedGoodQuantity,
                    TransactionType = TransactionTypeEnum.Return,
                    DeliveryStatus = DeliveryStatusEnum.Delivered
                };

                service.InsertStock(whStock);
            }

            if(obj.ReceivedBrokenQuantity.HasValue && obj.ReceivedBrokenQuantity > 0)
            {
                var whStock = new WHStock
                {
                    WarehouseId = param.WarehouseId,
                    WHDeliveryDetailId = obj.Id,
                    ItemId = obj.ItemId,
                    OnHand = obj.ReceivedBrokenQuantity,
                    Broken = true,
                    TransactionType = TransactionTypeEnum.Return,
                    DeliveryStatus = DeliveryStatusEnum.Delivered
                };

                service.InsertStock(whStock);
            }

        }
    }
}

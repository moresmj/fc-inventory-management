using FC.Api.DTOs.Store.ReceiveItems;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Services.Stores.ReceiveItems
{
    public class ClientReturnService : IClientReturnService
    {

        private DataContext _context;

        public ClientReturnService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return this._context;
        }

        public IEnumerable<object> GetAll(SearchReceiveItemsClientReturns search)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.Store)
                                                 .Include(p => p.ClientPurchasedItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                 .Where
                                                 (p =>
                                                     p.StoreId == search.StoreId
                                                     && p.ReturnType == ReturnTypeEnum.ClientReturn
                                                 ).OrderByDescending(p => p.Id);

            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            var retList = new List<object>();
            foreach (var x in query)
            {
                List<STClientReturn> clientPurchasedItems = null;

                if (x.ClientReturnType == ClientReturnTypeEnum.StoreReturn)
                {
                    clientPurchasedItems = x.ClientPurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Pending).ToList();
                }
                else
                {
                    clientPurchasedItems = x.ClientPurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList();
                }

                if (clientPurchasedItems != null && clientPurchasedItems.Count > 0)
                {
                    var obj = new
                    {
                        x.Id,
                        x.TransactionNo,
                        x.ReturnType,
                        ReturnTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), x.ReturnType)),
                        ReturnedBy = _context.STSales.Where(p => p.Id == x.STSalesId && p.StoreId == x.StoreId).FirstOrDefault().ClientName,
                        ReturnedTo = x.Store.Name,
                        RequestDate = x.DateCreated,
                        x.ApprovedDeliveryDate,
                        ReturnDRNumber = (x.ReturnType == ReturnTypeEnum.ClientReturn) ? "" : x.ReturnDRNumber,
                        x.Remarks,
                        x.RequestStatus,
                        RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), x.RequestStatus))
                    };

                    retList.Add(obj);
                }
            }

            return retList;
        }

        public object GetByIdAndStoreId(int? id, int? storeId)
        {
            var record = _context.STReturns
                                 .Include(p => p.Store)
                                 .Include(p => p.ClientPurchasedItems)
                                 .ThenInclude(p => p.Item)
                                    .ThenInclude(p => p.Size)
                                 .Where
                                 (p =>
                                     p.Id == id
                                     && p.StoreId == storeId
                                     && p.ReturnType == ReturnTypeEnum.ClientReturn
                                 )
                                 .FirstOrDefault();

            if(record != null)
            {

                List<STClientReturn> clientPurchasedItems = null;

                if (record.ClientReturnType == ClientReturnTypeEnum.StoreReturn)
                {
                    clientPurchasedItems = record.ClientPurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Pending).ToList();
                }
                else
                {
                    clientPurchasedItems = record.ClientPurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList();
                }

                if (clientPurchasedItems != null && clientPurchasedItems.Count > 0)
                {
                    return new
                    {
                        record.Id,
                        record.TransactionNo,
                        record.ReturnType,
                        record.ReturnDRNumber,
                        record.ReturnFormNumber,
                        record.STSalesId,
                        ReturnTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), record.ReturnType)),
                        ReturnedBy = _context.STSales.Where(p => p.Id == record.STSalesId && p.StoreId == record.StoreId).FirstOrDefault().ClientName,
                        ReturnedTo = record.Store.Name,
                        RequestDate = record.DateCreated,
                        record.Remarks,
                        ClientPurchasedItems = clientPurchasedItems.Select(p => new
                        {
                            p.Id,
                            p.STReturnId,
                            p.ItemId,
                            serialNumber = p.Item.SerialNumber,
                            itemCode = p.Item.Code,
                            ItemName = p.Item.Name,
                            SizeName = p.Item.Size.Name,
                            p.Item.Tonality,
                            p.Quantity,
                            p.ReturnReason,
                            ReturnReasonStr = (p.ReturnReason.HasValue) ? EnumExtensions.SplitName(Enum.GetName(typeof(ReturnReasonEnum), p.ReturnReason)) : null,
                            p.Remarks,
                            p.isTonalityAny,
                        }),
                        record.RequestStatus,
                        RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), record.RequestStatus)),
                        record.ApprovedDeliveryDate,
                        record.DriverName,
                        record.PlateNumber
                    };
                }
            }
            
            return null;
        }

        private STReturn GetReturnByIdAndStoreId(int? id, int? storeId)
        {
            var record = _context.STReturns
                                 .Include(p => p.Store)
                                 .Include(p => p.ClientPurchasedItems)
                                 .ThenInclude(p => p.Item)
                                    .ThenInclude(p => p.Size)
                                 .Where
                                 (p =>
                                     p.Id == id
                                     && p.StoreId == storeId
                                     && p.ReturnType == ReturnTypeEnum.ClientReturn
                                 )
                                 .FirstOrDefault();

            
            return record;
        }

        public void ReceiveClientReturns(ReceiveClientReturnsDTO param)
        {
            var objSTReturn = this.GetReturnByIdAndStoreId(param.Id, param.StoreId);
            if(objSTReturn != null)
            {

                var stockService = new STStockService(_context);
                foreach (var item in param.ClientPurchasedItems)
                {
                    var objSTClientReturn = _context.STClientReturns
                                                    .Where(p => p.Id == item.Id
                                                                && p.STReturnId == item.STReturnId
                                                                && p.ItemId == item.ItemId)
                                                    .FirstOrDefault();

                    if(objSTClientReturn != null)
                    {
                        objSTClientReturn.ReceivedRemarks = item.ReceivedRemarks;
                        objSTClientReturn.DeliveryStatus = DeliveryStatusEnum.Delivered;
                        objSTClientReturn.DateUpdated = DateTime.Now;

                        _context.STClientReturns.Update(objSTClientReturn);
                        _context.SaveChanges();

                        var stStock = new STStock
                        {
                            StoreId = param.StoreId,
                            STClientReturnId = objSTClientReturn.Id,
                            ItemId = objSTClientReturn.ItemId,
                            OnHand = objSTClientReturn.Quantity,
                            DeliveryStatus = DeliveryStatusEnum.Delivered
                        };

                        if(objSTClientReturn.ReturnReason == ReturnReasonEnum.Breakage)
                        {
                            stStock.Broken = true;
                        }

                        stockService.InsertSTStock(stStock);

                    }
                    
                }

                objSTReturn.RequestStatus = RequestStatusEnum.Approved;
                objSTReturn.DateUpdated = DateTime.Now;
                _context.STReturns.Update(objSTReturn);
                _context.SaveChanges();


                if(objSTReturn.ReturnType == ReturnTypeEnum.ClientReturn && (objSTReturn.ClientReturnType == ClientReturnTypeEnum.StoreReturn || objSTReturn.ClientReturnType ==ClientReturnTypeEnum.RequestPickup))
                {
                    if(objSTReturn.ClientPurchasedItems.Count() == objSTReturn.ClientPurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered).Count())
                    {
                        objSTReturn.OrderStatus = OrderStatusEnum.Completed;
                    }
                }

            }
        }
    }
}

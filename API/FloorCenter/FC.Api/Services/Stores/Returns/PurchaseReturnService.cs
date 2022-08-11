using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FC.Api.Services.Stores.Returns
{
    public class PurchaseReturnService : IPurchaseReturnService
    {

        private DataContext _context;

        public PurchaseReturnService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return this._context;
        }

        public void AddPurchaseReturn(STReturn param)
        {
            var totalRecordCount = Convert.ToInt32
                                   (
                                        this._context.STReturns
                                                     .Where(x => x.ReturnType == param.ReturnType)
                                                     .Count() + 1
                                    ).ToString();

            param.TransactionNo = string.Format("RT{0}", totalRecordCount.PadLeft(6, '0'));

            param.DateCreated = DateTime.Now;
            param.RequestStatus = RequestStatusEnum.Pending;
            //added for optimization and filtering
            param.OrderStatus = OrderStatusEnum.Incomplete;

            foreach (var retItem in param.PurchasedItems)
            {
                retItem.DateCreated = DateTime.Now;
                retItem.DeliveryStatus = DeliveryStatusEnum.Pending;
                retItem.ReleaseStatus = ReleaseStatusEnum.Pending;
            }

            //  Get store info
            var storeInfo = new StoreService(_context).GetStoreById(param.StoreId);
            if(storeInfo != null)
            {
                //  Set warehouseid from storeinfo
                param.WarehouseId = storeInfo.WarehouseId;
            }

            _context.STReturns.Add(param);
            _context.SaveChanges();
        }

        public void AddBreakage(STReturn param)
        {
            var totalRecordCount = Convert.ToInt32
                                   (
                                        this._context.STReturns
                                                     .Where(x => x.ReturnType == ReturnTypeEnum.Breakage)
                                                     .Count() + 1
                                    ).ToString();

            param.TransactionNo = string.Format("BR{0}", totalRecordCount.PadLeft(6, '0'));

            param.DateCreated = DateTime.Now;
            param.RequestStatus = RequestStatusEnum.Pending;
            param.ReturnFormNumber = param.TransactionNo;

            //added for optimization
            param.OrderStatus = OrderStatusEnum.Incomplete;

            foreach (var retItem in param.PurchasedItems)
            {
                retItem.DateCreated = DateTime.Now;
                retItem.DeliveryStatus = DeliveryStatusEnum.Pending;
                retItem.ReleaseStatus = ReleaseStatusEnum.Pending;
                retItem.ReturnReason = ReturnReasonEnum.Breakage;
                retItem.GoodQuantity = 0;

                var stockItem = _context.STStocks.AsNoTracking().Where(s => s.StoreId == param.StoreId && s.ItemId == retItem.ItemId).FirstOrDefault();

                if (stockItem != null)
                {
                    stockItem.ChangeDate = DateTime.Now;
                }
                _context.STStocks.Update(stockItem);
            }
            
            //  Get store info
            var storeInfo = new StoreService(_context).GetStoreById(param.StoreId);
            if (storeInfo != null)
            {
                //  Set warehouseid from storeinfo
                param.WarehouseId = storeInfo.WarehouseId;
            }

            _context.STReturns.Add(param);
            _context.SaveChanges();
        }


        public object GetPurchaseReturnByIdAndStoreId(int id, int? storeId)
        {
            var record = this._context.STReturns
                                      .Where(p => p.Id == id
                                                  && p.StoreId == storeId
                                                  && (p.ReturnType == ReturnTypeEnum.RTV || p.ReturnType == ReturnTypeEnum.Breakage))
                                      .FirstOrDefault();

            return record;
        }

        public object GetPurchaseReturnDeliveryRecordsByIdAndStoreId(int? id, int? storeId)
        {
            var record = this._context.STReturns
                                      .Include(p => p.PurchasedItems)
                                            .ThenInclude(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                      .Include(p => p.Deliveries)
                                            .ThenInclude(p => p.WarehouseDeliveries)
                                                .ThenInclude(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                      .Include(p => p.Store)
                                      .Include(p => p.Warehouse)
                                      .Where(p => p.Id == id
                                                  && p.StoreId == storeId
                                                  && ((p.ReturnType == ReturnTypeEnum.RTV && p.RequestStatus == RequestStatusEnum.Approved) || (p.ReturnType == ReturnTypeEnum.Breakage 
                                                  && p.RequestStatus == RequestStatusEnum.Pending)))
                                      .FirstOrDefault();

            var retObj = new
            {
                record.Id,
                record.TransactionNo,
                record.ReturnFormNumber,
                ReturnedTo = record.Warehouse.Name,
                record.Remarks,
                Deliveries = record.Deliveries.Select(p => new
                {
                    p.DRNumber,
                    p.WarehouseDeliveries,
                    p.DeliveryDate,
                    p.ApprovedDeliveryDate,
                    Status = EnumExtensions.SplitName(Enum.GetName(typeof(ReleaseStatusEnum), p.WarehouseDeliveries.Select(x => x.ReleaseStatus).FirstOrDefault()))
                }),
                record.PurchasedItems,
                RemainingForDelivery = GetRemainingForDelivery(record)
            };

            return retObj;
        }

        private int GetRemainingForDelivery(STReturn record)
        {
            var goodQtyTotal = Convert.ToInt32(record.PurchasedItems.Sum(p => p.GoodQuantity));
            var brokenQtyTotal = Convert.ToInt32(record.PurchasedItems.Sum(p => p.BrokenQuantity));


            int total = 0;
            foreach (var del in record.Deliveries)
            {
                if (del.WarehouseDeliveries != null)
                {
                    total += Convert.ToInt32(del.WarehouseDeliveries.Sum(p => p.Quantity));
                }
            }

            return (goodQtyTotal + brokenQtyTotal) - total;
        }
    }
}

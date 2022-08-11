using System;
using System.Collections.Generic;
using System.Linq;
using FC.Api.DTOs.Store.Returns;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;

namespace FC.Api.Services.Stores.Releasing
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

        public IEnumerable<object> GetAll(SearchReturns search)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.Deliveries)
                                                    .ThenInclude(p => p.WarehouseDeliveries)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                                .Include(p => p.Warehouse)
                                                 .Where
                                                 (p => 
                                                        p.StoreId == search.StoreId
                                                        && ((p.ReturnType == ReturnTypeEnum.RTV 
                                                        && p.RequestStatus == RequestStatusEnum.Approved) 
                                                        || (p.ReturnType == ReturnTypeEnum.Breakage
                                                        && p.RequestStatus == RequestStatusEnum.Pending))
                                                 );

            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ReturnFormNumber))
            {
                query = query.Where(p => p.ReturnFormNumber.ToLower() == search.ReturnFormNumber.ToLower());
            }

            var records = new List<object>();
            foreach (var q in query.OrderByDescending(p => p.Id))
            {
                q.Deliveries = q.Deliveries.Where(p => p.ApprovedDeliveryDate.HasValue == true).ToList();

                if (!string.IsNullOrWhiteSpace(search.DRNumber))
                {
                    q.Deliveries = q.Deliveries.Where(p => p.DRNumber.ToLower() == search.DRNumber.ToLower()).ToList();
                }

                if (q.Deliveries != null && q.Deliveries.Count() > 0)
                {
                    foreach (var delivery in q.Deliveries.OrderByDescending(p => p.Id))
                    {
                        var warehouseDeliveries = delivery.WarehouseDeliveries.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList();
                        if (warehouseDeliveries != null && warehouseDeliveries.Count() > 0)
                        {
                            records.Add(new
                            {
                                delivery.Id,
                                delivery.DRNumber,
                                delivery.DeliveryDate,
                                q.TransactionNo,
                                q.ReturnFormNumber,
                                q.ReturnType,
                                ReturnTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), q.ReturnType)),
                                delivery.ApprovedDeliveryDate,
                                delivery.DriverName,
                                delivery.PlateNumber,
                                q.Remarks,
                                q.Warehouse.ContactNumber,
                                delivery.ReleaseDate,
                                IsReleased = warehouseDeliveries
                                                    .Where(p => p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                    .Count() != 0,
                                Deliveries = warehouseDeliveries.Select(p => new
                                {
                                    p.Item.SerialNumber,
                                    p.Item.Code,
                                    ItemName = p.Item.Name,
                                    SizeName = p.Item.Size.Name,
                                    p.Item.Tonality,
                                    p.Quantity,
                                    DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus)),
                                })
                            });
                        }
                    }
                }
            }

            return records;

        }


        public object GetAllPaged(SearchReturns search, AppSettings appSettings)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.Deliveries)
                                                    .ThenInclude(p => p.WarehouseDeliveries)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                                .Include(p => p.Warehouse)
                                                 .Where
                                                 (p =>
                                                        p.StoreId == search.StoreId
                                                        && ((p.ReturnType == ReturnTypeEnum.RTV
                                                        && p.RequestStatus == RequestStatusEnum.Approved)
                                                        || (p.ReturnType == ReturnTypeEnum.Breakage
                                                        && p.RequestStatus == RequestStatusEnum.Pending))
                                                 );

            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ReturnFormNumber))
            {
                query = query.Where(p => p.ReturnFormNumber.ToLower() == search.ReturnFormNumber.ToLower());
            }

            query = query.Where(p => p.Deliveries != null && p.Deliveries.Count() > 0 && p.Deliveries.Any(x => x.ApprovedDeliveryDate.HasValue));

            //test = test.Where(p => p.Deliveries.Any(x => x.DRNumber.ToLower() == search.DRNumber.ToLower()));

            query = query.Where(p => p.Deliveries.Any(x => x.WarehouseDeliveries.Any(w => w.DeliveryStatus == DeliveryStatusEnum.Waiting)));

            var deliveries = query.SelectMany(p => p.Deliveries);

      


            GetAllResponse response = null;
            if (search.ShowAll == false)
            {
                response = new GetAllResponse(deliveries.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                //check if currentpage is greater than totalpage
                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;


                }

                deliveries = deliveries.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                            .Take(appSettings.RecordDisplayPerPage);



            }
            else
            {
                response = new GetAllResponse(deliveries.Count());
            }

            var delReturnId = deliveries.Select(p => p.STReturnId);
            var returnId = _context.STReturns.Where(p => delReturnId.Contains(p.Id)).Select(p => p.Id);
            var returns = query.Where(p => returnId.Contains(p.Id));


            var records = new List<object>();
            
            foreach (var delivery in deliveries.OrderByDescending(p => p.Id))
            {
                var returnDetails = returns.Where(p => p.Id == delivery.STReturnId).FirstOrDefault();
                //var warehouseDeliveries = delivery.WarehouseDeliveries.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList();
                //if (warehouseDeliveries != null && warehouseDeliveries.Count() > 0)
                //{
                records.Add(new
                {
                    delivery.Id,
                    delivery.DRNumber,
                    delivery.DeliveryDate,
                    returnDetails.TransactionNo,
                    returnDetails.ReturnFormNumber,
                    returnDetails.ReturnType,
                    ReturnTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), returnDetails.ReturnType)),
                    delivery.ApprovedDeliveryDate,
                    delivery.DriverName,
                    delivery.PlateNumber,
                    returnDetails.Remarks,
                    returnDetails.Warehouse.ContactNumber,
                    delivery.ReleaseDate,
                    IsReleased = delivery.WarehouseDeliveries
                                        .Where(p => p.ReleaseStatus == ReleaseStatusEnum.Released)
                                        .Count() != 0,
                    Deliveries = delivery.WarehouseDeliveries.Select(p => new
                    {
                        p.Item.SerialNumber,
                        p.Item.Code,
                        ItemName = p.Item.Name,
                        SizeName = p.Item.Size.Name,
                        p.Item.Tonality,
                        p.Quantity,
                        DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus)),
                        p.DeliveryStatus,
                    }).Where(p => p.DeliveryStatus == DeliveryStatusEnum.Waiting)
                });
                //}
            }

            response.List.AddRange(records);
              
            return response;

        }

        public WHDelivery GetDeliveryByIdAndStoreId(int id, int? storeId)
        {
            var record = _context.WHDeliveries
                                 .Include(p => p.WarehouseDeliveries)
                                 .Where(p => p.StoreId == storeId
                                             && p.Id == id
                                             && p.ApprovedDeliveryDate.HasValue
                                        )
                                 .FirstOrDefault();

            if (record != null)
            {
                var totalNotDelivered = record.WarehouseDeliveries
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

        public void ReleasePurchaseReturnDelivery(ISTStockService stockService, WHDelivery param)
        {
            param.DateUpdated = DateTime.Now;
            param.ReleaseDate = DateTime.Now;

            foreach(var delivery in param.WarehouseDeliveries)
            {
                var stStock = new STStock
                {
                    StoreId = param.StoreId,
                    WHDeliveryDetailId = delivery.Id,
                    ItemId = delivery.ItemId,
                    OnHand = -delivery.Quantity,
                    DeliveryStatus = DeliveryStatusEnum.Waiting,
                    ReleaseStatus = ReleaseStatusEnum.Released,
                    //added flag to reflect on inventory history once released
                    IsDeliveryTransfer = true
                };

                stockService.InsertSTStock(stStock);

                delivery.ReleaseStatus = ReleaseStatusEnum.Released;
                delivery.DateUpdated = DateTime.Now;
            }

            _context.WHDeliveries.Update(param);
            _context.SaveChanges();

            foreach (var delivery in param.WarehouseDeliveries)
            {
                var retItem = _context.STPurchaseReturns
                                      .Where(p => p.Id == delivery.STPurchaseReturnId
                                                  && p.ItemId == delivery.ItemId
                                                  && p.ReleaseStatus != ReleaseStatusEnum.Released)
                                      .FirstOrDefault();
                if(retItem != null)
                {

                    var warehouseDeliveries = _context.WHDeliveryDetails
                                                      .Where(p => p.STPurchaseReturnId == retItem.Id
                                                                  && p.ItemId == retItem.ItemId)
                                                      .ToList();

                    //  Get warehouse released records
                    var wReleasedItems = warehouseDeliveries.Where(p => p.ReleaseStatus == ReleaseStatusEnum.Released);

                    var totalReleased = Convert.ToInt32(wReleasedItems.Sum(p => p.Quantity));

                    if((Convert.ToInt32(retItem.BrokenQuantity) + Convert.ToInt32(retItem.GoodQuantity))
                        == totalReleased)
                    {
                        //  Mark record as released
                        retItem.ReleaseStatus = ReleaseStatusEnum.Released;
                        retItem.DateUpdated = DateTime.Now;

                        _context.STPurchaseReturns.Update(retItem);
                        _context.SaveChanges();
                    }
                }
            }

        }
    }
}

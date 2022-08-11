using System;
using System.Collections.Generic;
using System.Linq;
using FC.Api.DTOs.Store.Returns;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Items;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;

namespace FC.Api.Services.Stores.Returns
{
    public class ReturnService : IReturnService
    {

        private DataContext _context;

        public ReturnService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return this._context;
        }


        public IEnumerable<object> GetAllForApproval(SearchReturns search)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.Store)
                                                 .Include(p => p.Warehouse)
                                                 .Include(p => p.PurchasedItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                        .Where(p => ((p.ReturnType == ReturnTypeEnum.Breakage  || p.ReturnType == ReturnTypeEnum.RTV)));


            if (search.StoreId.HasValue)
            {
                query = query.Where(p => p.StoreId == search.StoreId);
            }

            if (search.RequestDateFrom.HasValue)
            {
                query = query.Where(p => search.RequestDateFrom.Value <= p.DateCreated);
            }

            if (search.RequestDateTo.HasValue)
            {
                query = query.Where(p => search.RequestDateTo.Value >= p.DateCreated);
            }

            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ReturnFormNumber))
            {
                query = query.Where(p => p.ReturnFormNumber.ToLower() == search.ReturnFormNumber.ToLower());
            }

            if (search.RequestStatus != null)
            {
                query = query.Where(p => search.RequestStatus.Contains(p.RequestStatus));
            }


            return BuildList(query, search);

        }

        public object GetForApprovalPaged(SearchReturns search, AppSettings appSettings)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.Store)
                                                 .Include(p => p.Warehouse)
                                                 .Include(p => p.PurchasedItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                        .Where(p => ((p.ReturnType == ReturnTypeEnum.Breakage || p.ReturnType == ReturnTypeEnum.RTV)));


            if (search.StoreId.HasValue)
            {
                query = query.Where(p => p.StoreId == search.StoreId);
            }

            if (search.RequestDateFrom.HasValue)
            {
                query = query.Where(p => search.RequestDateFrom.Value <= p.DateCreated);
            }

            if (search.RequestDateTo.HasValue)
            {
                query = query.Where(p => search.RequestDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59) >= p.DateCreated);
            }

            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ReturnFormNumber))
            {
                query = query.Where(p => p.ReturnFormNumber.ToLower() == search.ReturnFormNumber.ToLower());
            }

            if (search.RequestStatus != null)
            {
                query = query.Where(p => search.RequestStatus.Contains(p.RequestStatus));
            }

            var records = BuildList(query, search);

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


        public IEnumerable<object> GetAllReturns(SearchReturns search)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.Store)
                                                 .Include(p => p.Warehouse)
                                                 .Include(p => p.PurchasedItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                 .Include(p => p.ClientPurchasedItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                 .Include(p => p.Deliveries)
                                                    .ThenInclude(p => p.WarehouseDeliveries)
                                                  
                                                 .Where
                                                 (p =>
                                                    p.StoreId == search.StoreId

                                                 ).OrderByDescending(p => p.Id);

            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ReturnFormNumber))
            {
                query = query.Where(p => p.ReturnFormNumber.ToLower() == search.ReturnFormNumber.ToLower());
            }
            if (search.ReturnType.HasValue)
            {
                query = query.Where(p => p.ReturnType == search.ReturnType);
            }
            if (search.RequestDateFrom.HasValue)
            {
                query = query.Where(p => search.RequestDateFrom.Value <= p.DateCreated);
            }

            if (search.RequestDateTo.HasValue)
            {

                query = query.Where(p => search.RequestDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59) >= p.DateCreated);
            }

            if (search.RequestStatus != null)
            {

                query = query.Where(p => search.RequestStatus.Contains(p.RequestStatus));


            }


        

            return BuildList(query, search);
        }


        private IEnumerable<object> BuildList(IQueryable<STReturn> query, SearchReturns search)
        {
            var retList = new List<object>();
            foreach (var x in query.OrderByDescending(p => p.Id))
            {

                OrderStatusEnum? OrderStatus = null;
                var OrderStatusStr = "";

                if (x.RequestStatus != RequestStatusEnum.Approved)
                {
                    OrderStatus = OrderStatusEnum.Incomplete;
                }
                else
                {
                    if (x.ReturnType == ReturnTypeEnum.RTV)
                    {
                       
                    
                        var pQuantity = x.PurchasedItems?.Sum(p => p.GoodQuantity) + x.PurchasedItems?.Sum(p => p.BrokenQuantity);
                        var dQuantity =  x.Deliveries?.Sum(p => p.WarehouseDeliveries.Sum(z => z.ReceivedGoodQuantity)) + x.Deliveries?.Sum(p => p.WarehouseDeliveries.Sum(z => z.ReceivedBrokenQuantity));

                        OrderStatus = pQuantity == dQuantity ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;
 
                        //OrderStatus = (x.PurchasedItems.Count() == x.PurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered).Count()) ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;
                    }
                    else if (x.ReturnType == ReturnTypeEnum.Breakage)
                    {
                        OrderStatus = (x.PurchasedItems.Count() == x.PurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered).Count()) ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;
                    }
                    else if (x.ReturnType == ReturnTypeEnum.ClientReturn)
                    {
                        OrderStatus = (x.ClientPurchasedItems.Count() == x.ClientPurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered).Count()) ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;
                    }



                }
                OrderStatusStr = (OrderStatus.HasValue) ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), OrderStatus)) : null;

                if (search.OrderStatus != null)
                {
                    if (!search.OrderStatus.Contains(OrderStatus))
                    {
                        continue;
                    }

                }


                var SINumber = "";
                if (x.ClientPurchasedItems?.Count > 0)
                {

                    var stdetail = _context.STSalesDetails.Where(p => p.Id == x.ClientPurchasedItems.Select(z => z.STSalesDetailId).FirstOrDefault()).FirstOrDefault();
                    SINumber = _context.STSales.Where(p => p.Id == stdetail.STSalesId).ToList().Select(z => z.SINumber).FirstOrDefault();
                }

                var obj = new
                {
                    x.Id,
                    x.TransactionNo,
                    x.ReturnFormNumber,
                    x.ReturnType,
                    x.ClientReturnType,
                    ReturnDrNumber  = x.ReturnDRNumber,
                    ApproveDeliveryDate = x.ApprovedDeliveryDate,
                    ReturnTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), x.ReturnType)),
                    ReturnedBy = (x.ReturnType == ReturnTypeEnum.RTV || x.ReturnType == ReturnTypeEnum.Breakage)
                                 ? x.Store.Name
                                 : _context.STSales.Where(p => p.Id == x.STSalesId && p.StoreId == x.StoreId).FirstOrDefault().ClientName,
                    ReturnedTo = (x.ReturnType == ReturnTypeEnum.RTV || x.ReturnType == ReturnTypeEnum.Breakage)
                                 ? x.Warehouse.Name
                                 : x.Store.Name,
                    RequestDate = x.DateCreated,
                    x.Remarks,
                    Items = (x.ReturnType == ReturnTypeEnum.RTV || x.ReturnType == ReturnTypeEnum.Breakage)
                            ?
                            x.PurchasedItems.Select(p => new
                            {
                                serialNumber = p.Item.SerialNumber,
                                itemCode = p.Item.Code,
                                ItemName = p.Item.Name,
                                SizeName = p.Item.Size.Name,
                                p.Item.Tonality,
                                p.BrokenQuantity,
                                p.GoodQuantity,
                                p.ActualBrokenQuantity,
                                ReturnReasonStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnReasonEnum), p.ReturnReason)),
                                p.Remarks,
                                totalQty = p.BrokenQuantity + p.GoodQuantity,
                                p.DeliveryStatus
                            })
                            :
                            null,
                    ClientPurchasedItems = (x.ReturnType == ReturnTypeEnum.ClientReturn)
                                           ?
                                            x.ClientPurchasedItems.Select(p => new
                                            {
                                                serialNumber = p.Item.SerialNumber,
                                                itemCode = p.Item.Code,
                                                ItemName = p.Item.Name,
                                                SizeName = p.Item.Size.Name,
                                                p.Item.Tonality,
                                                p.Quantity,
                                                ReturnReasonStr = (p.ReturnReason.HasValue) ? EnumExtensions.SplitName(Enum.GetName(typeof(ReturnReasonEnum), p.ReturnReason)) : null,
                                                p.Remarks
                                            })
                                            : null,
                    x.RequestStatus,
                    RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), x.RequestStatus)),
                    OrderStatus,
                    OrderStatusStr,
                    TestStatus = x.OrderStatus,
                    TestStatusStr = (x.OrderStatus.HasValue) ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), x.OrderStatus)) : null,
                    SINumber
                };


                //check if the deliveries is already delivered before displaying in Approve returns main
                if (obj.ReturnType == ReturnTypeEnum.Breakage && search.mainList == true)
                {
                    var delivered = 0;
                    foreach (var item in obj.Items)
                    {
                        if (item.DeliveryStatus == DeliveryStatusEnum.Delivered)
                        {
                            delivered++;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (delivered > 0)
                    {
                        retList.Add(obj);
                    }

                }
                else
                {
                    retList.Add(obj);
                }


                





            }




            return retList;
        }


        public object GetForApprovalPaged2(SearchReturns search, AppSettings appSettings)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.Store)
                                                 .Include(p => p.Warehouse)
                                                 .Include(p => p.PurchasedItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                        .Where(p => ((p.ReturnType == ReturnTypeEnum.Breakage || p.ReturnType == ReturnTypeEnum.RTV)));


            if (search.StoreId.HasValue)
            {
                query = query.Where(p => p.StoreId == search.StoreId);
            }

            if (search.RequestDateFrom.HasValue)
            {
                query = query.Where(p => search.RequestDateFrom.Value <= p.DateCreated);
            }

            if (search.RequestDateTo.HasValue)
            {
                query = query.Where(p => search.RequestDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59) >= p.DateCreated);
            }

            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ReturnFormNumber))
            {
                query = query.Where(p => p.ReturnFormNumber.ToLower() == search.ReturnFormNumber.ToLower());
            }

            if (search.RequestStatus != null)
            {
                query = query.Where(p => search.RequestStatus.Contains(p.RequestStatus));
            }


            if (search.mainList)
            {
                query = query.Where(p => p.ReturnType == ReturnTypeEnum.Breakage
                                        ? p.PurchasedItems.Where(x => x.DeliveryStatus == DeliveryStatusEnum.Delivered).Count() > 0
                                        : p.ReturnType != null);
            }

            query = query.OrderByDescending(p => p.Id);

           

            GetAllResponse response = null;
            if (search.ShowAll == false)
            {
                response = new GetAllResponse(query.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


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

            var records = BuildList2(query, search);


            response.List.AddRange(records);


            return response;

        }



        public object GetAllReturnsPaged(SearchReturns search, AppSettings appSettings)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.Store)
                                                 .Include(p => p.Warehouse)
                                                 .Include(p => p.PurchasedItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                 .Include(p => p.ClientPurchasedItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                 .Include(p => p.Deliveries)
                                                    .ThenInclude(p => p.WarehouseDeliveries)

                                                 .Where
                                                 (p =>
                                                    p.StoreId == search.StoreId

                                                 ).OrderByDescending(p => p.Id);

            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ReturnFormNumber))
            {
                query = query.Where(p => p.ReturnFormNumber.ToLower() == search.ReturnFormNumber.ToLower());
            }
            if (search.ReturnType.HasValue)
            {
                query = query.Where(p => p.ReturnType == search.ReturnType);
            }
            if (search.RequestDateFrom.HasValue)
            {
                query = query.Where(p => search.RequestDateFrom.Value <= p.DateCreated);
            }

            if (search.RequestDateTo.HasValue)
            {

                query = query.Where(p => search.RequestDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59) >= p.DateCreated);
            }

            if (search.RequestStatus != null)
            {

                query = query.Where(p => search.RequestStatus.Contains(p.RequestStatus));


            }

            if (search.mainList)
            {
                query = query.Where(p => p.ReturnType == ReturnTypeEnum.Breakage
                                        ? p.PurchasedItems.Where(x => x.DeliveryStatus == DeliveryStatusEnum.Delivered).Count() > 0
                                        : p.ReturnType != null);
            }

            if (search.OrderStatus != null)
            {
                query = query.Where(p => search.OrderStatus.Contains(p.OrderStatus));

            }


            GetAllResponse response = null;


            if (search.ShowAll == false)
            {
                response = new GetAllResponse(query.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                //check if currentpage is greater than totalpage
                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();


                }

                query = query.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                            .Take(appSettings.RecordDisplayPerPage);



            }
            else
            {
                response = new GetAllResponse(query.Count());
            }

            var formattedList = BuildList2(query, search);

            response.List.AddRange(formattedList);


            return response;
        }


        private List<object> BuildList2(IQueryable<STReturn> query, SearchReturns search)
        {
            var retList = new List<object>();
            var returns = query.ToList();
            foreach (var x in returns.OrderByDescending(p => p.Id))
            {

                OrderStatusEnum? OrderStatus = null;
                var OrderStatusStr = "";

                if (x.RequestStatus != RequestStatusEnum.Approved)
                {
                    OrderStatus = OrderStatusEnum.Incomplete;
                }
                else
                {
                    if (x.ReturnType == ReturnTypeEnum.RTV)
                    {


                        var pQuantity = x.PurchasedItems?.Sum(p => p.GoodQuantity) + x.PurchasedItems?.Sum(p => p.BrokenQuantity);
                        var dQuantity = x.Deliveries?.Sum(p => p.WarehouseDeliveries.Sum(z => z.ReceivedGoodQuantity)) + x.Deliveries?.Sum(p => p.WarehouseDeliveries.Sum(z => z.ReceivedBrokenQuantity));

                        OrderStatus = pQuantity == dQuantity ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;

                        //OrderStatus = (x.PurchasedItems.Count() == x.PurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered).Count()) ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;
                    }
                    else if (x.ReturnType == ReturnTypeEnum.Breakage)
                    {
                        OrderStatus = (x.PurchasedItems.Count() == x.PurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered).Count()) ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;
                    }
                    else if (x.ReturnType == ReturnTypeEnum.ClientReturn)
                    {
                        OrderStatus = (x.ClientPurchasedItems.Count() == x.ClientPurchasedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered).Count()) ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;
                    }



                }
                OrderStatusStr = (OrderStatus.HasValue) ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), OrderStatus)) : null;

                if (search.OrderStatus != null)
                {
                    if (!search.OrderStatus.Contains(OrderStatus))
                    {
                        continue;
                    }

                }


                var SINumber = "";
                if (x.ClientPurchasedItems?.Count > 0)
                {

                    var stdetail = _context.STSalesDetails.Where(p => p.Id == x.ClientPurchasedItems.Select(z => z.STSalesDetailId).FirstOrDefault()).FirstOrDefault();
                    SINumber = _context.STSales.Where(p => p.Id == stdetail.STSalesId).ToList().Select(z => z.SINumber).FirstOrDefault();
                }

                var obj = new
                {
                    x.Id,
                    x.TransactionNo,
                    x.ReturnFormNumber,
                    x.ReturnType,
                    x.ClientReturnType,
                    ReturnDrNumber = x.ReturnDRNumber,
                    ApproveDeliveryDate = x.ApprovedDeliveryDate,
                    ReturnTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), x.ReturnType)),
                    ReturnedBy = (x.ReturnType == ReturnTypeEnum.RTV || x.ReturnType == ReturnTypeEnum.Breakage)
                                 ? x.Store.Name
                                 : _context.STSales.Where(p => p.Id == x.STSalesId && p.StoreId == x.StoreId).FirstOrDefault().ClientName,
                    ReturnedTo = (x.ReturnType == ReturnTypeEnum.RTV || x.ReturnType == ReturnTypeEnum.Breakage)
                                 ? x.Warehouse.Name
                                 : x.Store.Name,
                    RequestDate = x.DateCreated,
                    x.Remarks,
                    Items = (x.ReturnType == ReturnTypeEnum.RTV || x.ReturnType == ReturnTypeEnum.Breakage)
                            ?
                            x.PurchasedItems.Select(p => new
                            {
                                serialNumber = p.Item.SerialNumber,
                                itemCode = p.Item.Code,
                                ItemName = p.Item.Name,
                                SizeName = p.Item.Size.Name,
                                p.Item.Tonality,
                                p.BrokenQuantity,
                                p.GoodQuantity,
                                p.ActualBrokenQuantity,
                                ReturnReasonStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnReasonEnum), p.ReturnReason)),
                                p.Remarks,
                                totalQty = p.BrokenQuantity + p.GoodQuantity,
                                p.DeliveryStatus,
                                p.isTonalityAny
                            })
                            :
                            null,
                    ClientPurchasedItems = (x.ReturnType == ReturnTypeEnum.ClientReturn)
                                           ?
                                            x.ClientPurchasedItems.Select(p => new
                                            {
                                                serialNumber = p.Item.SerialNumber,
                                                itemCode = p.Item.Code,
                                                ItemName = p.Item.Name,
                                                SizeName = p.Item.Size.Name,
                                                p.Item.Tonality,
                                                p.Quantity,
                                                ReturnReasonStr = (p.ReturnReason.HasValue) ? EnumExtensions.SplitName(Enum.GetName(typeof(ReturnReasonEnum), p.ReturnReason)) : null,
                                                p.Remarks,
                                                p.isTonalityAny
                                            })
                                            : null,
                    x.RequestStatus,
                    RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), x.RequestStatus)),
                    OrderStatus,
                    OrderStatusStr,
                    TestStatus = x.OrderStatus,
                    TestStatusStr = (x.OrderStatus.HasValue) ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), x.OrderStatus)) : null,
                    SINumber
                };


                //check if the deliveries is already delivered before displaying in Approve returns main
                if (obj.ReturnType == ReturnTypeEnum.Breakage && search.mainList == true)
                {
                    var delivered = 0;
                    foreach (var item in obj.Items)
                    {
                        if (item.DeliveryStatus == DeliveryStatusEnum.Delivered)
                        {
                            delivered++;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (delivered > 0)
                    {
                        retList.Add(obj);
                    }

                }
                else
                {
                    retList.Add(obj);
                }


            }

            return retList;
        }


        public STReturn GetPurchaseReturnBy(int id)
        {
            var record = _context.STReturns
                                 .Include(p => p.PurchasedItems)
                                 .Where
                                 (p => p.Id == id
                                       && p.RequestStatus == RequestStatusEnum.Pending
                                       && (p.ReturnType == ReturnTypeEnum.RTV
                                       || p.ReturnType == ReturnTypeEnum.Breakage)
                                 )
                                 .FirstOrDefault();

            return record;
        }

        public void ApprovePurchaseReturn(STReturn param)
        {

            param.DateUpdated = DateTime.Now;

            if (param.RequestStatus != RequestStatusEnum.Cancelled)
            {
                param.RequestStatus = RequestStatusEnum.Approved;
            }
            else
            {
                param.OrderStatus = OrderStatusEnum.Cancelled;
            }

            var stStock = _context.STStocks;
            var adjustment = 0;

            foreach (var returnItem in param.PurchasedItems)
            {

                returnItem.DateUpdated = DateTime.Now;


                if (param.ReturnType == ReturnTypeEnum.Breakage)
                {
                    //update last created stock for the item to trigger update of inventory
                    var stock = stStock.Where(p => p.ItemId == returnItem.ItemId && p.StoreId == param.StoreId).Last();
                    if (stock != null)
                    {
                      
                        if (returnItem.BrokenQuantity != returnItem.ActualBrokenQuantity && returnItem.ActualBrokenQuantity != null)
                        {
                            adjustment = returnItem.BrokenQuantity.Value - returnItem.ActualBrokenQuantity.Value;
                        }
                  

                    }

                    //if (adjustment != 0)
                    //{
                    //    var new_stock = new STStock();
                    //    if (new_stock != null)
                    //    {
                    //        new_stock.ItemId = stock.ItemId;
                    //        new_stock.DateCreated = DateTime.Now;
                    //        new_stock.OnHand = adjustment;
                    //        new_stock.DeliveryStatus = DeliveryStatusEnum.Delivered;
                    //        new_stock.ReleaseStatus = ReleaseStatusEnum.Released;
                    //        new_stock.WHDeliveryDetailId = stock.WHDeliveryDetailId;
                    //        new_stock.StoreId = stock.StoreId;
                    //        _context.STStocks.Add(new_stock);

                    //    }
                    //}


                  
                }
                else
                {
                    returnItem.DeliveryStatus = DeliveryStatusEnum.Waiting;
                    returnItem.ReleaseStatus = ReleaseStatusEnum.Waiting;
                    var stock = stStock.Where(p => p.ItemId == returnItem.ItemId && p.StoreId == param.StoreId).Last();
                    if (stock != null)
                    {
                        //stock.DateCreated = DateTime.Now;
                        stock.ChangeDate = DateTime.Now;
                        // removed causing auto marking as delivered for orders
                        //stock.DeliveryStatus = DeliveryStatusEnum.Delivered;
                        _context.STStocks.Update(stock);

                    }
                }

            }

            if(param.ReturnType == ReturnTypeEnum.Breakage)
            {
                param.OrderStatus = OrderStatusEnum.Completed;
            }

            _context.STReturns.Update(param);
            _context.SaveChanges();
        }


        public object GetMainSelectedStoreRTV(SearchReturns search, AppSettings appSettings)
        {
            // To lessen the searching time of item details
            //var itemDetails = new Item();
            //var cachedItems = new List<Item>();

            var retList = new List<object>();
            //var items = _context.Items.AsNoTracking()
            //                    .Include(p => p.Size).ToList();

            // Get only the RTV deliveries that are already received by store
            IQueryable<STReturn> records = _context.STReturns.AsNoTracking()
                        .Include(p => p.Deliveries)
                            .ThenInclude(p => p.WarehouseDeliveries)
                                //.ThenInclude(p => p.Item)
                            .Where(p => ((p.ReturnType != ReturnTypeEnum.ClientReturn
                                            && p.RequestStatus == RequestStatusEnum.Approved))
                                            && p.Deliveries.Where(x =>
                                                    x.WarehouseDeliveries.Where(y =>
                                                                   y.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                                && y.ReleaseStatus == ReleaseStatusEnum.Released).Count() > 0
                                                                  ).Count() > 0);




            records = records.Where(p => p.Deliveries.Any(x => x.WarehouseDeliveries.Any(c => c.DeliveryStatus == DeliveryStatusEnum.Delivered)));

            records = records.Where(p => p.Deliveries.Any(x => x.WarehouseDeliveries.Where(c => c.DeliveryStatus == DeliveryStatusEnum.Delivered && c.ReleaseStatus == ReleaseStatusEnum.Released).Count() > 0));

            if (search.StoreId.HasValue)
            {
                records = records.Where(p => p.StoreId == search.StoreId);
            }

            if (!string.IsNullOrEmpty(search.ReturnFormNumber))
            {
                records = records.Where(p => p.ReturnFormNumber.ToLower() == search.ReturnFormNumber.ToLower());
            }

            if(!string.IsNullOrEmpty(search.DRNumber))
            {
                records = records.Where(p => p.Deliveries.Any(x => x.DRNumber.ToLower() == search.DRNumber.ToLower()));
            }

            if (search.RequestDateFrom.HasValue)
            {
                records = records.Where(p => p.Deliveries.Any(x => search.RequestDateFrom.Value <= x.DeliveryDate));

            }
            if (search.RequestDateTo.HasValue)
            {
                records = records.Where(p => p.Deliveries.Any(x => search.RequestDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59) >= x.DeliveryDate));
   
            }
            records = records.OrderByDescending(p => p.Id);

            //if (!string.IsNullOrEmpty(search.Code))
            //{
            //    records = records.Where(p => p.Deliveries.Any(x => x.WarehouseDeliveries.Any(c => c.Item.Code.ToLower().Contains(search.Code.ToLower())))).ToList();
            //}

            // Select Delivery details to be displayed
            var deliveries = records.SelectMany(p => p.Deliveries.SelectMany(x => x.WarehouseDeliveries))
                                        .Include(p => p.Item)
                                            .ThenInclude(p => p.Size)
                                                .ToList();

            if (!string.IsNullOrEmpty(search.Code))
            {
                deliveries = deliveries.Where(p => p.Item.Code.ToLower().Contains(search.Code.ToLower())).ToList();
            }


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
                            .Take(appSettings.RecordDisplayPerPage).ToList();



            }
            else
            {
                response = new GetAllResponse(deliveries.Count());
            }

            


            //var sizes = _context.Sizes.AsNoTracking();

            for (int i = 0; i < deliveries.Count(); i++)
            {
                // Return details
                var stRet = records.Where(p => p.Deliveries.Any(x => x.Id == deliveries[i].WHDeliveryId)).FirstOrDefault();
                //WhDelivery data
                var whDel = stRet.Deliveries.Where(p => p.Id == deliveries[i].WHDeliveryId).FirstOrDefault();

                var obj = new
                {
                    stRet.TransactionNo,
                    stRet.ReturnFormNumber,
                    whDel.DRNumber,
                    deliveries[i].ItemId,
                    code = deliveries[i].Item.Code,
                    Serialnumber = deliveries[i].Item.SerialNumber,
                    SizeName = deliveries[i].Item.Size.Name,
                    deliveries[i].Quantity,
                    deliveries[i].ReceivedBrokenQuantity,
                    deliveries[i].ReceivedGoodQuantity,
                    whDel.DeliveryDate,
                    whDel.ReleaseDate
                };
                retList.Add(obj);

            }




            //for (int i = 0; i < records.Count(); i++)
            //{
            //    var deliveries = records[i].Deliveries.ToList();

            //    for (int x = 0; x < deliveries.Count(); x++)
            //    {
            //        // Searching by DRNumber
            //        if (!string.IsNullOrEmpty(search.DRNumber))
            //        {
            //            if (deliveries[x].DRNumber.ToLower() != search.DRNumber.ToLower())
            //            {
            //                continue;
            //            }
            //        }

            //        // Searching by Dates [Conditions are inverted - Because IF it success proceed adding on the list ELSE  looping will continue on next index ]
            //        if (search.RequestDateFrom.HasValue)
            //        {
            //            if (!(search.RequestDateFrom.Value <= deliveries[x].DeliveryDate))
            //            {
            //                continue;
            //            }
            //        }
            //        if (search.RequestDateTo.HasValue)
            //        {
            //            if (!(search.RequestDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59) >= deliveries[x].DeliveryDate))
            //            {
            //                continue;
            //            }
            //        }

            //        var deliveryDetails = deliveries[x].WarehouseDeliveries.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered && p.ReleaseStatus == ReleaseStatusEnum.Released).ToList();
            //        // Check if Warehouse has already rceived item from this return order
            //        if (deliveryDetails.Count() == 0)
            //        {
            //            continue;
            //        }


            //        for (int y = 0; y < deliveryDetails.Count(); y++)
            //        {
            //            // Lessen the searching time of item.
            //            if (cachedItems.Where(p => p.Id == deliveryDetails[y].ItemId).Count() == 0)
            //            {
            //                itemDetails = items.Where(p => p.Id == deliveryDetails[y].ItemId).FirstOrDefault();
            //                cachedItems.Add(itemDetails);
            //            }
            //            else
            //            {
            //                itemDetails = cachedItems.Where(p => p.Id == deliveryDetails[y].ItemId).FirstOrDefault();
            //            }

            //            //if (!string.IsNullOrEmpty(search.Code))
            //            //{
            //            //    if (!itemDetails.Code.ToLower().Contains(search.Code.ToLower()))
            //            //    {
            //            //        continue;
            //            //    }
            //            //}


            //            var obj = new
            //            {
            //                records[i].TransactionNo,
            //                records[i].ReturnFormNumber,
            //                deliveries[x].DRNumber,
            //                deliveryDetails[y].ItemId,
            //                code = itemDetails.Code,
            //                Serialnumber = itemDetails.SerialNumber,
            //                SizeName = itemDetails.Size.Name,
            //                deliveryDetails[y].Quantity,
            //                deliveryDetails[y].ReceivedBrokenQuantity,
            //                deliveryDetails[y].ReceivedGoodQuantity,
            //                deliveries[x].DeliveryDate,
            //                deliveries[x].ReleaseDate
            //            };
            //            retList.Add(obj);
            //        }
            //    }
            //}

            response.List.AddRange(retList);

            return response;
        }




        public object GetMainSelectedStoreRTV2(SearchReturns search, AppSettings appSettings)
        {
            // To lessen the searching time of item details
            //var itemDetails = new Item();
            //var cachedItems = new List<Item>();

            var retList = new List<object>();
            //var items = _context.Items.AsNoTracking()
            //                    .Include(p => p.Size).ToList();

            // Get only the RTV deliveries that are already received by store
            IQueryable<STReturn> records = _context.STReturns.AsNoTracking()
                        .Include(p => p.Deliveries)
                            .ThenInclude(p => p.WarehouseDeliveries)
                                .ThenInclude(p => p.Item)
                                    .ThenInclude(p => p.Size)
                            .Where(p => ((p.ReturnType != ReturnTypeEnum.ClientReturn
                                            && p.RequestStatus == RequestStatusEnum.Approved))
                                            && p.Deliveries.Where(x =>
                                                    x.WarehouseDeliveries.Where(y =>
                                                                   y.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                                && y.ReleaseStatus == ReleaseStatusEnum.Released).Count() > 0
                                                                  ).Count() > 0);




            records = records.Where(p => p.Deliveries.Any(x => x.WarehouseDeliveries.Any(c => c.DeliveryStatus == DeliveryStatusEnum.Delivered)));

            records = records.Where(p => p.Deliveries.Any(x => x.WarehouseDeliveries.Where(c => c.DeliveryStatus == DeliveryStatusEnum.Delivered && c.ReleaseStatus == ReleaseStatusEnum.Released).Count() > 0));

            if (search.StoreId.HasValue)
            {
                records = records.Where(p => p.StoreId == search.StoreId);
            }

            if (!string.IsNullOrEmpty(search.ReturnFormNumber))
            {
                records = records.Where(p => p.ReturnFormNumber.ToLower() == search.ReturnFormNumber.ToLower());
            }

            if (!string.IsNullOrEmpty(search.DRNumber))
            {
                records = records.Where(p => p.Deliveries.Any(x => x.DRNumber.ToLower() == search.DRNumber.ToLower()));
            }

            if (search.RequestDateFrom.HasValue)
            {
                records = records.Where(p => p.Deliveries.Any(x => search.RequestDateFrom.Value <= x.DeliveryDate));

            }
            if (search.RequestDateTo.HasValue)
            {
                records = records.Where(p => p.Deliveries.Any(x => search.RequestDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59) >= x.DeliveryDate));

            }
            records = records.OrderByDescending(p => p.Id);

            //if (!string.IsNullOrEmpty(search.Code))
            //{
            //    records = records.Where(p => p.Deliveries.Any(x => x.WarehouseDeliveries.Any(c => c.Item.Code.ToLower().Contains(search.Code.ToLower())))).ToList();
            //}

            // Select Delivery details to be displayed
            var deliveries = records.SelectMany(p => p.Deliveries.SelectMany(x => x.WarehouseDeliveries))
                                    .Include(p => p.Item)
                                    .ThenInclude(p => p.Size);

            if (!string.IsNullOrEmpty(search.Code))
            {
                deliveries = deliveries.Where(p => p.Item.Code.ToLower().Contains(search.Code.ToLower()))
                                        .Include(p => p.Item)
                                        .ThenInclude(p => p.Size);
            }


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
                            .Take(appSettings.RecordDisplayPerPage)
                                        .Include(p => p.Item)
                                        .ThenInclude(p => p.Size); ;



            }
            else
            {
                response = new GetAllResponse(deliveries.Count());
            }




            //var sizes = _context.Sizes.AsNoTracking();

            foreach(var delivery in deliveries)
            {
                // Return details
                var stRet = records.Where(p => p.Deliveries.Any(x => x.Id == delivery.WHDeliveryId)).FirstOrDefault();
                //WhDelivery data
                var whDel = stRet.Deliveries.Where(p => p.Id == delivery.WHDeliveryId).FirstOrDefault();

                var obj = new
                {
                    stRet.TransactionNo,
                    stRet.ReturnFormNumber,
                    whDel.DRNumber,
                    delivery.ItemId,
                    code = delivery.Item.Code,
                    Serialnumber = delivery.Item.SerialNumber,
                    SizeName = delivery.Item.Size.Name,
                    delivery.Quantity,
                    delivery.ReceivedBrokenQuantity,
                    delivery.ReceivedGoodQuantity,
                    whDel.DeliveryDate,
                    whDel.ReleaseDate
                };
                retList.Add(obj);

            }

   

            response.List.AddRange(retList);

            return response;
        }

    }
}

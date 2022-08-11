using FC.Api.DTOs.Store.BranchOrders;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Views;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Services.Stores
{
    public class BranchOrdersService : IBranchOrdersService
    {

        private DataContext _context;

        public BranchOrdersService(DataContext context)
        {
            _context = context;
        }
        
        public DataContext DataContext()
        {
            return this._context;
        }

        public IEnumerable<object> GetAllBranchOrders(Search search, ISTStockService stockService)
        {
            IQueryable<STOrder> query = _context.STOrders.Where(p => p.OrderToStoreId == search.OrderToStoreId
                                                            && p.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                            && p.RequestStatus == RequestStatusEnum.Approved).OrderByDescending(p => p.Id)
                                                            .Include(p => p.Deliveries)
                                                                .ThenInclude(p => p.ClientDeliveries)
                                                             .Include(p => p.OrderedItems);

            if (search.OrderedBy.HasValue)
            {
                query = query.Where(p => p.StoreId == search.OrderedBy);
            }

            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            if (search.PODateFrom.HasValue)
            {
                query = query.Where(p => search.PODateFrom.Value <= p.PODate);
            }

            if (search.PODateTo.HasValue)
            {
                query = query.Where(p => search.PODateTo.Value >= p.PODate);
            }

            stockService.stStock = _context.STStocks;
            stockService.stSales = _context.STSales;

            var records = from x in query
                          select new
                          {
                              x.Id,
                              OrderedItems = x.OrderedItems
                                              .Select(p => new
                                              {
                                                  p.Id,
                                                  p.STOrderId,
                                                  p.ItemId,
                                                  p.Item.SerialNumber,
                                                  p.Item.Code,
                                                  ItemName = p.Item.Name,
                                                  SizeName = p.Item.Size.Name,
                                                  p.Item.Tonality,
                                                  p.RequestedQuantity,
                                                  p.RequestedRemarks,
                                                  p.ApprovedQuantity,
                                                  p.ApprovedRemarks,
                                                  Available = stockService.GetItemAvailableQuantity(p.ItemId, x.OrderToStoreId, true),
                                                  p.DeliveryStatus
                                              })
                          };

            var retList = new List<object>();

            var stShowroom = _context.STShowroomDeliveries;
            var stClient = _context.STClientDeliveries;

            foreach (var detail in query)
            {
                var orderByDetails = _context.Stores.Where(p => p.Id == detail.StoreId).FirstOrDefault();
                var orderToDetails = _context.Stores.Where(p => p.Id == detail.OrderToStoreId).FirstOrDefault();
                var totalDeliveredQty = 0;

                //Get total quantity of delivered items
                foreach (var items in detail.OrderedItems)
                {
                     totalDeliveredQty += Convert.ToInt32(
                                            stShowroom
                                                .Where(p => p.STOrderDetailId == items.Id
                                                           && p.ItemId == items.ItemId
                                                           && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                           && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                .Sum(p => p.Quantity)
                                        );

                    //if(detail.DeliveryType != DeliveryTypeEnum.Delivery)
                    //{
                        totalDeliveredQty += Convert.ToInt32(
                                            stClient
                                                .Where(p => p.STOrderDetailId == items.Id
                                                           && p.ItemId == items.ItemId
                                                           && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                           && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                .Sum(p => p.Quantity)
                                        );
                    //}
                    if(detail.DeliveryType == DeliveryTypeEnum.Pickup)
                    {
                        totalDeliveredQty += Convert.ToInt32(
                        _context.STOrderDetails.Where(p => p.STOrderId == items.STOrderId
                                                                && p.ItemId == items.ItemId
                                                                && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                                && p.ReleaseStatus == ReleaseStatusEnum.Released
                                                                )
                                                                .Sum(p => p.ApprovedQuantity)
                                                                );
                    }
                    
                    

                }

                //if (detail.DeliveryType == DeliveryTypeEnum.Delivery)
                //{
                //    detail.Deliveries.Where(p => p.Delivered == DeliveryStatusEnum.Delivered).Sum(x => x.ClientDeliveries.Sum(y => y.Quantity));
                //}
                //get total quantity of approved items
                var totalItemQty = detail.OrderedItems.Sum(p => p.ApprovedQuantity);
                DeliveryStatusEnum delStatus;


                //set delivery status
                if(totalItemQty == totalDeliveredQty)
                {
                    delStatus = DeliveryStatusEnum.Delivered;
                }
                else
                {
                    delStatus = DeliveryStatusEnum.Waiting;
                    
                }
                

                var obj = new
                {
                    detail.Id,
                    detail.TransactionNo,
                    detail.TransactionType,
                    DeliveryStatus = delStatus,
                    //DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum),
                    //                        records.Where(p => p.Id == detail.Id).Select(p => p.OrderedItems.Select(x => x.DeliveryStatus).FirstOrDefault()).FirstOrDefault())),
                    DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum),delStatus)),
                    TransactionTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), detail.TransactionType)),
                    detail.RequestStatus,
                    RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), detail.RequestStatus)),
                    detail.PONumber,
                    detail.PODate,
                    detail.DeliveryType,
                    DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), detail.DeliveryType)),
                    OrderedBy = _context.Stores.Where(p => p.Id == detail.StoreId).Select(p => p.Name).FirstOrDefault(),
                    OrderedTo = _context.Stores.Where(p => p.Id == detail.OrderToStoreId).Select(p => p.Name).FirstOrDefault(),
                    detail.PaymentMode,
                    PaymentModeStr = EnumExtensions.SplitName(Enum.GetName(typeof(PaymentModeEnum), detail.PaymentMode)),
                    detail.SalesAgent,
                    detail.ClientName,
                    detail.Address1,
                    detail.Address2,
                    detail.Address3,
                    detail.Remarks,
                    detail.ORNumber,
                    detail.SINumber,
                    detail.WHDRNumber,
                    IsInterbranch = (orderByDetails.CompanyId == orderToDetails.CompanyId)
                                                 ? true : false,
                    StoreCompanyRelation = (orderByDetails.CompanyId == orderToDetails.CompanyId)
                                                 ? StoreCompanyRelationEnum.InterBranch : StoreCompanyRelationEnum.InterCompany,
                    StoreCompanyRelationStr = (orderByDetails.CompanyId == orderToDetails.CompanyId)
                                                 ? EnumExtensions.SplitName(Enum.GetName(typeof(StoreCompanyRelationEnum), StoreCompanyRelationEnum.InterBranch)) : EnumExtensions.SplitName(Enum.GetName(typeof(StoreCompanyRelationEnum), StoreCompanyRelationEnum.InterCompany)),

                    OrderedItems = records.Where(p => p.Id == detail.Id).Select(p => p.OrderedItems).FirstOrDefault()
                };

                // Filtering for InterBranch or InterCompany
                if (search.StoreCompanyRelation != null)
                {
                    if (search.StoreCompanyRelation.Contains(obj.StoreCompanyRelation))
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


        public object GetAllBranchOrdersPaged(Search search, ISTStockService stockService, AppSettings appSettings)
        {
            // Getting records using sql view BranchOrderView
            IQueryable<BranchOrderView> query = _context.BranchOrderViews
                                                            .FromSql("SELECT * FROM BranchOrderView WHERE OrderToStoreId = {0}", search.OrderToStoreId);

            if(search.OrderedBy.HasValue)
            {
                query = query.Where(p => p.StoreId == search.OrderedBy);
            }

            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            if (search.PODateFrom.HasValue)
            {
                query = query.Where(p => search.PODateFrom.Value <= p.PODate);
            }

            if (search.PODateTo.HasValue)
            {
                query = query.Where(p => search.PODateTo.Value >= p.PODate);
            }

            if(search.StoreCompanyRelation != null)
            {
                query = query.Where(p => search.StoreCompanyRelation.Contains(p.StoreCompanyRelation));
            }

            if(search.RequestStatus != null)
            {
                query = query.Where(p => search.RequestStatus.Contains(p.RequestStatus));
            }

            stockService.stStock = _context.STStocks;
            stockService.stSales = _context.STSales;

            // executing query to avoid error on implementing pagination
            var records = query.ToList();

            records = records.OrderByDescending(p => p.Id).ToList();

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





        

            var retList = new List<object>();

            // selecting the id of records will be used as criteria on searching order details
            var recordId = records.Select(p => p.Id);

            var orderedItems = _context.STOrderDetails
                                            .Include(p => p.Item)
                                                .ThenInclude(p => p.Size)
                                                    .AsNoTracking()
                                                      .Where(p => recordId.Contains(p.STOrderId)).ToList();


            foreach (var detail in records)
            {
                // Selecting order details base on id
               var orderList = orderedItems
                                    .Where(p => p.STOrderId == detail.Id)
                                        .Select(p => new {

                                                               p.Id,
                                                               p.STOrderId,
                                                               p.ItemId,
                                                               p.Item.SerialNumber,
                                                               p.Item.Code,
                                                               ItemName = p.Item.Name,
                                                               SizeName = p.Item.Size.Name,
                                                               p.Item.Tonality,
                                                               p.RequestedQuantity,
                                                               p.RequestedRemarks,
                                                               p.ApprovedQuantity,
                                                               p.ApprovedRemarks,
                                                               Available = stockService.GetItemAvailableQuantity(p.ItemId, detail.OrderToStoreId, true),
                                                               p.DeliveryStatus
                                                           }).OrderByDescending(p => p.Id);



                var obj = new
                {
                    detail.Id,
                    detail.TransactionNo,
                    detail.TransactionType,
                    detail.TransactionTypeStr,
                    detail.RequestStatus,
                    detail.RequestStatusStr,
                    detail.PONumber,
                    detail.PODate,
                    detail.DeliveryType,
                    detail.DeliveryTypeStr,
                    detail.OrderedBy,
                    detail.OrderedTo,
                    detail.PaymentMode,
                    detail.PaymentModeStr,
                    detail.SalesAgent,
                    detail.ClientName,
                    detail.Address1,
                    detail.Address2,
                    detail.Address3,
                    detail.Remarks,
                    detail.ORNumber,
                    detail.SINumber,
                    detail.WHDRNumber,
                    detail.IsInterbranch,
                    detail.StoreCompanyRelation,
                    detail.StoreCompanyRelationStr,
                    OrderedItems = orderList
                 };




                retList.Add(obj);
              
            }

            response.List.AddRange(retList);

            return response;
        }

        public void UpdateTransferRequest(UpdateTransferRequestDTO dto, ISTSalesService salesService)
        {
          
            var order = _context.STOrders.Include(p => p.OrderedItems).Where(p => p.Id == dto.Id && p.OrderToStoreId == dto.OrderToStoreId).FirstOrDefault();
            if (order != null)
            {
                var storeRequestFrom = _context.Stores.Where(p => p.Id == order.StoreId).FirstOrDefault();
                var storeRequestTo = _context.Stores.Where(p => p.Id == order.OrderToStoreId).FirstOrDefault();
                if (storeRequestFrom != null && storeRequestTo != null)
                {
                    if (storeRequestFrom.CompanyId == storeRequestTo.CompanyId)
                    {
                        order.ORNumber = dto.ORNumber;
                    }
                    else
                    {
                        order.SINumber = dto.SINumber;
                        order.WHDRNumber = dto.WHDRNumber;
                    }

                    order.DateUpdated = DateTime.Now;
                    var stStocks = _context.STStocks;

                    foreach(var items in order.OrderedItems)
                    {
                        var stock = _context.STStocks.Where(p => p.ItemId == items.ItemId && p.StoreId == order.OrderToStoreId).Last();

                        if (stock != null)
                        {
                            stock.ChangeDate = DateTime.Now;

                            _context.STStocks.Update(stock);

                        }
                    }
                 
                    

                    _context.STOrders.Update(order);
                    _context.SaveChanges();
                    
                }

                
            }

        }


    }
}

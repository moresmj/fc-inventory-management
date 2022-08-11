using FC.Api.DTOs.Store;
using FC.Api.DTOs.Store.BranchOrders;
using FC.Api.DTOs.Store.Transfers;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Views;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FC.Api.Services.Stores
{
    public class STTransferService : ISTTransferService
    {

        private DataContext _context;

        public STTransferService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return _context;
        }

        public IEnumerable<object> GetAllForTransferApproval(SearchTransfers search, ISTStockService stockService)
        {

            IQueryable<STOrder> query = _context.STOrders
                                                .Where(p => p.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                                                .OrderByDescending(p => p.Id).OrderByDescending(p => p.Id);


            if (search.StoreId.HasValue)
            {
                query = query.Where(p => p.StoreId == search.StoreId);
            }

            if (search.OrderToStoreId.HasValue)
            {
                query = query.Where(p => p.OrderToStoreId == search.OrderToStoreId);
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

            if (search.RequestStatus != null)
            {
                query = query.Where(p => search.RequestStatus.Contains(p.RequestStatus));
            }
            stockService.stStock = _context.STStocks.AsNoTracking();
            stockService.stSales = _context.STSales.AsNoTracking();
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
                                                  Available = stockService.GetItemAvailableQuantity(p.ItemId, x.OrderToStoreId, true)
                                              })
                          };

            var retList = new List<object>();

            foreach (var detail in query)
            {
                var orderByDetails = _context.Stores.Where(p => p.Id == detail.StoreId).FirstOrDefault();
                var orderToDetails = _context.Stores.Where(p => p.Id == detail.OrderToStoreId).FirstOrDefault();

                var obj = new
                {
                    detail.Id,
                    detail.TransactionNo,
                    detail.TransactionType,
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


        public object GetForTransferApprovalPaged2(SearchTransfers search, ISTStockService stockService, AppSettings appSettings)
        {
            // Uses view query to select transfer for approval
            IQueryable<ApproveTransferView> query = _context.ApproveTransferView.FromSql("SELECT * FROM ApproveTransferView");

            if (search.StoreId.HasValue)
            {
                query = query.Where(p => p.StoreId == search.StoreId);
            }

            if (search.OrderToStoreId.HasValue)
            {
                query = query.Where(p => p.OrderToStoreId == search.OrderToStoreId);
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

            if (search.RequestStatus != null)
            {
                query = query.Where(p => search.RequestStatus.Contains(p.RequestStatus));
            }

            if(search.StoreCompanyRelation != null)
            {
                query = query.Where(p => search.StoreCompanyRelation.Contains(p.StoreCompanyRelation));
            }

            if (search.PaymentMode.HasValue)
            {
                query = query.Where(p => p.PaymentMode == search.PaymentMode);
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

            
            var retList = new List<object>();

            // Getting All the id to select orderDetails
            var orderId = query.Select(p => p.Id).ToList();
            var orderDetails = _context.STOrderDetails.Where(p => orderId.Contains(p.STOrderId)).AsNoTracking()
                                                        .Include(p => p.Item)
                                                          .ThenInclude(p => p.Size).ToList();

            var stores = _context.Stores.AsNoTracking();

            foreach (var detail in query)
            {
                var ordItems = orderDetails.Where(p => p.STOrderId == detail.Id).Select(p => new
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
                    p.isTonalityAny,
                    Available = stockService.GetItemAvailableQuantity(p.ItemId, detail.OrderToStoreId, true)
                }).OrderByDescending(p => p.Id).ToList();

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
                    OrderedBy = stores.Where(p => p.Id == detail.StoreId).Select(p => p.Name).FirstOrDefault(),
                    OrderedTo = stores.Where(p => p.Id == detail.OrderToStoreId).Select(p => p.Name).FirstOrDefault(),
                    detail.PaymentMode,
                    detail.PaymentModeStr,
                    detail.SalesAgent,
                    detail.ClientName,
                    detail.Address,
                    detail.Remarks,
                    detail.ORNumber,
                    detail.SINumber,
                    detail.WHDRNumber,
                    detail.IsInterbranch,
                    detail.StoreCompanyRelationStr,

                    OrderedItems =  ordItems/*.OrderByDescending(p => p.Id)*/
                };


                retList.Add(obj);
            }


            response.List.AddRange(retList);

            return response;
        }

        public object GetForTransferApprovalPaged(SearchTransfers search, ISTStockService stockService, AppSettings appSettings)
        {

            IQueryable<STOrder> query = _context.STOrders
                                                .Where(p => p.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                                                .OrderByDescending(p => p.Id).OrderByDescending(p => p.Id);


            if (search.StoreId.HasValue)
            {
                query = query.Where(p => p.StoreId == search.StoreId);
            }

            if (search.OrderToStoreId.HasValue)
            {
                query = query.Where(p => p.OrderToStoreId == search.OrderToStoreId);
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

            if (search.RequestStatus != null)
            {
                query = query.Where(p => search.RequestStatus.Contains(p.RequestStatus));
            }
            stockService.stStock = _context.STStocks.AsNoTracking();
            stockService.stSales = _context.STSales.AsNoTracking();
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
                                                  Available = stockService.GetItemAvailableQuantity(p.ItemId, x.OrderToStoreId, true)
                                              })
                          };

            var retList = new List<object>();

            foreach (var detail in query)
            {
                var orderByDetails = _context.Stores.Where(p => p.Id == detail.StoreId).FirstOrDefault();
                var orderToDetails = _context.Stores.Where(p => p.Id == detail.OrderToStoreId).FirstOrDefault();

                var obj = new
                {
                    detail.Id,
                    detail.TransactionNo,
                    detail.TransactionType,
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

            GetAllResponse response = null;
            if (search.ShowAll == false)
            {
                response = new GetAllResponse(retList.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;
                }

                retList = retList.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                .Take(appSettings.RecordDisplayPerPage).ToList();
            }
            else
            {
                response = new GetAllResponse(retList.Count());
            }


            response.List.AddRange(retList);

            return response;
        }


        public void ApproveTransferRequestOnMain(ApproveTransferRequestDTO dto, ISTSalesService salesService)
        {
            var order = _context.STOrders.Where(p => p.Id == dto.Id).FirstOrDefault();
            if (order != null)
            {
                var storeRequestFrom = _context.Stores.Where(p => p.Id == order.StoreId).FirstOrDefault();
                var storeRequestTo = _context.Stores.Where(p => p.Id == order.OrderToStoreId).FirstOrDefault();
                if (storeRequestFrom != null && storeRequestTo != null)
                {
                    order.DateUpdated = DateTime.Now;

                    order.RequestStatus = (dto.TransferredItems.Sum(p => p.ApprovedQuantity) > 0) ? RequestStatusEnum.Approved : RequestStatusEnum.Cancelled;

                    order.OrderStatus = (dto.TransferredItems.Sum(p => p.ApprovedQuantity) > 0) ? order.OrderStatus : OrderStatusEnum.Cancelled;

                    _context.STOrders.Update(order);
                    _context.SaveChanges();

                    foreach (var item in dto.TransferredItems)
                    {
                        var orderDetail = _context.STOrderDetails.Where(p => p.Id == item.Id
                                                                             && p.ItemId == item.ItemId
                                                                             && p.STOrderId == order.Id)
                                                                 .FirstOrDefault();
                        if (orderDetail != null)
                        {
                            orderDetail.ApprovedQuantity = item.ApprovedQuantity;
                            orderDetail.ApprovedRemarks = item.ApprovedRemarks;
                            orderDetail.DeliveryStatus = DeliveryStatusEnum.Waiting;
                            orderDetail.ReleaseStatus = ReleaseStatusEnum.Waiting;
                            orderDetail.DateUpdated = DateTime.Now;

                            _context.STOrderDetails.Update(orderDetail);
                            _context.SaveChanges();

                            if (order.DeliveryType == DeliveryTypeEnum.Pickup)
                            {
                                //  Deduct stock to store
                                var deductFromStore = new STStock
                                {
                                    ItemId = item.ItemId,
                                    OnHand = -item.ApprovedQuantity,
                                    STOrderDetailId = orderDetail.Id,
                                    StoreId = order.OrderToStoreId,
                                    DeliveryStatus = DeliveryStatusEnum.Waiting,
                                    ReleaseStatus = ReleaseStatusEnum.Waiting
                                };

                                new STStockService(_context).InsertSTStock(deductFromStore);
                            }
                        }
                    }

                }


            }

        }


        /// <summary>
        /// Insert transfer record
        /// </summary>
        /// <param name="param">STTransfer</param>
        public int? AddTransferOrder(STTransfer param, ISTStockService stockService)
        {
            param.DateCreated = DateTime.Now;

            foreach (var item in param.TransferredItems)
            {
                item.DateCreated = DateTime.Now;
            }

            _context.STTransfers.Add(param);
            _context.SaveChanges();


            return param.Id;

        }


        private IEnumerable<object> GetOrderTo(int? storeId, ICollection<STTransferDetail> transferredItems, ISTStockService stockService)
        {

            return stockService.GetStoreWithItemAvailable(storeId, transferredItems);
            
        }

        public void SaveTransferToOrder(AddTransferOrderDTO dto,ClaimsPrincipal user, AppSettings appSettings)
        {
            var order = new STOrder
            {
                StoreId = dto.StoreId,
                OrderToStoreId = dto.OrderToStoreId,
                TransactionType = TransactionTypeEnum.Transfer,
                DeliveryType = dto.DeliveryType,
                OrderType = OrderTypeEnum.InterbranchOrIntercompanyOrder,
                PONumber = dto.PONumber,
                PODate = dto.PODate,
                PaymentMode = dto.PaymentMode,
                SalesAgent = dto.SalesAgent,
                ClientName = dto.ClientName,
                Address1 = dto.Address1,
                Address2 = dto.Address2,
                Address3 = dto.Address3,
                Remarks = dto.Remarks,
                ContactNumber = dto.ContactNumber,
                RequestStatus = RequestStatusEnum.Pending,
                OrderedItems = new List<STOrderDetail>(),
                STTransferId = dto.TransferId,
                TRNumber  = dto.TRNumber,
                OrderStatus = OrderStatusEnum.Incomplete,

            };

            //  Get items to be transferred
            var itemsToBeTransferred = _context.STTransferDetails.Where(p => p.STTransferId == dto.TransferId);
            if(itemsToBeTransferred != null && itemsToBeTransferred.Count() > 0)
            {
                foreach(var item in itemsToBeTransferred)
                {
                    order.OrderedItems.Add(new STOrderDetail
                    {
                        ItemId = item.ItemId,
                        RequestedQuantity = item.Quantity,
                        ReleaseStatus = ReleaseStatusEnum.Pending
                    });
                }
            }

            new STOrderService(_context).InsertOrderInterBranch(order,user,appSettings);

        }

        public string GetTransactionNumberForMultipleStore()
        {
            var totalRecordCount = Convert.ToInt32(this._context.STOrders.Where(x => x.TransactionType == TransactionTypeEnum.Transfer).Count() + 1).ToString();
            return string.Format("IOTR{0}", totalRecordCount.PadLeft(6, '0'));
        }

    }
}

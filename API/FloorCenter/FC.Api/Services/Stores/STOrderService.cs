using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FC.Api.DTOs;
using FC.Api.DTOs.Store;
using FC.Api.DTOs.Store.BranchOrders;
using FC.Api.DTOs.Warehouse.ModifyTonality;
using FC.Api.Helpers;
using FC.Api.Services.Stores.AdvanceOrder;
using FC.Api.Services.UserTrails;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.UserTrail;
using FC.Core.Domain.Warehouses;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FC.Api.Services.Stores
{
    public class STOrderService : ISTOrderService
    {


        private DataContext _context;

        private List<Store> stores { get; set; }

        public STOrderService(DataContext context)
        {
            _context = context;
            
        }

        public DataContext DataContext()
        {
            return this._context;
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="dto">Search parameters</param>
        /// <returns>STOrders</returns>
        public IEnumerable<STOrder> GetAllOrders(OrderSearch dto)
        {
            IQueryable<STOrder> query = _context.STOrders.Where(p => p.StoreId == dto.StoreId);

            if (dto.TransactionType.HasValue)
            {
                query = query.Where(p => p.TransactionType == dto.TransactionType);
            }

            if (!string.IsNullOrWhiteSpace(dto.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == dto.PONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(dto.transactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == dto.transactionNo.ToLower());
            }

            if (dto.PODateFrom.HasValue)
            {
                query = query.Where(p => dto.PODateFrom.Value <= p.PODate);
            }

            if (dto.PODateTo.HasValue)
            {
                query = query.Where(p => dto.PODateTo.Value >= p.PODate);
            }

            if (dto.RequestStatus != null)
            {
                query = query.Where(p => dto.RequestStatus.Contains(p.RequestStatus));
            }

            return query
                .Include(p => p.Store)
                    .Include(p => p.Warehouse)
                        .Include(p => p.OrderedItems)
                            .ThenInclude(p => p.Item)
                                .ThenInclude(p => p.Size);
        }

        public IEnumerable<object> GetAllOrders2(OrderSearch dto)
        {
            IQueryable<STOrder> query = _context.STOrders
                                            .Include(p => p.Warehouse)
                                            .Include(p => p.Deliveries)
                                            .Include(p => p.OrderedItems)
                                               .ThenInclude(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Where(p => p.StoreId == dto.StoreId).OrderByDescending(p => p.Id);

            if (dto.TransactionType.HasValue)
            {
                query = query.Where(p => p.TransactionType == dto.TransactionType);
            }

            if (!string.IsNullOrWhiteSpace(dto.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == dto.PONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(dto.transactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == dto.transactionNo.ToLower());
            }

            if (dto.PODateFrom.HasValue)
            {
                query = query.Where(p => dto.PODateFrom.Value <= p.PODate);
            }

            if (dto.PODateTo.HasValue)
            {
                query = query.Where(p => dto.PODateTo.Value >= p.PODate);
            }

            if (dto.RequestStatus != null)
            {
                query = query.Where(p => dto.RequestStatus.Contains(p.RequestStatus));
            }


            if (!string.IsNullOrWhiteSpace(dto.Remarks))
            {
               query = query.Where(p => p.Remarks.Contains(dto.Remarks));
            }

          
                

            var records = query.ToList();
            var orders = new List<object>();

            // Load all stores
            stores = _context.Stores.AsNoTracking().ToList();
            var cDeliveries = _context.STClientDeliveries;
            var deliveredStocks = _context.STStocks;

            for (int i = 0; i < records.Count(); i++)
            {
                var record = records[i];
                OrderStatusEnum? OrderStatus = null;
                string OrderStatusStr = null;
                bool ShowDeliveryOrPickUpButton = false;
                bool IsInterbranch = false;

                if (record.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                {
                    var storeCompanyId = stores.Where(p => p.Id == record.StoreId).Select(p => p.CompanyId).FirstOrDefault();
                    var orderToStoreCompanyId = stores.Where(p => p.Id == record.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault();

                    IsInterbranch = (storeCompanyId == orderToStoreCompanyId) ? true : false;

                    ShowDeliveryOrPickUpButton = (IsInterbranch && record.ORNumber != null) ? true
                                                    : (!IsInterbranch && record.SINumber != null && record.WHDRNumber != null) ? true : false;
                }
                

                if (record.Deliveries.Count() != 0)
                {
                    if (record.OrderType != OrderTypeEnum.ShowroomStockOrder && record.DeliveryType == DeliveryTypeEnum.Delivery)
                    {
                        OrderStatus = (record.Deliveries.Where(p => p.Delivered == DeliveryStatusEnum.Delivered).Count() == record.Deliveries.Count())
                            ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;
                        // Check how many is already delivered to determine if the order is completed

                        var deliveryId = record.Deliveries.Select(p => p.Id);

                        int? deliveredQty = 0;
                        foreach (var id in deliveryId)
                        {
                            var clientDelivery = cDeliveries.Where(p => p.STDeliveryId == id);
                            foreach (var delivery in clientDelivery)
                            {
                                deliveredQty += deliveredStocks.Where(p => p.STClientDeliveryId == delivery.Id && p.OnHand > 0 && p.DeliveryStatus == DeliveryStatusEnum.Delivered 
                                && (record.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder ? p.ReleaseStatus == null : p.ReleaseStatus == ReleaseStatusEnum.Released)).Sum(p => p.OnHand);
                            }
                            //deliveredQty += clientDelivery.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered).Sum(p => p.DeliveredQuantity);
                        }


                        OrderStatus = (record.OrderedItems.Sum(p => p.ApprovedQuantity) == deliveredQty) ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;







                    }
                    else
                    {
                        OrderStatus = record.RequestStatus == RequestStatusEnum.Cancelled
                                  ? OrderStatusEnum.Cancelled : (record.OrderedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered && p.ReleaseStatus == ReleaseStatusEnum.Released).Count() == record.OrderedItems.Count())
                                  ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;

                    }
                }
                else if (record.OrderedItems.Count() != 0)
                {
                    OrderStatus = record.RequestStatus == RequestStatusEnum.Cancelled
                            ? OrderStatusEnum.Cancelled : (record.OrderedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered && p.ReleaseStatus == ReleaseStatusEnum.Released).Count() == record.OrderedItems.Count())
                            ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;
                }
                else
                {
                    OrderStatus = OrderStatusEnum.Incomplete;
                }

                // Order Status Filtering
                if (dto.OrderStatus != null)
                {
                    if (!dto.OrderStatus.Contains(OrderStatus))
                    {
                        continue;
                    }
                }

                OrderStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), OrderStatus));

                var advanceOrderId = query.Where(p => p.STAdvanceOrderId.HasValue && p.isAdvanceOrderFlg == true).Select(p => p.STAdvanceOrderId);

                var allocatedOrder = _context.WHAllocateAdvanceOrder
                                       .Include(p => p.AllocateAdvanceOrderDetails)
                                       .Where(p => advanceOrderId.Contains(p.StAdvanceOrderId));

                var advanceOrder = _context.STAdvanceOrder.Include(p => p.AdvanceOrderDetails)
                                                                .Where(p => advanceOrderId.Contains(p.Id)).SelectMany(p => p.AdvanceOrderDetails);

                var stService = new STStockService(_context);
                var stAdvanceSerice = new STAdvanceOrderService(_context);

                var obj = new
                {
                    record.Id,
                    record.TransactionNo,
                    record.TransactionType,
                    TransactionTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), record.TransactionType)),
                    record.OrderType,
                    OrderTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), record.OrderType)),
                    record.RequestStatus,
                    RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), record.RequestStatus)),
                    record.PONumber,
                    record.PODate,
                    record.Remarks,
                    record.DeliveryType,
                    record.ClientName,
                    record.ContactNumber,
                    record.ORNumber,
                    record.SINumber,
                    record.WHDRNumber,
                    record.ClientSINumber,
                    record.isAdvanceOrderFlg,
                    transferType = (record.TransactionType == TransactionTypeEnum.Transfer) ?
                                              (IsInterbranch) ? "InterBranch" : "Intercompany" : null,

                    Vendor = (record.WarehouseId.HasValue) ? _context.Warehouses.Where(p => p.Id == record.WarehouseId).Select(p => p.Vendor).FirstOrDefault() : false,
                    DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), record.DeliveryType)),
                    OrderedTo = (record.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder)
                                          ? _context.Warehouses.Where(p => p.Id == record.WarehouseId).Select(p => p.Name).FirstOrDefault()
                                          : _context.Stores.Where(p => p.Id == record.OrderToStoreId).Select(p => p.Name).FirstOrDefault(),
                    OrderedItems = (record.OrderedItems != null) ?
                            (record.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder ?
                            record.OrderedItems.Select(p => new
                            {
                                p.Id,
                                p.STOrderId,
                                p.RequestedQuantity,
                                p.RequestedRemarks,
                                p.ApprovedQuantity,
                                p.ApprovedRemarks,
                                p.DeliveryStatus,
                                DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus)),
                                p.ItemId,
                                p.Item.SerialNumber,
                                p.Item.Code,
                                ItemName = p.Item.Name,
                                SizeName = p.Item.Size.Name,
                                p.Item.Tonality,
                                advOrderForAllocationQty = stAdvanceSerice.GetAdvanceOrderForAllocationQuantity(
                                                        advanceOrder.Where(c => c.STAdvanceOrderId == record.STAdvanceOrderId
                                                        && (c.ItemId == p.ItemId || c.Code == p.Item.Code || c.sizeId == p.Item.SizeId)).FirstOrDefault(),
                                                        p.ItemId,
                                                        record.STAdvanceOrderId),
                                AllocatedQty = stAdvanceSerice.GetAdvanceOrderAllocatedQuantity(
                                                            advanceOrder.Where(c => c.STAdvanceOrderId == record.STAdvanceOrderId
                                                            && (c.ItemId == p.ItemId || c.Code == p.Item.Code || c.sizeId == p.Item.SizeId)).FirstOrDefault(),
                                                            p.ItemId,
                                                            record.STAdvanceOrderId),
                            }).OrderByDescending(p => p.Id)
                            :
                            record.OrderedItems.Select(p => new
                            {
                                p.Id,
                                p.STOrderId,
                                p.RequestedQuantity,
                                p.RequestedRemarks,
                                p.ApprovedQuantity,
                                p.ApprovedRemarks,
                                p.DeliveryStatus,
                                DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus)),
                                p.ItemId,
                                p.Item.SerialNumber,
                                p.Item.Code,
                                ItemName = p.Item.Name,
                                SizeName = p.Item.Size.Name,
                                p.Item.Tonality,
                                advOrderForAllocationQty = stAdvanceSerice.GetAdvanceOrderForAllocationQuantity(
                                                        advanceOrder.Where(c => c.STAdvanceOrderId == record.STAdvanceOrderId
                                                        && (c.ItemId == p.ItemId || c.Code == p.Item.Code || c.sizeId == p.Item.SizeId)).FirstOrDefault(),
                                                        p.ItemId,
                                                        record.STAdvanceOrderId),
                                AllocatedQty = stAdvanceSerice.GetAdvanceOrderAllocatedQuantity(
                                                            advanceOrder.Where(c => c.STAdvanceOrderId == record.STAdvanceOrderId
                                                            && (c.ItemId == p.ItemId || c.Code == p.Item.Code || c.sizeId == p.Item.SizeId)).FirstOrDefault(),
                                                            p.ItemId,
                                                            record.STAdvanceOrderId),

                            }) )
                            
                            : null,

                    OrderStatus,
                    OrderStatusStr,
                    IsInterbranch,
                    ShowDeliveryOrPickUpButton,
                    TestStatus = record.OrderStatus,
                    TestStatusStr = record.OrderStatus != null ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), record.OrderStatus)) : "",
                };

                orders.Add(obj);
            }


            return orders;
        }



        public object GetAllOrders3(OrderSearch search, AppSettings appSettings)
        {
            IQueryable<STOrder> query = _context.STOrders
                                            .Include(p => p.Warehouse)
                                            .Include(p => p.Deliveries)
                                            .Include(p => p.OrderedItems)
                                               .ThenInclude(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Where(p => p.StoreId == search.StoreId).OrderByDescending(p => p.Id);

            if (search.TransactionType.HasValue)
            {
                query = query.Where(p => p.TransactionType == search.TransactionType);
            }

            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.transactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.transactionNo.ToLower());
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

            if(search.DeliveryType != null)
            {
                query = query.Where(p => search.DeliveryType.Contains(p.DeliveryType));
            }

            if (search.OrderStatus != null)
            {
                query = query.Where(p => search.OrderStatus.Contains(p.OrderStatus));
            }
            //as of 10-26-2020 add to correct filteration
            if (search.filter != null)
            {
                query = query.Where(p =>
                (p.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder || p.DeliveryType != DeliveryTypeEnum.Pickup) &&
                                    (p.ORNumber != null || p.SINumber != null || p.WHDRNumber != null) &&
                                               p.Deliveries.Count() == 0);
            }

            if (!string.IsNullOrWhiteSpace(search.Remarks))
            {
                query = query.Where(p => p.Remarks.Contains(search.Remarks));
            }



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




            var records = query.ToList();
            var orders = new List<object>();

            var advanceOrderId = query.Where(p => p.STAdvanceOrderId.HasValue && p.isAdvanceOrderFlg == true).Select(p => p.STAdvanceOrderId);

            var allocatedOrder = _context.WHAllocateAdvanceOrder
                                   .Include(p => p.AllocateAdvanceOrderDetails)
                                   .Where(p => advanceOrderId.Contains(p.StAdvanceOrderId));

            var advanceOrder = _context.STAdvanceOrder.Include(p => p.AdvanceOrderDetails)
                                                            .Where(p => advanceOrderId.Contains(p.Id)).SelectMany(p => p.AdvanceOrderDetails);

            var stService = new STStockService(_context);
            var stAdvanceSerice = new STAdvanceOrderService(_context);

            // Load all stores
            stores = _context.Stores.AsNoTracking().ToList();
            var cDeliveries = _context.STClientDeliveries;
            var deliveredStocks = _context.STStocks;

            for (int i = 0; i < records.Count(); i++)
            {
                var record = records[i];
                OrderStatusEnum? OrderStatus = null;
                string OrderStatusStr = null;
                bool ShowDeliveryOrPickUpButton = false;
                bool IsInterbranch = false;

                if (record.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                {
                    var storeCompanyId = stores.Where(p => p.Id == record.StoreId).Select(p => p.CompanyId).FirstOrDefault();
                    var orderToStoreCompanyId = stores.Where(p => p.Id == record.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault();

                    IsInterbranch = (storeCompanyId == orderToStoreCompanyId) ? true : false;

                    ShowDeliveryOrPickUpButton = (IsInterbranch && record.ORNumber != null) ? true
                                                    : (!IsInterbranch && record.SINumber != null && record.WHDRNumber != null) ? true : false;
                }


                if (record.Deliveries.Count() != 0)
                {
                    if (record.OrderType != OrderTypeEnum.ShowroomStockOrder && record.DeliveryType == DeliveryTypeEnum.Delivery)
                    {
                        OrderStatus = (record.Deliveries.Where(p => p.Delivered == DeliveryStatusEnum.Delivered).Count() == record.Deliveries.Count())
                            ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;
                        // Check how many is already delivered to determine if the order is completed

                        var deliveryId = record.Deliveries.Select(p => p.Id);

                        int? deliveredQty = 0;
                        foreach (var id in deliveryId)
                        {
                            var clientDelivery = cDeliveries.Where(p => p.STDeliveryId == id);
                            foreach (var delivery in clientDelivery)
                            {
                                deliveredQty += deliveredStocks.Where(p => p.STClientDeliveryId == delivery.Id && p.OnHand > 0 && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                && (record.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder ? p.ReleaseStatus == null : p.ReleaseStatus == ReleaseStatusEnum.Released)).Sum(p => p.OnHand);
                            }
                          
                        }


                        OrderStatus = (record.OrderedItems.Sum(p => p.ApprovedQuantity) == deliveredQty) ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;







                    }
                    else
                    {
                        OrderStatus = record.RequestStatus == RequestStatusEnum.Cancelled
                                  ? OrderStatusEnum.Cancelled : (record.OrderedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered && p.ReleaseStatus == ReleaseStatusEnum.Released).Count() == record.OrderedItems.Count())
                                  ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;

                    }
                }
                else if (record.OrderedItems.Count() != 0)
                {
                    OrderStatus = record.RequestStatus == RequestStatusEnum.Cancelled
                            ? OrderStatusEnum.Cancelled : (record.OrderedItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered && p.ReleaseStatus == ReleaseStatusEnum.Released).Count() == record.OrderedItems.Count())
                            ? OrderStatusEnum.Completed : OrderStatusEnum.Incomplete;
                }
                else
                {
                    OrderStatus = OrderStatusEnum.Incomplete;
                }


                OrderStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), OrderStatus));

                var paymentmode = _context.STAdvanceOrder.Where(p => p.Id == record.STAdvanceOrderId).Select(p => p.PaymentMode).FirstOrDefault();
                var obj = new
                {
                    record.Id,
                    record.StoreId,
                    record.TransactionNo,
                    record.TransactionType,
                    TransactionTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), record.TransactionType)),
                    record.OrderType,
                    OrderTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), record.OrderType)),
                    record.RequestStatus,
                    RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), record.RequestStatus)),
                    record.PONumber,
                    record.PODate,
                    record.Remarks,
                    record.DeliveryType,
                    record.ClientName,
                    record.ContactNumber,
                    record.ORNumber,
                    record.SINumber,
                    record.WHDRNumber,
                    record.ClientSINumber,
                    record.isAdvanceOrderFlg,
                    transferType = (record.TransactionType == TransactionTypeEnum.Transfer) ?
                                              (IsInterbranch) ? "InterBranch" : "Intercompany" : null,

                    Vendor = (record.WarehouseId.HasValue) ? _context.Warehouses.Where(p => p.Id == record.WarehouseId).Select(p => p.Vendor).FirstOrDefault() : false,
                    DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), record.DeliveryType)),
                    //record.PaymentMode,
                    paymentMode = _context.STAdvanceOrder.Where(p => p.Id == record.STAdvanceOrderId).Select(p => p.PaymentMode).FirstOrDefault(),
                    PaymentModeStr = paymentmode != null ?  EnumExtensions.SplitName(Enum.GetName(typeof(PaymentModeEnum), paymentmode)) : null,
                    OrderedTo = (record.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder)
                                          ? _context.Warehouses.Where(p => p.Id == record.WarehouseId).Select(p => p.Name).FirstOrDefault()
                                          : _context.Stores.Where(p => p.Id == record.OrderToStoreId).Select(p => p.Name).FirstOrDefault(),
                    OrderedItems = (record.OrderedItems != null) ?
                            (record.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder ?
                            record.OrderedItems.Select(p => new
                            {
                                p.Id,
                                p.STOrderId,
                                p.RequestedQuantity,
                                p.RequestedRemarks,
                                p.ApprovedQuantity,
                                p.ApprovedRemarks,
                                p.DeliveryStatus,
                                DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus)),
                                p.ItemId,
                                p.Item.SerialNumber,
                                p.Item.Code,
                                ItemName = p.Item.Name,
                                SizeName = p.Item.Size.Name,
                                p.Item.Tonality,
                                p.isTonalityAny,
                                advOrderForAllocationQty = stAdvanceSerice.GetAdvanceOrderForAllocationQuantity(
                                                        advanceOrder.Where(c => c.STAdvanceOrderId == record.STAdvanceOrderId
                                                        && (c.ItemId == p.ItemId || c.Code == p.Item.Code || c.sizeId == p.Item.SizeId)).FirstOrDefault(),
                                                        p.ItemId,
                                                        record.STAdvanceOrderId),
                                AllocatedQty = stAdvanceSerice.GetAdvanceOrderAllocatedQuantity(
                                                            advanceOrder.Where(c => c.STAdvanceOrderId == record.STAdvanceOrderId
                                                            && (c.ItemId == p.ItemId || c.Code == p.Item.Code || c.sizeId == p.Item.SizeId)).FirstOrDefault(),
                                                            p.ItemId,
                                                            record.STAdvanceOrderId),
                            }).OrderByDescending(p => p.Id)
                            :
                            record.OrderedItems.Select(p => new
                            {
                                p.Id,
                                p.STOrderId,
                                p.RequestedQuantity,
                                p.RequestedRemarks,
                                p.ApprovedQuantity,
                                p.ApprovedRemarks,
                                p.DeliveryStatus,
                                DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus)),
                                p.ItemId,
                                p.Item.SerialNumber,
                                p.Item.Code,
                                ItemName = p.Item.Name,
                                SizeName = p.Item.Size.Name,
                                p.Item.Tonality,
                                p.isTonalityAny,
                                advOrderForAllocationQty = stAdvanceSerice.GetAdvanceOrderForAllocationQuantity(
                                                        advanceOrder.Where(c => c.STAdvanceOrderId == record.STAdvanceOrderId
                                                        && (c.ItemId == p.ItemId || c.Code == p.Item.Code || c.sizeId == p.Item.SizeId)).FirstOrDefault(),
                                                        p.ItemId,
                                                        record.STAdvanceOrderId),
                                AllocatedQty = stAdvanceSerice.GetAdvanceOrderAllocatedQuantity(
                                                            advanceOrder.Where(c => c.STAdvanceOrderId == record.STAdvanceOrderId
                                                            && (c.ItemId == p.ItemId || c.Code == p.Item.Code || c.sizeId == p.Item.SizeId)).FirstOrDefault(),
                                                            p.ItemId,
                                                            record.STAdvanceOrderId),
                            }))

                            : null,

                    OrderStatus,
                    OrderStatusStr,
                    IsInterbranch,
                    ShowDeliveryOrPickUpButton,
                    TestStatus = record.OrderStatus,
                    TestStatusStr = record.OrderStatus != null ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), record.OrderStatus)) : "",
                };

                orders.Add(obj);
            }

            response.List.AddRange(orders);

            return response;
        }



        /// <summary>
        /// Get all orders
        /// </summary>
        /// <param name="dto">Search parameters</param>
        /// <returns>STOrders</returns>
        public IEnumerable<STOrder> GetAllOrders(SearchApproveRequests dto)
        {
            IQueryable<STOrder> query = _context.STOrders;


            if (dto.StoreId.HasValue)
            {
                query = query.Where(p => dto.StoreId == p.StoreId);
            }

            if (!string.IsNullOrWhiteSpace(dto.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == dto.PONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(dto.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == dto.TransactionNo.ToLower());
            }

            if (dto.PODateFrom.HasValue)
            {
                query = query.Where(p => dto.PODateFrom.Value <= p.PODate);
            }

            if (dto.PODateTo.HasValue)
            {
                query = query.Where(p => dto.PODateTo.Value >= p.PODate);
            }

            if (dto.RequestStatus != null)
            {
                query = query.Where(p => dto.RequestStatus.Contains(p.RequestStatus));
            }

            if (!string.IsNullOrWhiteSpace(dto.ItemName))
            {
                query = query.Where(x => x.OrderedItems.Where(i => i.Item.Name.Contains(dto.ItemName)).Count() > 0);
            }

            if (dto.TransactionType.HasValue)
            {
                query = query.Where(p => p.TransactionType == dto.TransactionType);
            }

            return query
                    .Include(p => p.Store)
                    .Include(p => p.Warehouse)
                    .Include(p => p.OrderedItems)
                        .ThenInclude(p => p.Item)
                            .ThenInclude(p => p.Size);
        }

        public IEnumerable<object> GetAllOrders2(SearchApproveRequests dto)
        {

            IQueryable<STOrder> query = _context.STOrders
                                                .Include(p => p.Store)
                                                .Include(p => p.Warehouse)
                                                .Include(p => p.OrderedItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                .Where(p => p.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder);


            if (dto.StoreId.HasValue)
            {
                query = query.Where(p => dto.StoreId == p.StoreId);
            }

            if (!string.IsNullOrWhiteSpace(dto.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == dto.PONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(dto.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == dto.TransactionNo.ToLower());
            }

            if (dto.PODateFrom.HasValue)
            {
                query = query.Where(p => dto.PODateFrom.Value <= p.PODate);
            }

            if (dto.PODateTo.HasValue)
            {
                query = query.Where(p => dto.PODateTo.Value >= p.PODate);
            }

            if (dto.RequestStatus != null)
            {
                query = query.Where(p => dto.RequestStatus.Contains(p.RequestStatus));
            }

            if (!string.IsNullOrWhiteSpace(dto.ItemName))
            {
                query = query.Where(x => x.OrderedItems.Where(i => i.Item.Name.Contains(dto.ItemName)).Count() > 0);
            }

            if (dto.TransactionType.HasValue)
            {
                query = query.Where(p => p.TransactionType == dto.TransactionType);
            }

            var service = new WHStockService(_context);
            var stService = new STStockService(_context);
            service.whStock = _context.WHStocks.AsNoTracking();
            service.stOrder = _context.STOrders.AsNoTracking();
            stService.stStock = _context.STStocks.AsNoTracking();
            stService.stSales = _context.STSales.AsNoTracking();

            var records = from x in query
                          select new
                          {
                              x.Id,
                              x.TransactionNo,
                              TransactionTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), x.TransactionType)),
                              x.OrderType,
                              OrderTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), x.OrderType)),
                              x.DeliveryType,
                              DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                              x.RequestStatus,
                              RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), x.RequestStatus)),
                              OrderedBy = x.Store.Name,
                              OrderedTo = x.Warehouse.Name,
                              x.PONumber,
                              x.PODate,
                              OrderedDate = x.PODate,
                              x.ClientName,
                              x.ContactNumber,
                              x.Address1,
                              x.Address2,
                              x.Address3,
                              x.Remarks,
                              x.PaymentMode,
                              x.SalesAgent,
                              PayMentModeStr = (x.PaymentMode != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(PaymentModeEnum), x.PaymentMode)) : null,
                              OrderedItems = x.OrderedItems.Select(p => new
                              {
                                  p.Id,
                                  p.STOrderId,
                                  p.ItemId,
                                  p.Item.SerialNumber,
                                  ItemCode = p.Item.Code,
                                  ItemName = p.Item.Name,
                                  SizeName = p.Item.Size.Name,
                                  p.Item.Tonality,
                                  p.RequestedQuantity,
                                  Available = service.GetItemAvailableQuantity(p.ItemId, x.WarehouseId),
                                  StAvailable = stService.GetItemAvailableQuantity(p.ItemId, x.StoreId, true),
                                  p.ApprovedQuantity,
                                  p.ApprovedRemarks
                              })

                          };

            return records.OrderByDescending(p => p.Id);
        }


        public object GetAllOrders3(SearchApproveRequests search, AppSettings appSettings)
        {
            IQueryable<STOrder> query = _context.STOrders
                                                .Include(p => p.Store)
                                                .Include(p => p.Warehouse)
                                                .Include(p => p.OrderedItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                .Where(p => p.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder && p.isDealerOrder == search.isDealer).OrderByDescending(p => p.Id);

            query = query.Where(p => p.PONumber != null);

            if (search.StoreId.HasValue)
            {
                query = query.Where(p => search.StoreId == p.StoreId);
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

            if (!string.IsNullOrWhiteSpace(search.ItemName))
            {
                query = query.Where(x => x.OrderedItems.Where(i => i.Item.Name.Contains(search.ItemName)).Count() > 0);
            }

            if (search.TransactionType.HasValue)
            {
                query = query.Where(p => p.TransactionType == search.TransactionType);
            }


            if(search.PaymentMode.HasValue)
            {
                query = query.Where(p => p.PaymentMode == search.PaymentMode);
            }


            var service = new WHStockService(_context);
            var stService = new STStockService(_context);
            var stAdvanceSerice = new STAdvanceOrderService(_context);
            //service.whStock = _context.WHStocks.AsNoTracking();
            //service.stOrder = _context.STOrders.AsNoTracking();
            //stService.stStock = _context.STStocks.AsNoTracking();
            //stService.stSales = _context.STSales.AsNoTracking();


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

            var advanceOrderId = query.Where(p => p.STAdvanceOrderId.HasValue && p.isAdvanceOrderFlg == true).Select(p => p.STAdvanceOrderId);

            var allocatedOrder = _context.WHAllocateAdvanceOrder
                                   .Include(p => p.AllocateAdvanceOrderDetails)
                                   .Where(p => advanceOrderId.Contains(p.StAdvanceOrderId));

            var advanceOrder = _context.STAdvanceOrder.Include(p => p.AdvanceOrderDetails)
                                                            .Where(p => advanceOrderId.Contains(p.Id)).SelectMany(p => p.AdvanceOrderDetails);



            var records = from x in query
                          select new
                          {
                              x.Id,
                              x.TransactionNo,
                              TransactionTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), x.TransactionType)),
                              x.OrderType,
                              OrderTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), x.OrderType)),
                              x.DeliveryType,
                              DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                              x.RequestStatus,
                              RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), x.RequestStatus)),
                              OrderedBy = x.Store.Name,
                              OrderedTo = x.Warehouse.Name,
                              x.PONumber,
                              x.PODate,
                              OrderedDate = x.PODate,
                              x.ClientName,
                              x.ContactNumber,
                              x.Address1,
                              x.Address2,
                              x.Address3,
                              x.Remarks,
                              x.PaymentMode,
                              x.SalesAgent,
                              x.isAdvanceOrderFlg,
                              PayMentModeStr = (x.PaymentMode != null) ? EnumExtensions.SplitName(Enum.GetName(typeof(PaymentModeEnum), x.PaymentMode)) : null,
                              OrderedItems = x.OrderedItems.Select(p => new
                              {
                                  p.Id,
                                  p.STOrderId,
                                  p.ItemId,
                                  p.Item.SerialNumber,
                                  ItemCode = p.Item.Code,
                                  ItemName = p.Item.Name,
                                  SizeName = p.Item.Size.Name,
                                  p.Item.Tonality,
                                  p.RequestedQuantity,
                                  Available = service.GetItemAvailableQuantity(p.ItemId, x.WarehouseId),
                                  StAvailable = stService.GetItemAvailableQuantity(p.ItemId, x.StoreId, true),
                                  advOrderForAllocationQty = stAdvanceSerice.GetAdvanceOrderForAllocationQuantity(
                                                            advanceOrder.Where(c => c.STAdvanceOrderId == x.STAdvanceOrderId
                                                            && (c.ItemId == p.ItemId || c.Code == p.Item.Code || c.sizeId == p.Item.SizeId)).FirstOrDefault(),
                                                            p.ItemId,
                                                            x.STAdvanceOrderId),
                                  AllocatedQty = stAdvanceSerice.GetAdvanceOrderAllocatedQuantity(
                                                            advanceOrder.Where(c => c.STAdvanceOrderId == x.STAdvanceOrderId
                                                            && (c.ItemId == p.ItemId || c.Code == p.Item.Code || c.sizeId == p.Item.SizeId)).FirstOrDefault(),
                                                            p.ItemId,
                                                            x.STAdvanceOrderId),
                                  p.ApprovedQuantity,
                                  p.ApprovedRemarks,
                                  p.RequestedRemarks,
                                  p.isTonalityAny,
                              }).OrderBy(p => p.Id)

                          };


            records = records.OrderByDescending(p => p.Id);

            response.List.AddRange(records);

            return response;



        }



        public object GetAllAdvanceOrders(SearchApproveRequests search, AppSettings appSettings)
        {
            IQueryable<STAdvanceOrder> query = _context.STAdvanceOrder
                                                .Include(p => p.Store)
                                                .Include(p => p.Warehouse)
                                                .Include(p => p.AdvanceOrderDetails)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size);

            if (search.RequestStatus != null)
            {
                query = query.Where(p => search.RequestStatus.Contains(p.RequestStatus));
            }

            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.AONumber))
            {
                query = query.Where(p => p.AONumber.ToLower() == search.AONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.SiNumber))
            {
                query = query.Where(p => p.SINumber.ToLower() == search.SiNumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ClientName))
            {
                query = query.Where(p => p.ClientName.ToLower() == search.ClientName.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.OrderedBy))
            {
                query = query.Where(p => p.Store.Name.ToLower() == search.OrderedBy.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ItemCode))
            {
                query = query.Where(p => p.AdvanceOrderDetails.Where(x => x.Code.Contains(search.ItemCode)).Count() > 0);
            }

            if (search.RequestDateFrom.HasValue)
            {
                query = query.Where(p => search.RequestDateFrom.Value <= p.DateCreated);
            }

            if (search.RequestDateTo.HasValue)
            {
                query = query.Where(p => search.RequestDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59) >= p.DateCreated);
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



            var records = from x in query
                          select new
                          {
                              x.Id,
                              x.AONumber,
                              x.SINumber,
                              x.PONumber,
                              x.RequestStatus,
                              RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), x.RequestStatus)),
                              x.OrderStatus,
                              OrderStatusStr = x.OrderStatus != null ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), x.OrderStatus)) : null,
                              OrderedBy = x.Store.Name,
                              OrderedTo = x.Warehouse.Name,
                              x.ClientName,
                              x.ContactNumber,
                              x.Address1,
                              x.Address2,
                              x.Address3,
                              x.Remarks,
                              x.DateCreated,
                              x.DeliveryStatus,
                              x.StoreId,
                              x.PaymentMode,
                              x.ApproveDate,
                              PaymentModeStr = x.PaymentMode != null ? EnumExtensions.SplitName(Enum.GetName(typeof(PaymentModeEnum), x.PaymentMode)) : null,
                              DeliveryStatusStr = x.DeliveryStatus != null ? EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), x.DeliveryStatus)) : null,
                              x.SalesAgent,
                              //x.changeStatusReasons,
                              AdvanceOrderDetails = x.AdvanceOrderDetails.Select(p => new
                              {
                                  p.Id, 
                                  p.STAdvanceOrderId,
                                  p.ItemId,
                                  p.Item.SerialNumber,
                                  ItemCode = (p.isCustom == true) ? p.Code : p.Item.Code,
                                  ItemName = p.Item.Name,
                                  SizeName = (p.isCustom == true) ? _context.Sizes.Where(c => c.Id == p.sizeId).Select(c => c.Name).FirstOrDefault() : p.Item.Size.Name,
                                  Tonality = (p.isCustom == true) ? p.tonality : p.Item.Tonality,
                                  p.Quantity,
                                  p.ApprovedQuantity,
                                  p.Remarks
                              })

                          };




            response.List.AddRange(records);

            return response;
        }



        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>STOrder</returns>
        public IEnumerable<object> GetOrdersToBeAssignedWithDRNumber(OrderSearch search)
        {

            var query = DrnumberQuery(search);


            var records = from x in query
                          select new
                          {
                              x.Id,
                              x.TransactionNo,
                              TransactionTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), x.TransactionType)),
                              x.OrderType,
                              OrderTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), x.OrderType)),
                              x.DeliveryType,
                              DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                              x.RequestStatus,
                              RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), x.RequestStatus)),
                              OrderedBy = x.Store.Name,
                              OrderedTo = x.Warehouse.Name,
                              x.PONumber,
                              x.PODate,
                              OrderedDate = x.PODate,
                              x.ClientName,
                              x.ContactNumber,
                              x.Address1,
                              x.Address2,
                              x.Address3,
                              x.Remarks,
                              x.WHDRNumber,
                              IsClient = (x.OrderType == OrderTypeEnum.ClientOrder && x.DeliveryType == DeliveryTypeEnum.Delivery) ? true : false,
                              HasWHDRNumber = (x.WHDRNumber != null) ? true : false,
                              OrderedItems = x.OrderedItems.Select(p => new
                              {
                                  p.Item.SerialNumber,
                                  ItemCode = p.Item.Code,
                                  ItemName = p.Item.Name,
                                  SizeName = p.Item.Size.Name,
                                  p.Item.Tonality,
                                  p.RequestedQuantity,
                                  p.ApprovedQuantity,
                                  p.ApprovedRemarks,
                                  p.Id
                              }).OrderBy(p => p.Id),
                              TotalOrderedItemsQty = x.OrderedItems.Sum(p => p.ApprovedQuantity)

                          };


            return records.OrderByDescending(p => p.Id);
        }


        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>STOrder</returns>
        public object GetOrdersToBeAssignedWithDRNumberPaged(OrderSearch search , AppSettings appSettings)
        {

            var query = DrnumberQuery(search);
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


            var records = from x in query
                          select new
                          {
                              x.Id,
                              x.TransactionNo,
                              TransactionTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), x.TransactionType)),
                              x.OrderType,
                              OrderTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), x.OrderType)),
                              x.DeliveryType,
                              DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                              x.RequestStatus,
                              RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), x.RequestStatus)),
                              OrderedBy = x.Store.Name,
                              OrderedTo = x.Warehouse.Name,
                              x.PONumber,
                              x.PODate,
                              OrderedDate = x.PODate,
                              x.ClientName,
                              x.ContactNumber,
                              x.Address1,
                              x.Address2,
                              x.Address3,
                              x.Remarks,
                              x.WHDRNumber,
                              IsClient = (x.OrderType == OrderTypeEnum.ClientOrder && x.DeliveryType == DeliveryTypeEnum.Delivery) ? true : false,
                              HasWHDRNumber = (x.WHDRNumber != null) ? true : false,
                              OrderedItems = x.OrderedItems.Select(p => new
                              {
                                  p.Item.SerialNumber,
                                  ItemCode = p.Item.Code,
                                  ItemName = p.Item.Name,
                                  SizeName = p.Item.Size.Name,
                                  p.Item.Tonality,
                                  p.RequestedQuantity,
                                  p.ApprovedQuantity,
                                  p.ApprovedRemarks,
                                  p.isTonalityAny,
                                  p.Id
                              }).OrderBy(p => p.Id),
                              TotalOrderedItemsQty = x.OrderedItems.Sum(p => p.ApprovedQuantity)

                          };

            response.List.AddRange(records);

            return response;
        }


        public IQueryable<STOrder> DrnumberQuery(OrderSearch search)
        {
            IQueryable<STOrder> query = _context.STOrders
                                                .Include(p => p.Store)
                                                .Include(p => p.Warehouse)
                                                .Include(p => p.OrderedItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                .Where(p =>
                                                    (p.OrderType == OrderTypeEnum.ShowroomStockOrder || p.OrderType == OrderTypeEnum.ClientOrder) &&
                                                    p.RequestStatus == RequestStatusEnum.Approved && (p.OrderedItems.Sum(x => x.ApprovedQuantity) != 0));

            if (search.StoreId.HasValue)
            {
                query = query.Where(p => p.StoreId == search.StoreId);
            }

            if (search.WarehouseId.HasValue)
            {
                query = query.Where(p => p.WarehouseId == search.WarehouseId);
            }

            if (search.PODateFrom.HasValue)
            {
                query = query.Where(p => search.PODateFrom.Value <= p.PODate);
            }

            if (search.PODateTo.HasValue)
            {
                query = query.Where(p => search.PODateTo.Value >= p.PODate);
            }

            if (!string.IsNullOrEmpty(search.PONumber))
            {
                query = query.Where(p => p.PONumber.Contains(search.PONumber));
            }

            if (!string.IsNullOrEmpty(search.transactionNo))
            {
                query = query.Where(p => p.TransactionNo.Contains(search.transactionNo));
            }

            if (!string.IsNullOrWhiteSpace(search.ItemName))
            {
                query = query.Where(x => x.OrderedItems.Where(i => i.Item.Name.Contains(search.ItemName)).Count() > 0);
            }

            if (search.WithDRNumber)
            {
                query = query.Where(p => p.WHDRNumber != null);
            }
            else
            {
                query = query.Where(p => p.WHDRNumber == null);
            }


            return query.OrderByDescending(p => p.Id);
        }

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>STOrder</returns>
        public STOrder GetOrderByIdAndStoreId(int? id, int? storeId)
        {


            var record =  _context.STOrders
                        .Where(p => p.StoreId == storeId
                                    && p.Id == id
                                    /*&& p.DeliveryType != DeliveryTypeEnum.Pickup*/)
                            .Include(p => p.Store)
                            .Include(p => p.Warehouse)
                            .Include(p => p.OrderedItems)
                                .ThenInclude(p => p.Item)
                                    .ThenInclude(p => p.Size)
                            .Include(p => p.Store)
                            .Include(p => p.Warehouse)
                            .FirstOrDefault();


            record.OrderedItems = record.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                ? record.OrderedItems.OrderByDescending(p => p.Id).ToList()
                : record.OrderedItems.OrderBy(p => p.Id).ToList();
   
            return record;
        }

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>STOrder</returns>
        public STOrder GetOrderById(int? id)
        {
            return _context.STOrders
                        .Where(p => p.Id == id
                                    && p.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder
                                    && p.DeliveryType != DeliveryTypeEnum.Pickup)
                            .Include(p => p.Store)
                            .Include(p => p.Warehouse)
                            .Include(p => p.OrderedItems)
                                .ThenInclude(p => p.Item)
                                    .ThenInclude(p => p.Size)
                            .Include(p => p.Store)
                            .Include(p => p.Warehouse)
                            .FirstOrDefault();
        }

        /// <summary>
        /// Insert advance order
        /// </summary>
        /// <param name="stAdvanceOrder">stAdvanceOrder</param>
        public void InsertAdvanceOrder(STAdvanceOrder stAdvanceOrder)
        {
            var totalRecordCount = Convert.ToInt32(this._context.STAdvanceOrder.Count() + 1).ToString();

            stAdvanceOrder.AONumber = string.Format("AO{0}", totalRecordCount.PadLeft(6, '0'));

            stAdvanceOrder.DateCreated = DateTime.Now;

            stAdvanceOrder.RequestStatus = RequestStatusEnum.Pending;
            stAdvanceOrder.DeliveryStatus = DeliveryStatusEnum.Pending;

        

            foreach(var details in stAdvanceOrder.AdvanceOrderDetails)
            {
                details.DateCreated = DateTime.Now;
                details.DeliveryStatus = DeliveryStatusEnum.Pending;
            }


            _context.STAdvanceOrder.Add(stAdvanceOrder);
            _context.SaveChanges();
        }

        /// <summary>
        /// Insert order
        /// </summary>
        /// <param name="stOrder">STOrder</param>
        public void InsertOrder(STOrder stOrder, AppSettings appSettings)
        {
            stOrder.TransactionType = TransactionTypeEnum.PO;
            var totalRecordCount = Convert.ToInt32(this._context.STOrders.Where(x => x.TransactionType == stOrder.TransactionType).Count() + 1).ToString();
            var totalRecordPerStoreCount = Convert.ToInt32(this._context.STOrders.Where(x => x.TransactionType == stOrder.TransactionType && x.StoreId == stOrder.StoreId && !string.IsNullOrEmpty(x.PONumber)).Count() + 1).ToString();

            stOrder.TransactionNo = string.Format("PO{0}", totalRecordCount.PadLeft(6, '0'));

            //stOrder.PONumber = stOrder.TransactionNo;
            var store = _context.Stores.Where(p => p.Id == stOrder.StoreId).FirstOrDefault();

            //added advance order flag it will not generate PO number

            if (store != null && stOrder.isAdvanceOrderFlg != true)
            {
                //will get series count base on store code if store code does not exist 0 will be the the default value
                var storeSeries = appSettings.StoreSeries.ContainsKey(store.Code) ? appSettings.StoreSeries[store.Code] : 0;
                // add series count and totalrecords on store
                var snumber = storeSeries + Convert.ToInt32(totalRecordPerStoreCount);

                //Added condition for dealer 
                if(string.IsNullOrWhiteSpace(stOrder.PONumber))
                {
                    stOrder.PONumber = this.getPONumber(store.Code, snumber, false); 
                    var poCount = this.getPOCount(stOrder.PONumber);
                    var deduct = true;

                    while(poCount > 0)
                    {
                        if(deduct)
                        {
                            //deducted series
                            var deSnumber = snumber - 1;
                            stOrder.PONumber = this.getPONumber(store.Code, deSnumber, false);
                            poCount = this.getPOCount(stOrder.PONumber);
                        }
                       
                        if(poCount > 0)
                        {
                            snumber = snumber + 1;
                            stOrder.PONumber = this.getPONumber(store.Code, snumber, false);
                            poCount = this.getPOCount(stOrder.PONumber);
                            deduct = false;
                            
                        }

                    }
                }
               
            }


            stOrder.RequestStatus = RequestStatusEnum.Pending;
            stOrder.DateCreated = DateTime.Now;

            //added for optimization purposes
            stOrder.OrderStatus = OrderStatusEnum.Incomplete;

            IQueryable<WHStock> whStock = _context.WHStocks.Where(p => p.WarehouseId == stOrder.WarehouseId);

            foreach (var detail in stOrder.OrderedItems)
            {
                detail.DeliveryStatus = DeliveryStatusEnum.Pending;
                detail.ReleaseStatus = ReleaseStatusEnum.Pending;
                detail.DateCreated = DateTime.Now;

                // To update the stock summary after creating new PO. Clients request
                var stock = whStock.Where(p => p.ItemId == detail.ItemId).FirstOrDefault();
                if(stock != null)
                {
                    stock.ChangeDate = DateTime.Now;

                    _context.WHStocks.Update(stock);
                }
            }


            if (stOrder.OrderType != OrderTypeEnum.ClientOrder)
            {
                stOrder.ClientName = null;
                stOrder.ContactNumber = null;
                stOrder.Address1 = null;
                stOrder.Address2 = null;
                stOrder.Address3 = null;
            }

            _context.STOrders.Add(stOrder);
            _context.SaveChanges();
        }


        public string GeneratePoNumber(STOrder stOrder, AppSettings appSettings, bool updateDelivery)
        {
            var totalRecordPerStoreCount = Convert.ToInt32(this._context.STOrders.Where(x => x.TransactionType == stOrder.TransactionType && x.StoreId == stOrder.StoreId && !string.IsNullOrEmpty(x.PONumber)).Count() + 1).ToString();
            var store = _context.Stores.Where(p => p.Id == stOrder.StoreId).FirstOrDefault();

            if (store != null)
            {
                if (stOrder.isAdvanceOrderFlg == false && updateDelivery == false)
                {
                    return String.Empty;
                }

                if(string.IsNullOrEmpty(stOrder.PONumber))
                {


                    //will get series count base on store code if store code does not exist 0 will be the the default value
                    var storeSeries = appSettings.StoreSeries.ContainsKey(store.Code) ? appSettings.StoreSeries[store.Code] : 0;
                    // add series count and totalrecords on store
                    var snumber = storeSeries + Convert.ToInt32(totalRecordPerStoreCount);

                    //Added condition for dealer 
               
                        stOrder.PONumber = this.getPONumber(store.Code, snumber, false);
                        var poCount = this.getPOCount(stOrder.PONumber);
                        var deduct = true;

                        while (poCount > 0)
                        {
                            if (deduct)
                            {
                                //deducted series
                                var deSnumber = snumber - 1;
                                stOrder.PONumber = this.getPONumber(store.Code, deSnumber, false);
                                poCount = this.getPOCount(stOrder.PONumber);
                            }

                            if (poCount > 0)
                            {
                                snumber = snumber + 1;
                                stOrder.PONumber = this.getPONumber(store.Code, snumber, false);
                                poCount = this.getPOCount(stOrder.PONumber);
                                deduct = false;

                            }

                        }

                
                    return stOrder.PONumber;

                }


            }

            return String.Empty;
        }

        public void InsertOrderInterBranch(STOrder stOrder, ClaimsPrincipal user, AppSettings appSettings)
        {
            var totalRecordCount = Convert.ToInt32(this._context.STOrders.Where(x => x.TransactionType == stOrder.TransactionType).Count() + 1).ToString();
            var totalRecordPerStoreCount = Convert.ToInt32(this._context.STOrders.Where(x => x.TransactionType == stOrder.TransactionType && x.StoreId == stOrder.StoreId).Count() + 1).ToString();

            stOrder.TransactionNo = string.Format("TR{0}", totalRecordCount.PadLeft(6, '0'));

            var store = _context.Stores.Where(p => p.Id == stOrder.StoreId).FirstOrDefault();
            if (store != null)
            {
                var storeSeries = appSettings.StoreTransferSeries.ContainsKey(store.Code) ? appSettings.StoreTransferSeries[store.Code] : 0;

                var snumber = storeSeries + Convert.ToInt32(totalRecordPerStoreCount);
                //Added condition for dealer 
                if (string.IsNullOrWhiteSpace(stOrder.PONumber))
                {

                    if (string.IsNullOrWhiteSpace(stOrder.PONumber))
                    {
                        stOrder.PONumber = this.getPONumber(store.Code, snumber, true);
                        var poCount = this.getPOCount(stOrder.PONumber);
                        var deduct = true;

                        while (poCount > 0)
                        {
                            if (deduct)
                            {
                                //deducted series
                                var deSnumber = snumber - 1;
                                stOrder.PONumber = this.getPONumber(store.Code, deSnumber, true);
                                poCount = this.getPOCount(stOrder.PONumber);
                            }

                            if (poCount > 0)
                            {
                                snumber = snumber + 1;
                                stOrder.PONumber = this.getPONumber(store.Code, snumber, true);
                                poCount = this.getPOCount(stOrder.PONumber);
                                deduct = false;

                            }

                        }
                    }
                    stOrder.PONumber = string.Format(store.Code + "-TR{0}", snumber.ToString().PadLeft(6, '0'));
                }
            }

            //stOrder.PONumber = stOrder.TransactionNo;ss

            stOrder.DateCreated = DateTime.Now;

            foreach (var detail in stOrder.OrderedItems)
            {
                detail.DeliveryStatus = DeliveryStatusEnum.Pending;
                detail.DateCreated = DateTime.Now;
            }

            _context.STOrders.Add(stOrder);
            _context.SaveChanges();

            var trailDetail = new UserTrail();
            trailDetail.Action = "Add transfer";
            trailDetail.DateCreated = DateTime.Now;
            trailDetail.Detail = " Record affected : Transaction No. " + stOrder.PONumber;
            trailDetail.Transaction = "Add transfer order";
            trailDetail.UserId = Convert.ToInt32(user.Claims.ToList()[1].Value);

            new UserTrailService(_context).InsertTrail(trailDetail);
        }


        /// <summary>
        /// Updates order used in Main site for Approve Request module
        /// </summary>
        /// <param name="obj">STOrder</param>
        public void UpdateOrder(STOrder param)
        {
            var order = _context.STOrders.Where(x => x.Id == param.Id).SingleOrDefault();
            var warehouse = _context.Warehouses.Where(p => p.Id == order.WarehouseId).SingleOrDefault();

            order.DateUpdated = DateTime.Now;
            order.RequestStatus = (param.OrderedItems.Sum(p => p.ApprovedQuantity) != 0) ? RequestStatusEnum.Approved : RequestStatusEnum.Cancelled;

            if(param.OrderStatus.HasValue)
            {
                order.OrderStatus = param.OrderStatus;
            }
           

            if (warehouse.Vendor)
            {
                order.WHDRNumber = "N/A";
            }

            _context.STOrders.Update(order);
            _context.SaveChanges();

           var WhStocks =  _context.WHStocks.Where(p => p.WarehouseId == warehouse.Id);

            foreach (var item in param.OrderedItems)
            {
                var orderedItem = order.OrderedItems.FirstOrDefault(x => x.Id == item.Id && x.STOrderId == item.STOrderId && x.ItemId == item.ItemId);
                if (orderedItem == null)
                {
                    continue;
                }

                orderedItem.ApprovedQuantity = item.ApprovedQuantity;
                orderedItem.ApprovedRemarks = item.ApprovedRemarks;
                orderedItem.isTonalityAny = item.isTonalityAny;
                orderedItem.DeliveryStatus = DeliveryStatusEnum.Waiting;
                orderedItem.ReleaseStatus = ReleaseStatusEnum.Waiting;
                if (warehouse.Vendor)
                {
                    orderedItem.ReleaseStatus = ReleaseStatusEnum.Released;
                }

                var stock = WhStocks.Where(p => p.ItemId == item.ItemId).FirstOrDefault();

                if(stock != null)
                {
                    stock.ChangeDate = DateTime.Now;
                    _context.WHStocks.Update(stock);
                }
                



                orderedItem.DateUpdated = DateTime.Now;

                _context.STOrderDetails.Update(orderedItem);
               
                _context.SaveChanges();

            }

            if (warehouse.Vendor)
            {
                InsertDeliveryToShowroom(param, order.StoreId.Value);
            }

        }

        /// <summary>
        /// Create Delivery Record for Vendor Orders
        /// </summary>
        /// <param name="stdelivery">STDelivery</param>
        public void InsertDeliveryToShowroom(STOrder order, int storeId)
        {
            var stdelivery = new STDelivery();
            if (order != null)
            {

                stdelivery.STOrderId = order.Id;
                stdelivery.Id = 0;
                stdelivery.DateCreated = DateTime.Now;
                stdelivery.DeliveryDate = DateTime.Now;
                stdelivery.ApprovedDeliveryDate = DateTime.Now;
                stdelivery.StoreId = storeId;

                var ShowroomDeliveries = new List<STShowroomDelivery>();

                foreach (var detail in order.OrderedItems)
                {

                    //  Check if request deliver quantity is 0
                    if (detail.ApprovedQuantity == 0)
                    {
                        //  Skip record
                        continue;
                    }
                    var showroomDelivery = new STShowroomDelivery();
                    showroomDelivery.STOrderDetailId = detail.Id;
                    showroomDelivery.Id = 0;
                    showroomDelivery.DateCreated = DateTime.Now;
                    showroomDelivery.DeliveryStatus = DeliveryStatusEnum.Waiting;
                    showroomDelivery.ReleaseStatus = ReleaseStatusEnum.Released;
                    showroomDelivery.ItemId = detail.ItemId;
                    showroomDelivery.Quantity = detail.ApprovedQuantity;
                    //showroomDelivery.Remarks = detail.RequestedRemarks;

                    _context.STShowroomDeliveries.Add(showroomDelivery);
                    _context.SaveChanges();

                    ShowroomDeliveries.Add(showroomDelivery);

                }
                stdelivery.ShowroomDeliveries = ShowroomDeliveries;
                _context.STDeliveries.Add(stdelivery);
                _context.SaveChanges();

            }

        }



        /// <summary>
        /// Updates the WHDRNumber of Order
        /// </summary>
        /// <param name="obj">STOrder</param>
        public void UpdateWHDRNumberOfSTOrder(STOrder param, IWHStockService whStockService)
        {
            var order = _context.STOrders.Where(x => x.Id == param.Id).SingleOrDefault();

            order.DateUpdated = DateTime.Now;

            if (param.RequestStatus != RequestStatusEnum.Cancelled)
            {

                order.WHDRNumber = param.WHDRNumber;

            }
            else
            {
                order.RequestStatus = param.RequestStatus;
                order.OrderStatus = OrderStatusEnum.Cancelled;
            }

            _context.STOrders.Update(order);


            //to trigger update of stock warehouse
            var orderDetails = _context.STOrderDetails.Where(p => p.STOrderId == order.Id).ToList();
            if (orderDetails.Count() > 0)
            {
                for (int i = 0; i < orderDetails.Count(); i++)
                {
                    var stock = _context.WHStocks.Where(p => p.ItemId == orderDetails[i].ItemId && p.WarehouseId == order.WarehouseId).Last();
                    if (stock != null)
                    {
                        stock.ChangeDate = DateTime.Now;

                        _context.WHStocks.Update(stock);

                    }

                }
            }


            //var orderDetails = _context.STOrderDetails.Where(p => p.STOrderId == order.Id).ToList();

            //for (int i = 0; i < orderDetails.Count(); i++)
            //{
            //    var whstock = new WHStock
            //    {
            //        WarehouseId = order.WarehouseId,
            //        STOrderDetailId = orderDetails[i].Id,
            //        ItemId = orderDetails[i].ItemId,
            //        OnHand = -orderDetails[i].ApprovedQuantity,
            //        TransactionType = TransactionTypeEnum.PO,
            //    };

            //    whstock.DeliveryStatus = DeliveryStatusEnum.Waiting;
            //    whstock.ReleaseStatus = ReleaseStatusEnum.Waiting;

            //    whStockService.InsertStock(whstock);
            //}

            _context.SaveChanges();
        }


        /// <summary>
        /// Get all for deliveries
        /// </summary>
        /// <param name="search">Search parameters</param>
        /// <returns>STOrders</returns>
        public IEnumerable<STOrder> GetAllForDeliveries(SearchDeliveries search)
        {
            IQueryable<STOrder> query = _context.STOrders
                                            .Include(p => p.Store)
                                            .Include(p => p.Warehouse)
                                            .Include(p => p.Deliveries);

            //  Searched by PONumber
            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }

            var records = new List<STOrder>();

            foreach (var q in query)
            {

                //  Check if DRNumber search criteria has value
                if (!string.IsNullOrWhiteSpace(search.DRNumber))
                {
                    //  Searched by DRNumber
                    q.Deliveries = q.Deliveries.Where(y => y.DRNumber.ToLower() == search.DRNumber.ToLower()).ToList();
                }

                //  Check if DeliveryDateFrom search criteria has value
                if (search.DeliveryDateFrom.HasValue)
                {
                    //  Searched by DeliveryDateFrom <= DeliveryDate
                    q.Deliveries = q.Deliveries.Where(y => search.DeliveryDateFrom <= y.DeliveryDate).ToList();
                }

                //  Check if DeliveryDateTo search criteria has value
                if (search.DeliveryDateTo.HasValue)
                {
                    //  Searched by DeliveryDateTo >= DeliveryDate
                    q.Deliveries = q.Deliveries.Where(y => search.DeliveryDateTo >= y.DeliveryDate).ToList();
                }

                if (q.Deliveries != null && q.Deliveries.Count() > 0)
                {
                    foreach (var del in q.Deliveries)
                    {
                        if (q.OrderType == OrderTypeEnum.ShowroomStockOrder)
                        {
                            GetShowroomDeliveries(del);
                        }
                        else if (q.OrderType == OrderTypeEnum.ClientOrder || q.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                        {
                            if (q.DeliveryType == DeliveryTypeEnum.ShowroomPickup)
                            {
                                GetShowroomDeliveries(del);
                            }
                            else
                            {
                                GetClientDeliveries(del);
                            }
                        }
                    }

                    records.Add(q);
                }

            }

            return records;
        }

        private void GetClientDeliveries(STDelivery del)
        {
            del.ClientDeliveries = _context.STClientDeliveries
                                                                        .Include(p => p.Item)
                                                                            .ThenInclude(p => p.Size)
                                                                        .Where(p => p.STDeliveryId == del.Id).ToList();
        }

        private void GetShowroomDeliveries(STDelivery del)
        {
            del.ShowroomDeliveries = _context.STShowroomDeliveries
                                            .Include(p => p.Item)
                                                .ThenInclude(p => p.Size)
                                            .Where(p => p.STDeliveryId == del.Id).ToList();
        }


        /// <summary>
        /// Get all for receiving
        /// </summary>
        /// <param name="search">Search parameters</param>
        /// <returns>STOrders</returns>
        public IEnumerable<STOrder> GetAllForReceiving(SearchReceiveItems search)
        {
            IQueryable<STOrder> query = _context.STOrders
                                            .Where(p => p.StoreId == search.StoreId)
                                            .Include(p => p.Deliveries);

            if (search.TransactionType.HasValue)
            {
                query = query.Where(p => p.TransactionType == search.TransactionType);
            }

            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }

            var records = new List<STOrder>();

            foreach (var q in query)
            {



                //  Check if DRNumber search criteria has value
                if (!string.IsNullOrWhiteSpace(search.DRNumber))
                {
                    //  Searched by DRNumber
                    q.Deliveries = q.Deliveries.Where(y => y.DRNumber.ToLower() == search.DRNumber.ToLower()).ToList();
                }

                //  Check if DeliveryDateFrom search criteria has value
                if (search.DeliveryDateFrom.HasValue)
                {
                    //  Searched by DeliveryDateFrom <= DeliveryDate
                    q.Deliveries = q.Deliveries.Where(y => search.DeliveryDateFrom <= y.DeliveryDate).ToList();
                }

                //  Check if DeliveryDateTo search criteria has value
                if (search.DeliveryDateTo.HasValue)
                {
                    //  Searched by DeliveryDateTo >= DeliveryDate
                    q.Deliveries = q.Deliveries.Where(y => search.DeliveryDateTo >= y.DeliveryDate).ToList();
                }


                if (q.Deliveries != null && q.Deliveries.Count() > 0)
                {
                    foreach (var del in q.Deliveries)
                    {

                        //  Instantiate order record
                        var order = new STOrder
                        {
                            Id = q.Id,
                            TransactionNo = q.TransactionNo,
                            StoreId = q.StoreId,
                            Store = _context.Stores.Where(p => p.Id == q.StoreId).FirstOrDefault(),
                            TransactionType = q.TransactionType,
                            WarehouseId = q.WarehouseId,
                            Warehouse = _context.Warehouses.Where(p => p.Id == q.WarehouseId).FirstOrDefault(),
                            PONumber = q.PONumber,
                            PODate = q.PODate,
                            Remarks = q.Remarks,
                            ORNumber = q.ORNumber,
                            OrderToStoreId = q.OrderToStoreId,
                            RequestStatus = q.RequestStatus,
                            OrderType = q.OrderType,
                            DeliveryType = q.DeliveryType,
                            WHDRNumber = q.WHDRNumber,
                            Deliveries = new List<STDelivery>()
                        };


                        GetShowroomDeliveriesForReceiving(del);
                        if (del.ShowroomDeliveries != null && del.ShowroomDeliveries.Count() > 0)
                        {
                            order.Deliveries.Add(del);
                            records.Add(order);
                        }

                    }

                }

            }

            return records.OrderByDescending(p => p.Id);
        }


        public object GetAllForReceiving2(SearchReceiveItems search)
        {
            IQueryable<STOrder> query = _context.STOrders
                                            .Where(p => p.StoreId == search.StoreId)
                                            .Include(p => p.Deliveries);

            if (search.TransactionType.HasValue)
            {
                query = query.Where(p => p.TransactionType == search.TransactionType);
            }

            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }

            var records = new List<STOrderListDTO>();

            foreach (var q in query)
            {



                //  Check if DRNumber search criteria has value
                if (!string.IsNullOrWhiteSpace(search.DRNumber))
                {
                    //  Searched by DRNumber
                    q.Deliveries = q.Deliveries.Where(y => y.DRNumber.ToLower() == search.DRNumber.ToLower()).ToList();
                }

                //  Check if DeliveryDateFrom search criteria has value
                if (search.DeliveryDateFrom.HasValue)
                {
                    //  Searched by DeliveryDateFrom <= DeliveryDate
                    q.Deliveries = q.Deliveries.Where(y => search.DeliveryDateFrom <= y.DeliveryDate).ToList();
                }

                //  Check if DeliveryDateTo search criteria has value
                if (search.DeliveryDateTo.HasValue)
                {
                    //  Searched by DeliveryDateTo >= DeliveryDate
                    q.Deliveries = q.Deliveries.Where(y => search.DeliveryDateTo >= y.DeliveryDate).ToList();
                }



                if (q.Deliveries != null && q.Deliveries.Count() > 0)
                {
                    q.Deliveries = q.Deliveries.OrderByDescending(p => p.Id).ToList();
                    foreach (var del in q.Deliveries)
                    {

                        //  Instantiate order record
                        var order = new STOrderListDTO
                        {
                            Id = q.Id,
                            TransactionNo = q.TransactionNo,
                            StoreId = q.StoreId,
                            Store = _context.Stores.Where(p => p.Id == q.StoreId).FirstOrDefault(),
                            isTransfer = q.TransactionType == TransactionTypeEnum.Transfer ? true : false,
                            TransactionType = q.TransactionType,
                            TransactionTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), q.TransactionType)),
                            WarehouseId = q.WarehouseId,
                            Warehouse = _context.Warehouses.Where(p => p.Id == q.WarehouseId).FirstOrDefault(),
                            PONumber = q.PONumber,
                            PODate = q.PODate,
                            Remarks = q.Remarks,
                            ORNumber = q.ORNumber,
                            OrderToStoreId = q.OrderToStoreId,
                            RequestStatus = q.RequestStatus,
                            OrderType = q.OrderType,
                            DeliveryType = q.DeliveryType,
                            WHDRNumber = q.WHDRNumber,
                            orderToStoreCompanyId = _context.Stores.Where(p => p.Id == q.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault(),
                            storeCompany = _context.Stores.Where(p => p.Id == q.StoreId).Select(p => p.CompanyId).FirstOrDefault(),
                            isInterBranch = q.OrderToStoreId.HasValue ?
                                                                (_context.Stores.Where(p => p.Id == q.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault() ==
                                                                _context.Stores.Where(p => p.Id == q.StoreId).Select(p => p.CompanyId).FirstOrDefault() ? true : false)
                                                                : false,

                            Deliveries = new List<STDelivery>()
                        };


                        GetShowroomDeliveriesForReceiving(del);
                        if (del.ShowroomDeliveries != null && del.ShowroomDeliveries.Count() > 0)
                        {
                            order.Deliveries.Add(del);
                            records.Add(order);
                        }

                    }

                }

            }


            return records.OrderByDescending(p => p.Id);


        }

        private void GetShowroomDeliveriesForReceiving(STDelivery del)
        {
            del.ShowroomDeliveries = _context.STShowroomDeliveries
                                            .Include(p => p.Item)
                                                .ThenInclude(p => p.Size)
                                            .Where(p => p.STDeliveryId == del.Id
                                                        && p.DeliveryStatus == DeliveryStatusEnum.Waiting
                                                        && p.ReleaseStatus == ReleaseStatusEnum.Released).ToList();
        }

        /// <summary>
        /// Get receiving item
        /// </summary>
        /// <param name="id">STDelivery.Id</param>
        /// <returns>STOrder</returns>
        public object GetReceivingItemByIdAndStoreId(int? id, int? storeId)
        {

            STOrder obj = null;

            var del = _context.STDeliveries.Where(p => p.StoreId == storeId && p.Id == id)
                            .FirstOrDefault();
            if (del != null)
            {
                //  Get STOrder by STDelivery.STOrderId
                var order = _context.STOrders.Where(p => p.Id == del.STOrderId).FirstOrDefault();
                if (order != null)
                {
                    var delArr = new List<STDelivery>();


                    var showroomDeliveries = _context.STShowroomDeliveries
                                                .Include(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                                .Where(p => p.STDeliveryId == del.Id && p.DeliveryStatus == DeliveryStatusEnum.Waiting && p.ReleaseStatus == ReleaseStatusEnum.Released).ToList();
                    if (showroomDeliveries.Count > 0)
                    {
                        del.ShowroomDeliveries = showroomDeliveries;

                        delArr.Add(del);

                        order.Deliveries = delArr;

                        obj = order;
                    }
                }
            }

            var IsInterBranch = false;
            var IsTransferShowroomPickup = false;
            var TransferHeader = "";
            var IsVendor = false;


            if (obj.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
            {
                var storeCompany = _context.Stores.Where(p => p.Id == obj.StoreId).Select(p => p.CompanyId).FirstOrDefault();
                var orderToStoreCompany = _context.Stores.Where(p => p.Id == obj.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault();

                // Returns true or false
                IsInterBranch = (storeCompany == orderToStoreCompany);

                // Use this header if OrderType = InterBrancOrInterCompany and DeliveryType = ShowroomPickup
                TransferHeader = (IsInterBranch) ? "TOR No.:" : "Branch DR No.:";
                IsTransferShowroomPickup = (obj.DeliveryType == DeliveryTypeEnum.ShowroomPickup);

            }
            else
            {
                IsVendor = _context.Warehouses.AsNoTracking().Where(w => w.Id == obj.WarehouseId).Select(s => s.Vendor).FirstOrDefault();
            }




            return new
            {
                obj.Id,
                obj.TransactionNo,
                obj.PONumber,
                obj.PODate,
                obj.ORNumber,
                obj.WHDRNumber,
                obj.Deliveries,
                obj.DeliveryType,
                obj.OrderType,
                IsInterBranch,
                TransferHeader,
                IsTransferShowroomPickup,
                IsVendor
            };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">STDelivery.Id</param>
        /// <returns></returns>
        public STOrder GetReceivingItemById(int? id)
        {

            STOrder obj = null;

            var del = _context.STDeliveries.Where(p => p.Id == id)
                            .FirstOrDefault();
            if (del != null)
            {
                //  Get STOrder by STDelivery.STOrderId
                var order = _context.STOrders.Where(p => p.Id == del.STOrderId).FirstOrDefault();
                if (order != null)
                {
                    var delArr = new List<STDelivery>();

                    if (order.OrderType == OrderTypeEnum.ShowroomStockOrder)
                    {
                        obj = GetOrderForShowroomStockOrder(obj, del, order, delArr);
                    }
                    else
                    {
                        if (order.DeliveryType == DeliveryTypeEnum.ShowroomPickup)
                        {
                            obj = GetOrderForShowroomStockOrder(obj, del, order, delArr);
                        }
                        else if (order.DeliveryType == DeliveryTypeEnum.Delivery)
                        {
                            obj = GetOrderForClientOrder(obj, del, order, delArr);
                        }
                        else
                        {
                            obj = order;
                        }
                    }
                }
            }

            return obj;
        }

        private STOrder GetOrderForClientOrder(STOrder obj, STDelivery del, STOrder order, List<STDelivery> delArr)
        {
            var clientDeliveries = _context.STClientDeliveries
                                                                .Include(p => p.Item)
                                                                    .ThenInclude(p => p.Size)
                                                                .Where(p => p.STDeliveryId == del.Id && p.DeliveryStatus == DeliveryStatusEnum.Waiting && p.ReleaseStatus == ReleaseStatusEnum.Released).ToList();
            if (clientDeliveries.Count > 0)
            {
                del.ClientDeliveries = clientDeliveries;

                delArr.Add(del);

                order.Deliveries = delArr;

                obj = order;
            }

            return obj;
        }

        private STOrder GetOrderForShowroomStockOrder(STOrder obj, STDelivery del, STOrder order, List<STDelivery> delArr)
        {
            var showroomDeliveries = _context.STShowroomDeliveries
                                                                .Include(p => p.Item)
                                                                    .ThenInclude(p => p.Size)
                                                                .Where(p => p.STDeliveryId == del.Id && p.DeliveryStatus == DeliveryStatusEnum.Waiting && p.ReleaseStatus == ReleaseStatusEnum.Released).ToList();
            if (showroomDeliveries.Count > 0)
            {
                del.ShowroomDeliveries = showroomDeliveries;

                delArr.Add(del);

                order.Deliveries = delArr;

                obj = order;
            }

            return obj;
        }


        /// <summary>
        /// Get showroom deliveries
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>STOrders</returns>
        public object GetShowroomDeliveriesByIdAndStoreId(int? id, int? storeId)
        {
            var record = _context.STOrders
                        .Where(p => p.StoreId == storeId
                                    && p.Id == id
                                    //&& p.DeliveryType != DeliveryTypeEnum.Pickup
                                    && p.RequestStatus == RequestStatusEnum.Approved)
                            .Include(p => p.OrderedItems)
                                   .ThenInclude(p => p.Item)
                                        .ThenInclude(p => p.Size)
                            .Include(p => p.Deliveries)
                                .ThenInclude(p => p.ShowroomDeliveries)
                                    .ThenInclude(p => p.Item)
                                        .ThenInclude(p => p.Size)
                            .Include(p => p.Store)
                            .Include(p => p.Warehouse)
                            .FirstOrDefault();




            if (record != null)
            {
                //  Check if the order is for client and
                //  the delivery type is not showroom pickup
                if (
                    (record.OrderType == OrderTypeEnum.ClientOrder &&
                    record.DeliveryType != DeliveryTypeEnum.ShowroomPickup)
                    ||
                    (record.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder &&
                    record.DeliveryType != DeliveryTypeEnum.ShowroomPickup)
                    )
                {
                    return null;
                }


            }

            var deliveries = new
            {
                record.Address1,
                record.Address2,
                record.Address3,
                record.ClientName,
                record.ContactNumber,
                record.DateCreated,
                record.DateUpdated,
                record.Deliveries,
                record.DeliveryType,
                record.Id,
                record.OrderType,
                record.PODate,
                record.PONumber,
                record.Remarks,
                record.RequestStatus,
                record.Store,
                record.StoreId,
                record.TransactionNo,
                record.TransactionType,
                record.Warehouse,
                record.WarehouseId,
                record.OrderToStoreId,
                deliverFrom = _context.Stores.Where(p => p.Id == record.OrderToStoreId).Select(p => p.Name).FirstOrDefault(),
                OrderedItems = record.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder 
                               ? record.OrderedItems.OrderByDescending(p => p.Id).ToList() 
                               : record.OrderedItems.OrderBy(p => p.Id).ToList(),

                RemainingForDelivery = GetRemainingForDelivery(record),
            };

            //foreach (var del in deliveries.OrderedItems)
            //{
            //    var deliveredItem = 0;
            //    foreach (var stdel in deliveries.Deliveries)
            //    {
            //        deliveredItem += Convert.ToInt32(stdel.ShowroomDeliveries.Where(p => p.ItemId == del.ItemId && p.IsRemainingForReceiving == false).Sum(z => z.Quantity));
            //    }

            //    del.ApprovedQuantity = del.ApprovedQuantity - deliveredItem;


            //}


            return deliveries;
        }



        public object GetClientDeliveriesByIdAndStoreId(int? id, int? storeId)
        {
            var record = _context.STOrders
                        .Where(p => p.StoreId == storeId
                                    && p.Id == id
                                    &&
                                    (
                                        p.OrderType == OrderTypeEnum.ClientOrder
                                        || p.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                    )
                                    //&& p.DeliveryType == DeliveryTypeEnum.Delivery
                                    && p.RequestStatus == RequestStatusEnum.Approved)
                            .Include(p => p.OrderedItems)
                            .Include(p => p.Deliveries)
                                .ThenInclude(p => p.ClientDeliveries)
                                    .ThenInclude(p => p.Item)
                                        .ThenInclude(p => p.Size)
                            .Include(p => p.Store)
                            .Include(p => p.Warehouse)
                            .FirstOrDefault();

            int? orderByCompanyId = null;
            int? orderToCompanyId = null;
            bool IsInterbranch = false;

            // Set as default value for others.
            string TableHeader = record.WHDRNumber;
            string TableBodyValue = null;

            if (record.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
            {
                orderByCompanyId = _context.Stores.Where(p => p.Id == record.StoreId).Select(p => p.CompanyId).FirstOrDefault();
                orderToCompanyId = _context.Stores.Where(p => p.Id == record.OrderToStoreId).Select(p => p.CompanyId).FirstOrDefault();

                IsInterbranch = (orderByCompanyId == orderToCompanyId)
                                                ? true : false;
                TableHeader = (IsInterbranch) ? "TOR No." : "DR No.";
                TableBodyValue = (IsInterbranch) ? record.ORNumber : record.WHDRNumber;
            }

            var deliveries = new
            {
                record.Address1,
                record.Address2,
                record.Address3,
                record.ClientName,
                record.ContactNumber,
                record.DateCreated,
                record.DateUpdated,
                record.Deliveries,
                record.DeliveryType,
                record.Id,
                record.OrderType,
                OrderedItems = record.OrderedItems.OrderBy(p => p.Id).ToList(),
                record.PODate,
                record.PONumber,
                record.Remarks,
                record.RequestStatus,
                record.Store,
                record.StoreId,
                record.TransactionNo,
                record.TransactionType,
                IsInterbranch,
                TableHeader,
                TableBodyValue,
                OrderedTo = record.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder
                            ? (record.Warehouse != null ? record.Warehouse.Name : null)
                            : _context.Stores.Where(p => p.Id == record.OrderToStoreId).Select(y => y.Name).FirstOrDefault(),

                RemainingForDelivery = GetRemainingForDelivery(record)
            };

            return deliveries;
        }


        private int GetRemainingForDelivery(STOrder record)
        {

            var soldQty = Convert.ToInt32(record.OrderedItems.Sum(p => p.ApprovedQuantity));


            int total = 0;
            foreach (var del in record.Deliveries)
            {
                if (del.ShowroomDeliveries != null)
                {
                    total += Convert.ToInt32(del.ShowroomDeliveries.Where(p => p.IsRemainingForReceiving == false).Sum(p => p.Quantity));
                }
                else
                {
                    total += Convert.ToInt32(del.ClientDeliveries.Sum(p => p.Quantity));
                }

            }
            return soldQty - total;
        }


        /// <summary>
        /// Get delivery by id
        /// </summary>
        /// <param name="id">STDelivery.Id</param>
        /// <returns>STOrder</returns>
        public STOrder GetDeliveryById(int? id)
        {
            STOrder obj = null;

            var del = _context.STDeliveries.Where(p => p.Id == id)
                            .Include(p => p.ShowroomDeliveries)
                                .ThenInclude(p => p.Item)
                                    .ThenInclude(p => p.Size)
                            .Include(p => p.ClientDeliveries)
                                .ThenInclude(p => p.Item)
                                    .ThenInclude(p => p.Size)
                            .FirstOrDefault();
            if (del != null)
            {
                //  Get STOrder by STDelivery.STOrderId
                var order = _context.STOrders.Where(p => p.Id == del.STOrderId).FirstOrDefault();
                if (order != null)
                {
                    var delArr = new List<STDelivery>();
                    delArr.Add(del);

                    order.Deliveries = delArr;

                    obj = order;
                }
            }

            return obj;
        }


        /// <summary>
        /// Get all for releasing
        /// </summary>
        /// <param name="search">Search parameters</param>
        /// <returns>STOrders</returns>
        public IEnumerable<STOrder> GetAllForReleasing(SearchReleasing search)
        {
            IQueryable<STOrder> query = _context.STOrders
                                            .Where(p => p.WarehouseId == search.WarehouseId)
                                            .Include(p => p.Deliveries)
                                                .ThenInclude(p => p.ShowroomDeliveries)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size);

            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }


            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                query = (from x in query
                         select new STOrder
                         {
                             TransactionNo = x.TransactionNo,
                             StoreId = x.StoreId,
                             TransactionType = x.TransactionType,
                             DeliveryType = x.DeliveryType,
                             WarehouseId = x.WarehouseId,
                             PONumber = x.PONumber,
                             PODate = x.PODate,
                             Remarks = x.Remarks,
                             RequestStatus = x.RequestStatus,
                             OrderType = x.OrderType,
                             ClientName = x.ClientName,
                             ContactNumber = x.ContactNumber,
                             Address1 = x.Address1,
                             Address2 = x.Address2,
                             Address3 = x.Address3,
                             Deliveries = x.Deliveries.Where(y => y.DRNumber.ToLower() == search.DRNumber.ToLower()).ToList()
                         }).Where(x => x.Deliveries.Count > 0);
            }


            if (search.DeliveryDateFrom.HasValue)
            {
                query = query.Where(p => p.Deliveries.Any(x => search.DeliveryDateFrom <= x.ApprovedDeliveryDate) && p.Deliveries.Count > 0);
                //query = (from x in query
                //         select new STOrder
                //         {
                //             TransactionNo = x.TransactionNo,
                //             StoreId = x.StoreId,
                //             TransactionType = x.TransactionType,
                //             DeliveryType = x.DeliveryType,
                //             WarehouseId = x.WarehouseId,
                //             PONumber = x.PONumber,
                //             PODate = x.PODate,
                //             Remarks = x.Remarks,
                //             RequestStatus = x.RequestStatus,
                //             OrderType = x.OrderType,
                //             ClientName = x.ClientName,
                //             ContactNumber = x.ContactNumber,
                //             Address1 = x.Address1,
                //             Address2 = x.Address2,
                //             Address3 = x.Address3,
                //             Deliveries = x.Deliveries.Where(y => search.DeliveryDateFrom <= y.ApprovedDeliveryDate).ToList()
                //         }).Where(x => x.Deliveries.Count > 0);
            }


            if (search.DeliveryDateTo.HasValue)
            {
                query = query.Where(p => p.Deliveries.Any(x => search.DeliveryDateTo >= x.ApprovedDeliveryDate) && p.Deliveries.Count > 0);
                //query = (from x in query
                //         select new STOrder
                //         {
                //             TransactionNo = x.TransactionNo,
                //             StoreId = x.StoreId,
                //             TransactionType = x.TransactionType,
                //             DeliveryType = x.DeliveryType,
                //             WarehouseId = x.WarehouseId,
                //             PONumber = x.PONumber,
                //             PODate = x.PODate,
                //             Remarks = x.Remarks,
                //             RequestStatus = x.RequestStatus,
                //             OrderType = x.OrderType,
                //             ClientName = x.ClientName,
                //             ContactNumber = x.ContactNumber,
                //             Address1 = x.Address1,
                //             Address2 = x.Address2,
                //             Address3 = x.Address3,
                //             Deliveries = x.Deliveries.Where(y => search.DeliveryDateTo >= y.ApprovedDeliveryDate).ToList()
                //         }).Where(x => x.Deliveries.Count > 0);

            }



            List<STOrder> retList = new List<STOrder>();

            foreach (var r in query)
            {

                var order = new STOrder
                {
                    Id = r.Id,
                    TransactionNo = r.TransactionNo,
                    StoreId = r.StoreId,
                    Store = _context.Stores.Where(p => p.Id == r.StoreId).FirstOrDefault(),
                    TransactionType = r.TransactionType,
                    DeliveryType = r.DeliveryType,
                    WarehouseId = r.WarehouseId,
                    Warehouse = _context.Warehouses.Where(p => p.Id == r.WarehouseId).FirstOrDefault(),
                    PONumber = r.PONumber,
                    PODate = r.PODate,
                    Remarks = r.Remarks,
                    OrderType = r.OrderType,
                    ClientName = r.ClientName,
                    ContactNumber = r.ContactNumber,
                    Address1 = r.Address1,
                    Address2 = r.Address2,
                    Address3 = r.Address3,
                    RequestStatus = r.RequestStatus,
                    WHDRNumber = r.WHDRNumber
                };

                order.Deliveries = new List<STDelivery>();

                //if (order.OrderType == OrderTypeEnum.ClientOrder && order.DeliveryType == DeliveryTypeEnum.Pickup)
                //{
                //    //  STOrderDetail records by STOrder.Id
                //    var orders = _context.STOrderDetails
                //                        .Include(p => p.Item)
                //                            .ThenInclude(p => p.Size)
                //                    .Where(p => p.STOrderId == order.Id);

                //    foreach (var ord in orders)
                //    {
                //        var whStocks = _context.WHStocks.Where(p => p.STOrderDetailId == ord.Id && p.ItemId == ord.ItemId && p.ReleaseStatus == ReleaseStatusEnum.Waiting);
                //        if (whStocks != null && whStocks.Count() > 0)
                //        {
                //            if(order.OrderedItems == null)
                //            {
                //                order.OrderedItems = new List<STOrderDetail>();
                //            }

                //            order.OrderedItems.Add(ord);
                //        }
                //    }

                //    if (order.OrderedItems != null && order.OrderedItems.Count() > 0)
                //    {
                //        retList.Add(order);
                //    }
                //}
                //else
                //{
                foreach (var del in r.Deliveries)
                {
                    var delivery = new STDelivery
                    {
                        Id = del.Id,
                        ApprovedDeliveryDate = del.ApprovedDeliveryDate,
                        DeliveryDate = del.DeliveryDate,
                        DriverName = del.DriverName,
                        DRNumber = del.DRNumber,
                        PlateNumber = del.PlateNumber,
                        STOrderId = del.STOrderId,
                        StoreId = del.StoreId,
                        ShowroomDeliveries = new List<STShowroomDelivery>(),
                        ClientDeliveries = new List<STClientDelivery>(),
                        ORNumber = del.ORNumber,
                        SINumber = del.SINumber
                    };

                    if (order.OrderType == OrderTypeEnum.ShowroomStockOrder)
                    {
                        GetShowroomDeliveriesForReleasing(order, del, delivery, search);
                    }
                    else if (order.OrderType == OrderTypeEnum.ClientOrder)
                    {
                        if (order.DeliveryType == DeliveryTypeEnum.ShowroomPickup)
                        {
                            GetShowroomDeliveriesForReleasing(order, del, delivery, search);
                        }
                        else if (order.DeliveryType == DeliveryTypeEnum.Pickup)
                        {

                            GetClientPickUpForReleasing(order, del, delivery, search);

                        }
                        else
                        {
                            GetClientDeliveriesForReleasing(order, del, delivery, search);
                        }
                    }
                }

                if (order.Deliveries != null && order.Deliveries.Count > 0)
                {
                    retList.Add(order);
                }
                //}
            }

            if (search.DeliveryType != null)
            {
                if (search.DeliveryType[0] != null)
                {
                    return retList.Where(p => search.DeliveryType.Contains(p.DeliveryType)).OrderByDescending(p => p.Id);
                }

            }

            return retList.OrderByDescending(p => p.Id);
        }


        public object GetForShowroomReleasing(SearchReleasing search, AppSettings appSettings)
        {
            IQueryable<STOrder> query = _context.STOrders
                                            .Where(p => p.WarehouseId == search.WarehouseId)
                                            .Include(p => p.Deliveries)
                                                .ThenInclude(p => p.ShowroomDeliveries)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size);


            if (!string.IsNullOrWhiteSpace(search.PONumber)) {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }

            query = query.Where(p =>
                                      p.Deliveries.Any(x => (x.ShowroomDeliveries.Count > 0
                                      && x.ShowroomDeliveries.Any(c => c.DeliveryStatus == DeliveryStatusEnum.Waiting && c.ReleaseStatus == ReleaseStatusEnum.Waiting))
                                ));


            query = query.OrderByDescending(p => p.Id);
            var delQuery = query.SelectMany(p => p.Deliveries).ToList();

            GetAllResponse response = null;

            if (search.ShowAll == false)
            {
                response = new GetAllResponse(delQuery.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                //check if currentpage is greater than totalpage
                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;


                }

                delQuery = delQuery.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                            .Take(appSettings.RecordDisplayPerPage).ToList();



            }
            else
            {
                response = new GetAllResponse(delQuery.Count());
            }

            IQueryable<STOrder> stOrder = _context.STOrders;
            IQueryable<Warehouse> whouse = _context.Warehouses;
            IQueryable<Store> store = _context.Stores;
            List<DeliveryCustom> varlist = new List<DeliveryCustom>();

            foreach (var deliveries in delQuery)
            {

                var order = stOrder.Where(p => p.Id == deliveries.STOrderId).FirstOrDefault();

                var details = new STOrderCustomDTO
                {
                    Id = order.Id,
                    TransactionNo = order.TransactionNo,
                    StoreId = order.StoreId,
                    Store = store.Where(p => p.Id == order.StoreId).FirstOrDefault(),
                    TransactionType = order.TransactionType,
                    DeliveryType = order.DeliveryType,
                    WarehouseId = order.WarehouseId,
                    Warehouse = whouse.Where(p => p.Id == order.WarehouseId).FirstOrDefault(),
                    PONumber = order.PONumber,
                    PODate = order.PODate,
                    Remarks = order.Remarks,
                    OrderType = order.OrderType,
                    ClientName = order.ClientName,
                    ContactNumber = order.ContactNumber,
                    Address1 = order.Address1,
                    Address2 = order.Address2,
                    Address3 = order.Address3,
                    RequestStatus = order.RequestStatus,
                    WHDRNumber = order.WHDRNumber,
                    OrderTypeStr = order.OrderType != null ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), order.OrderType)) : null,
                    //TransactionTypeStr = order.TransactionType != null ? EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), order.TransactionType)) :  null
                };

                var del = new DeliveryCustom
                {
                    Id = deliveries.Id,
                    ApprovedDeliveryDate = deliveries.ApprovedDeliveryDate,
                    DeliveryDate = deliveries.DeliveryDate,
                    DriverName = deliveries.DriverName,
                    DRNumber = deliveries.DRNumber,
                    PlateNumber = deliveries.PlateNumber,
                    STOrderId = deliveries.STOrderId,
                    StoreId = deliveries.StoreId,
                    ShowroomDeliveries = deliveries.ShowroomDeliveries,
                    ClientDeliveries = deliveries.ClientDeliveries,
                    ORNumber = deliveries.ORNumber,
                    SINumber = deliveries.SINumber,
                    Details = details

                };
                varlist.Add(del);

            }
            response.List.AddRange(varlist);

            return response;
        }



        //Replacement for GetAllForReleasing
        public object GetAllForReleasingPaged(SearchReleasing search, AppSettings appSettings)
        {
            IQueryable<STOrder> query = _context.STOrders
                                            .Where(p => p.WarehouseId == search.WarehouseId)
                                            .Include(p => p.Deliveries)
                                                .ThenInclude(p => p.ShowroomDeliveries)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                              .Include(p => p.Deliveries)
                                                .ThenInclude(p => p.ClientDeliveries)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size);

            var ForModifications = _context.WHModifyItemTonality
                                            .Where(p => p.RequestStatus == RequestStatusEnum.Pending && p.WarehouseId == search.WarehouseId)
                                                .Select(p => p.STOrderId).ToList();

            if(ForModifications.Count() > 0)
            {
                query = query.Where(p => !ForModifications.Contains(p.Id));
            }



            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }

            //query = query.Where(p => p.Deliveries.Any(x => x.ClientDeliveries.Count > 0) 
            //? p.Deliveries.Any(x => x.ClientDeliveries.Any(c =>
            //                                                     search.ReleaseStatus == ReleaseStatusEnum.Released 
            //                                                     ?  c.ReleaseStatus == search.ReleaseStatus 
            //                                                     : (c.DeliveryStatus == DeliveryStatusEnum.Waiting && c.ReleaseStatus == ReleaseStatusEnum.Waiting))) 
            //: p.Deliveries.Any(x => x.ShowroomDeliveries.Any(c =>
            //                                                     search.ReleaseStatus == ReleaseStatusEnum.Released
            //                                                     ? c.ReleaseStatus == search.ReleaseStatus 
            //                                                     : (c.DeliveryStatus == DeliveryStatusEnum.Waiting && c.ReleaseStatus == ReleaseStatusEnum.Waiting))) );

            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                query = query.Where(p => p.Deliveries.Any(x => x.DRNumber.ToLower() == search.DRNumber.ToLower()) && p.Deliveries.Count > 0);

            }


            if (search.DeliveryDateFrom.HasValue)
            {
                query = query.Where(p => p.Deliveries.Any(x => search.DeliveryDateFrom <= x.ApprovedDeliveryDate) && p.Deliveries.Count > 0);

            }


            if (search.DeliveryDateTo.HasValue)
            {
                query = query.Where(p => p.Deliveries.Any(x => search.DeliveryDateTo >= x.ApprovedDeliveryDate) && p.Deliveries.Count > 0);

            }

            if (search.DeliveryType != null)
            {
                if (search.DeliveryType[0] != null)
                {
                    query = query.Where(p => search.DeliveryType.Contains(p.DeliveryType)).OrderByDescending(p => p.Id);
                }

            }


            if (!string.IsNullOrWhiteSpace(search.WHDRNumber))
            {
                query = query.Where(p => p.WHDRNumber.ToLower() == search.WHDRNumber.ToLower());
            }





                GetAllResponse response = null;

            query = query.OrderByDescending(p => p.Id);
            //List<STDelivery> delQuery = new List<STDelivery>();
            IQueryable<STDelivery> delQuery = null;

            if (search.ReleaseStatus == ReleaseStatusEnum.Released)
            {
                delQuery = query.SelectMany(p => p.Deliveries.Where(
                    x => (x.ClientDeliveries.Any(c => c.ReleaseStatus == ReleaseStatusEnum.Released))
                          ||
                         (x.ShowroomDeliveries.Any(s => s.ReleaseStatus == ReleaseStatusEnum.Released))

                ));
            }
            else
            {
                delQuery = query.SelectMany(p => p.Deliveries.Where(
                x => (x.ClientDeliveries.Any(c => c.ReleaseStatus == ReleaseStatusEnum.Waiting && c.DeliveryStatus == DeliveryStatusEnum.Waiting))
                      ||
                     (x.ShowroomDeliveries.Any(s => s.ReleaseStatus == ReleaseStatusEnum.Waiting && s.DeliveryStatus == DeliveryStatusEnum.Waiting))

                ));
            }


            if (search.ShowAll == false)
            {
                response = new GetAllResponse(delQuery.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                //check if currentpage is greater than totalpage
                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;


                }

                delQuery = delQuery.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                            .Take(appSettings.RecordDisplayPerPage);



            }
            else
            {
                response = new GetAllResponse(delQuery.Count());
            }




            IQueryable<STOrder> stOrder = _context.STOrders.Where(p => p.WarehouseId == search.WarehouseId);
            IQueryable<Warehouse> whouse = _context.Warehouses;
            IQueryable<STShowroomDelivery> stShowroom = _context.STShowroomDeliveries
                                                                    .Include(p => p.Item)
                                                                        .ThenInclude(p => p.Size);
            IQueryable<STClientDelivery> stClient = _context.STClientDeliveries
                                                                        .Include(p => p.Item)
                                                                            .ThenInclude(p => p.Size); ;
            IQueryable<Store> store = _context.Stores;

            List<DeliveryCustom> retList = new List<DeliveryCustom>();

            foreach (var deliveries in delQuery)
            {
                var details = query.Where(p => p.Id == deliveries.STOrderId).FirstOrDefault();


                var order = new STOrderCustomDTO
                {
                    Id = details.Id,
                    TransactionNo = details.TransactionNo,
                    StoreId = details.StoreId,
                    Store = _context.Stores.Where(p => p.Id == details.StoreId).FirstOrDefault(),
                    TransactionType = details.TransactionType,
                    DeliveryType = details.DeliveryType,
                    WarehouseId = details.WarehouseId,
                    Warehouse = _context.Warehouses.Where(p => p.Id == details.WarehouseId).FirstOrDefault(),
                    PONumber = details.PONumber,
                    PODate = details.PODate,
                    Remarks = details.Remarks,
                    OrderType = details.OrderType,
                    ClientName = details.ClientName,
                    ContactNumber = details.ContactNumber,
                    Address1 = details.Address1,
                    Address2 = details.Address2,
                    Address3 = details.Address3,
                    RequestStatus = details.RequestStatus,
                    WHDRNumber = details.WHDRNumber,
                    OrderTypeStr = details.OrderType != null ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), details.OrderType)) : null,
                    TransactionTypeStr = details.TransactionType != null ? EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), details.TransactionType)) : null,


                };

                var del = new DeliveryCustom
                {
                    Id = deliveries.Id,
                    ApprovedDeliveryDate = deliveries.ApprovedDeliveryDate,
                    DeliveryDate = deliveries.DeliveryDate,
                    DriverName = deliveries.DriverName,
                    DRNumber = deliveries.DRNumber,
                    PlateNumber = deliveries.PlateNumber,
                    STOrderId = deliveries.STOrderId,
                    StoreId = deliveries.StoreId,
                    ShowroomDeliveries = stShowroom.Where(p => p.STDeliveryId == deliveries.Id).ToList(),
                    ClientDeliveries = stClient.Where(p => p.STDeliveryId == deliveries.Id).ToList(),
                    ORNumber = deliveries.ORNumber,
                    SINumber = deliveries.SINumber,
                    //adding order details for display
                    Details = order,


                };


                if (del != null)
                {
                    retList.Add(del);
                }

            }


            response.List.AddRange(retList);

            return response;
        }

        // get Orders that items can be changed
        public object GetAllForTonalityReplacement(SearchReleasing search, AppSettings appSettings)
        {
            IQueryable<STOrder> query = _context.STOrders
                                            .Where(p => p.WarehouseId == search.WarehouseId)
                                            .Include(p => p.Deliveries)
                                                .ThenInclude(p => p.ShowroomDeliveries)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                              .Include(p => p.Deliveries)
                                                .ThenInclude(p => p.ClientDeliveries)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size);

            var orderWithItemModification = _context.WHModifyItemTonality
                                                        .Where(p => p.WarehouseId == search.WarehouseId && p.RequestStatus == RequestStatusEnum.Pending)
                                                            .Select(p => p.STOrderId).Distinct();

            if(orderWithItemModification.Count() > 0)
            {
                query = query.Where(p => !orderWithItemModification.Contains(p.Id));
            }


            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                query = query.Where(p => p.Deliveries.Any(x => x.DRNumber.ToLower() == search.DRNumber.ToLower()) && p.Deliveries.Count > 0);

            }


            if (search.DeliveryDateFrom.HasValue)
            {
                query = query.Where(p => p.Deliveries.Any(x => search.DeliveryDateFrom <= x.ApprovedDeliveryDate) && p.Deliveries.Count > 0);

            }


            if (search.DeliveryDateTo.HasValue)
            {
                query = query.Where(p => p.Deliveries.Any(x => search.DeliveryDateTo >= x.ApprovedDeliveryDate) && p.Deliveries.Count > 0);

            }

            if (search.DeliveryType != null)
            {
                if (search.DeliveryType[0] != null)
                {
                    query = query.Where(p => search.DeliveryType.Contains(p.DeliveryType)).OrderByDescending(p => p.Id);
                }

            }


            if (!string.IsNullOrWhiteSpace(search.WHDRNumber))
            {
                query = query.Where(p => p.WHDRNumber.ToLower() == search.WHDRNumber.ToLower());
            }




            GetAllResponse response = null;

            query = query.OrderByDescending(p => p.Id);

            List<STDelivery> delQuery = new List<STDelivery>();

            // Get Id with item already released will not be included on the list
            var stOrderIds = query.SelectMany(p => p.Deliveries.Where(
                     x => (x.ClientDeliveries.Any(c => c.ReleaseStatus == ReleaseStatusEnum.Released))
                           ||
                          (x.ShowroomDeliveries.Any(s => s.ReleaseStatus == ReleaseStatusEnum.Released))

                 )).Select(z => z.STOrderId).ToList();

         
                delQuery = query.SelectMany(p => p.Deliveries.Where(
                x => (x.ClientDeliveries.Any(c => c.ReleaseStatus == ReleaseStatusEnum.Waiting && c.DeliveryStatus == DeliveryStatusEnum.Waiting))
                      ||
                     (x.ShowroomDeliveries.Any(s => s.ReleaseStatus == ReleaseStatusEnum.Waiting && s.DeliveryStatus == DeliveryStatusEnum.Waiting))

                )).ToList();

            
    
            // Filter list base on storderId that has items released
            delQuery = delQuery.Where(p => !stOrderIds.Contains(p.STOrderId)).ToList();


            if (search.ShowAll == false)
            {
                response = new GetAllResponse(delQuery.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                //check if currentpage is greater than totalpage
                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;


                }

                delQuery = delQuery.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                            .Take(appSettings.RecordDisplayPerPage).ToList();



            }
            else
            {
                response = new GetAllResponse(delQuery.Count());
            }




            IQueryable<STOrder> stOrder = _context.STOrders.Where(p => p.WarehouseId == search.WarehouseId);
            IQueryable<Warehouse> whouse = _context.Warehouses;
            IQueryable<STShowroomDelivery> stShowroom = _context.STShowroomDeliveries
                                                                    .Include(p => p.Item)
                                                                        .ThenInclude(p => p.Size);
            IQueryable<STClientDelivery> stClient = _context.STClientDeliveries
                                                                        .Include(p => p.Item)
                                                                            .ThenInclude(p => p.Size); ;
            IQueryable<Store> store = _context.Stores;

            List<DeliveryCustom> retList = new List<DeliveryCustom>();

            foreach (var deliveries in delQuery)
            {
                var details = query.Where(p => p.Id == deliveries.STOrderId).FirstOrDefault();


                var order = new STOrderCustomDTO
                {
                    Id = details.Id,
                    TransactionNo = details.TransactionNo,
                    StoreId = details.StoreId,
                    Store = _context.Stores.Where(p => p.Id == details.StoreId).FirstOrDefault(),
                    TransactionType = details.TransactionType,
                    DeliveryType = details.DeliveryType,
                    DeliveryTypeStr = details.DeliveryType != null ? EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), details.DeliveryType)) : null,
                    WarehouseId = details.WarehouseId,
                    Warehouse = _context.Warehouses.Where(p => p.Id == details.WarehouseId).FirstOrDefault(),
                    PONumber = details.PONumber,
                    PODate = details.PODate,
                    Remarks = details.Remarks,
                    OrderType = details.OrderType,
                    ClientName = details.ClientName,
                    ContactNumber = details.ContactNumber,
                    Address1 = details.Address1,
                    Address2 = details.Address2,
                    Address3 = details.Address3,
                    RequestStatus = details.RequestStatus,
                    WHDRNumber = details.WHDRNumber,
                    OrderTypeStr = details.OrderType != null ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderTypeEnum), details.OrderType)) : null,
                    TransactionTypeStr = details.TransactionType != null ? EnumExtensions.SplitName(Enum.GetName(typeof(TransactionTypeEnum), details.TransactionType)) : null,


                };

                var del = new DeliveryCustom
                {
                    Id = deliveries.Id,
                    ApprovedDeliveryDate = deliveries.ApprovedDeliveryDate,
                    DeliveryDate = deliveries.DeliveryDate,
                    DriverName = deliveries.DriverName,
                    DRNumber = deliveries.DRNumber,
                    PlateNumber = deliveries.PlateNumber,
                    STOrderId = deliveries.STOrderId,
                    StoreId = deliveries.StoreId,
                    ShowroomDeliveries = stShowroom.Where(p => p.STDeliveryId == deliveries.Id).ToList(),
                    ClientDeliveries = stClient.Where(p => p.STDeliveryId == deliveries.Id).ToList(),
                    ORNumber = deliveries.ORNumber,
                    SINumber = deliveries.SINumber,
                    //adding order details for display
                    Details = order,


                };


                if (del != null)
                {
                    retList.Add(del);
                }

            }


            response.List.AddRange(retList);

            return response;
        }



        public void AddModifyItemTonality(WHModifyItemTonalityDTO record)
        {

            
            if(record != null)
            {
                if(record.ModifyItemTonalityDetails.Where(p => p.ItemId != null).Count() > 0)
                {

                    var itemForTonalityChange = new List<WHModifyItemTonalityDetails>();
        

                    foreach(var rec in record.ModifyItemTonalityDetails)
                    {
                        if(rec.ItemId.HasValue)
                        {
                            var item = new WHModifyItemTonalityDetails
                            {
                                ItemId = rec.ItemId,
                                OldItemId = rec.OldItemId,
                                RequestStatus = RequestStatusEnum.Pending,
                                Remarks = rec.Remarks,
                                DateCreated = DateTime.Now,
                                StClientDeliveryId = rec.StClientDeliveryId,
                                StShowroomDeliveryId = rec.StShowroomDeliveryId,

                            };

                            itemForTonalityChange.Add(item);

                        }
                    }


                    var modifiedRecord = new WHModifyItemTonality
                    {
                        WarehouseId = record.WarehouseId,
                        STOrderId = record.STOrderId,
                        Remarks = record.Remarks,
                        ModifyItemTonalityDetails = itemForTonalityChange,
                        DateCreated = DateTime.Now,
                        RequestStatus = RequestStatusEnum.Pending
                    };


                    _context.WHModifyItemTonality.Add(modifiedRecord);
                    _context.SaveChanges();


                    var modifyTonService = new ModifyTonalityService(_context);

                    var dto = new ApproveModifyTonalityDTO();
                    dto.Id = modifiedRecord.Id;
                    dto.RequestStatus = RequestStatusEnum.Approved;


                    modifyTonService.ApproveModifyTonality(dto);

                }


               
            }

        }

        private void GetClientPickUpForReleasing(STOrder order, STDelivery del, STDelivery delivery,SearchReleasing search)
        {
            if (del.ClientDeliveries == null)
            {
                del.ClientDeliveries = _context.STClientDeliveries
                                                .Include(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Where(p => p.STDeliveryId == del.Id)
                                            .ToList();
            }


            if (del.ClientDeliveries != null && del.ClientDeliveries.Count > 0)
            {
                foreach (var client in del.ClientDeliveries.Where(p => search.ReleaseStatus == ReleaseStatusEnum.Released ?
                p.ReleaseStatus == search.ReleaseStatus 
                : p.DeliveryStatus == DeliveryStatusEnum.Waiting && p.ReleaseStatus == ReleaseStatusEnum.Waiting))
                {
                    delivery.ClientDeliveries.Add(client);
                }

                if (delivery.ClientDeliveries != null && delivery.ClientDeliveries.Count > 0)
                {
                    order.Deliveries.Add(delivery);
                }
            }
        }

        private void GetClientDeliveriesForReleasing(STOrder order, STDelivery del, STDelivery delivery, SearchReleasing search)
        {
            if (del.ClientDeliveries == null)
            {
                del.ClientDeliveries = _context.STClientDeliveries
                                                .Include(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Where(p => p.STDeliveryId == del.Id)
                                            .ToList();
            }


            if (del.ClientDeliveries != null && del.ClientDeliveries.Count > 0)
            {
                foreach (var client in del.ClientDeliveries.Where(p => 
                search.ReleaseStatus == ReleaseStatusEnum.Released
                ? p.ReleaseStatus == search.ReleaseStatus 
                : p.DeliveryStatus == DeliveryStatusEnum.Waiting && p.ReleaseStatus == ReleaseStatusEnum.Waiting))
                {
                    delivery.ClientDeliveries.Add(client);
                }

                if (delivery.ClientDeliveries != null && delivery.ClientDeliveries.Count > 0)
                {
                    order.Deliveries.Add(delivery);
                }
            }
        }

        private void GetShowroomDeliveriesForReleasing(STOrder order, STDelivery del, STDelivery delivery,SearchReleasing search)
        {
            if (del.ShowroomDeliveries == null)
            {
                del.ShowroomDeliveries = _context.STShowroomDeliveries
                                                .Include(p => p.Item)
                                                    .ThenInclude(p => p.Size)
                                            .Where(p => p.STDeliveryId == del.Id)
                                            .ToList();
            }


            if (del.ShowroomDeliveries != null && del.ShowroomDeliveries.Count > 0)
            {

                foreach (var show in del.ShowroomDeliveries.Where(p =>
                search.ReleaseStatus == ReleaseStatusEnum.Released 
                    ? p.ReleaseStatus == search.ReleaseStatus 
                    :  p.DeliveryStatus == DeliveryStatusEnum.Waiting && p.ReleaseStatus == ReleaseStatusEnum.Waiting))
                {
                    delivery.ShowroomDeliveries.Add(show);
                }

                if (delivery.ShowroomDeliveries != null && delivery.ShowroomDeliveries.Count > 0)
                {
                    order.Deliveries.Add(delivery);
                }
            }
        }


        /// <summary>
        /// Get for releasing by id
        /// </summary>
        /// <param name="id">STDelivery.Id</param>
        /// <returns>STOrder</returns>
        public STOrder GetReleasingById(int? id, int? warehouseId)
        {
            var obj = _context.STDeliveries
                        .Where(p => p.Id == id)
                        .FirstOrDefault();

            if (obj == null)
            {
                return null;
            }

            //  Get order record
            var retOrder = _context.STOrders
                        .Include(p => p.OrderedItems)
                        .Where(p => p.WarehouseId == warehouseId
                                    && p.Id == obj.STOrderId)
                            .Include(p => p.Warehouse).AsNoTracking()
                        .FirstOrDefault();

            if (retOrder == null)
            {
                return null;
            }

            if (retOrder.OrderType == OrderTypeEnum.ClientOrder && retOrder.DeliveryType != DeliveryTypeEnum.ShowroomPickup)
            {

                var clientDeliveries = _context.STClientDeliveries.Where(p => p.STDeliveryId == obj.Id).ToList();
                if (clientDeliveries != null && clientDeliveries.Count() > 0)
                {
                    obj.ClientDeliveries = clientDeliveries;
                }

            }
            else
            {
                var showroomDeliveries = _context.STShowroomDeliveries.Where(p => p.STDeliveryId == obj.Id).ToList();
                if (showroomDeliveries != null && showroomDeliveries.Count() > 0)
                {
                    obj.ShowroomDeliveries = showroomDeliveries;
                }
            }

            if (retOrder.Deliveries == null)
            {
                retOrder.Deliveries = new List<STDelivery>();
            }

            retOrder.Deliveries.Add(obj);

            return retOrder;
        }

        public void UpdateForReleasing(int? id, int? warehouseId)
        {
            var record = this.GetReleasingById(id, warehouseId);
            if (record != null)
            {
                foreach (var del in record.Deliveries)
                {
                    if (record.OrderType == OrderTypeEnum.ClientOrder && record.DeliveryType != DeliveryTypeEnum.ShowroomPickup)
                    {
                        ReleaseClientOrder(record, del);
                    }
                    //else if (record.OrderType == OrderTypeEnum.ClientOrder && record.DeliveryType == DeliveryTypeEnum.Pickup)
                    //{
                    //    ReleaseClientOrder(record, del);
                    //}
                    else
                    {
                        ReleaseShowroomOrder(record, del);
                    }
                }
            }
        }

        private void ReleaseShowroomOrder(STOrder record, STDelivery del)
        {
            foreach (var show in del.ShowroomDeliveries)
            {
                //  Assign release status
                show.ReleaseStatus = ReleaseStatusEnum.Released;
                show.DateUpdated = DateTime.Now;

                var whStock = _context.WHStocks
                                .Where(p => p.STShowroomDeliveryId == show.Id
                                            && p.ItemId == show.ItemId).FirstOrDefault();

                if (whStock != null)
                {
                    //  Assign release status
                    whStock.DeliveryStatus = DeliveryStatusEnum.Waiting;
                    whStock.ReleaseStatus = ReleaseStatusEnum.Released;
                    whStock.DateReleased = DateTime.Now;
                    whStock.DateUpdated = DateTime.Now;

                    _context.WHStocks.Update(whStock);
                    _context.SaveChanges();
                }

                if (record.OrderType == OrderTypeEnum.ClientOrder && record.DeliveryType == DeliveryTypeEnum.Pickup)
                {
                    MarkOrderDetailRecordAsReleasedAndSaveReleaseDate(show.STOrderDetailId);
                }
            }

            if (record.OrderType == OrderTypeEnum.ClientOrder && record.DeliveryType == DeliveryTypeEnum.Pickup)
            {
                record.ReleaseDate = DateTime.Now;
                record.DateUpdated = DateTime.Now;
            }


            del.ReleaseDate = DateTime.Now;
            del.DateUpdated = DateTime.Now;

            

            _context.STDeliveries.Update(del);
            _context.SaveChanges();

        }

        private void MarkOrderDetailRecordAsReleasedAndSaveReleaseDate(int? STOrderDetailId)
        {
            var objSTOrderDetail = _context.STOrderDetails
                                                           .Where(p => p.Id == STOrderDetailId)
                                                           .FirstOrDefault();
            if (objSTOrderDetail != null)
            {
                objSTOrderDetail.ReleaseStatus = ReleaseStatusEnum.Released;
                objSTOrderDetail.DeliveryStatus = DeliveryStatusEnum.Delivered;
                objSTOrderDetail.DateReleased = DateTime.Now;

                _context.STOrderDetails.Update(objSTOrderDetail);
                _context.SaveChanges();
            }
        }

        private void ReleaseClientOrder(STOrder order, STDelivery del)
        {

            var stockService = new STStockService(_context);


            //  Add sales record
            var sales = new STSales
            {
                STOrderId = del.STOrderId,
                StoreId = del.StoreId,
                SINumber = del.SINumber,
                DRNumber = del.DRNumber,
                ORNumber = del.ORNumber,
                ClientName = del.ClientName,
                ContactNumber = del.ContactNumber,
                Address1 = del.Address1,
                Address2 = del.Address2,
                Address3 = del.Address3,
                SoldItems = new List<STSalesDetail>(),
                DeliveryType = order.DeliveryType,
                SalesType = SalesTypeEnum.ClientOrder,
                STDeliveryId = del.Id,
                // added for ticket #632 so it will appear on returns
                ReleaseDate = DateTime.Now
               
            };

            foreach (var client in del.ClientDeliveries)
            {
                client.DeliveredQuantity = client.Quantity;

                //  Assign release status
                client.ReleaseStatus = ReleaseStatusEnum.Released;
                client.DeliveryStatus = DeliveryStatusEnum.Delivered;
                client.DateUpdated = DateTime.Now;

                var whStock = _context.WHStocks
                                .Where(p => p.STClientDeliveryId == client.Id
                                            && p.ItemId == client.ItemId).FirstOrDefault();

                if (whStock != null)
                {
                    //  Assign release status
                    whStock.ReleaseStatus = ReleaseStatusEnum.Released;
                    whStock.DeliveryStatus = DeliveryStatusEnum.Delivered;
                    whStock.DateReleased = DateTime.Now;
                    whStock.DateUpdated = DateTime.Now;

                    _context.WHStocks.Update(whStock);
                    _context.SaveChanges();


                    var salesDetail = new STSalesDetail
                    {
                        ItemId = client.ItemId,
                        Quantity = client.Quantity,
                        DeliveryStatus = DeliveryStatusEnum.Delivered
                    };

                    sales.SoldItems.Add(salesDetail);
                    //added for ticket #328 creation of stock moved to logistic update
                    if (order.DeliveryType != DeliveryTypeEnum.Delivery && order.OrderType == OrderTypeEnum.ClientOrder)
                        {
                            var stStock = new STStock
                            {
                                STClientDeliveryId = client.Id,
                                StoreId = order.StoreId,
                                ItemId = client.ItemId,
                                OnHand = client.Quantity,
                                DeliveryStatus = order.DeliveryType == DeliveryTypeEnum.Pickup ? DeliveryStatusEnum.Delivered : DeliveryStatusEnum.Waiting,
                                ReleaseStatus = ReleaseStatusEnum.Released
                            };
                      

                            stockService.InsertSTStock(stStock);
                        }
                    //else
                    //{//added for ticket #328 creation of stock moved to logistic update
                    //    var stock =  _context.STStocks.Where(p => p.ItemId == client.ItemId && p.STClientDeliveryId == client.Id && p.DeliveryStatus == DeliveryStatusEnum.Waiting).FirstOrDefault();

                    //    stock.DeliveryStatus = DeliveryStatusEnum.Delivered;
                    //    stockService.UpdateSTStock(stock);
                    //}

                }

            }

            if (order.DeliveryType == DeliveryTypeEnum.Delivery)
            {
                del.Delivered = DeliveryStatusEnum.Waiting;
            }

            del.ReleaseDate = DateTime.Now;
            del.DateUpdated = DateTime.Now;

            _context.STDeliveries.Update(del);
            _context.SaveChanges();

            foreach (var client in del.ClientDeliveries)
            {
                var objSTOrderDetail = _context.STOrderDetails
                                               .Where(p => p.Id == client.STOrderDetailId)
                                               .FirstOrDefault();
                if (objSTOrderDetail != null)
                {
                    var totalDeliveredAndReleased = Convert.ToInt32(
                                                        _context.STClientDeliveries
                                                                .Where(p => p.STOrderDetailId == objSTOrderDetail.Id
                                                                            && p.ItemId == objSTOrderDetail.ItemId
                                                                            && p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                                            && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                                .Sum(p => p.DeliveredQuantity)
                                                    );

                    if (totalDeliveredAndReleased == objSTOrderDetail.ApprovedQuantity)
                    {
                        objSTOrderDetail.DeliveryStatus = DeliveryStatusEnum.Delivered;
                        objSTOrderDetail.ReleaseStatus = ReleaseStatusEnum.Released;
                        objSTOrderDetail.DateReleased = DateTime.Now;

                        _context.STOrderDetails.Update(objSTOrderDetail);
                        _context.SaveChanges();
                    }

                }
            }

            if (sales.SoldItems.Count > 0)
            {
                new STSalesService(_context).InsertSales(sales);

                //added for ticket #328 creation of stock moved to logistic update
                if (order.DeliveryType != DeliveryTypeEnum.Delivery && order.OrderType == OrderTypeEnum.ClientOrder)
                {
                    foreach (var sold in sales.SoldItems)
                    {
                        var stStock = new STStock
                        {
                            STSalesDetailId = sold.Id,
                            StoreId = order.StoreId,
                            ItemId = sold.ItemId,
                            OnHand = -sold.Quantity,
                            DeliveryStatus = DeliveryStatusEnum.Delivered,
                            ReleaseStatus = ReleaseStatusEnum.Released
                        };

                        stockService.InsertSTStock(stStock);
                    }
                }

            }

            var objSTOrder = _context.STOrders
                                     .Include(p => p.OrderedItems)
                                     .Where(p => p.Id == order.Id)
                                     .FirstOrDefault();

            if (objSTOrder.OrderedItems != null && objSTOrder.OrderedItems.Count > 0)
            {
                //  Check if all ordered items are delivered and released
                if (objSTOrder.OrderedItems.Where(p => p.DeliveryStatus != DeliveryStatusEnum.Delivered
                                                    && p.ReleaseStatus != ReleaseStatusEnum.Released)
                                        .Count() == 0)
                {
                    objSTOrder.ReleaseDate = DateTime.Now;
                    objSTOrder.DateUpdated = DateTime.Now;

                    if(objSTOrder.DeliveryType == DeliveryTypeEnum.Pickup && objSTOrder.OrderType == OrderTypeEnum.ClientOrder)
                    {
                        objSTOrder.OrderStatus = OrderStatusEnum.Completed;
                    }
                  

                    _context.STOrders.Update(objSTOrder);
                    _context.SaveChanges();
                }

            }
        }


        /// <summary>
        /// Update client order
        /// </summary>
        /// <param name="salesService"></param>
        /// <param name="id">STOrder.Id</param>
        public void UpdateClientOrderForReleasing(STOrder record, ISTSalesService salesService, ISTStockService stockService, int id, int? warehouseId)
        {

            if (record.OrderedItems != null && record.OrderedItems.Count > 0)
            {

                //  Add sales record
                var sales = new STSales
                {
                    Id = 0,
                    STOrderId = record.Id,
                    StoreId = record.StoreId,
                    ClientName = record.ClientName,
                    ContactNumber = record.ContactNumber,
                    Address1 = record.Address1,
                    Address2 = record.Address2,
                    Address3 = record.Address3,
                    SoldItems = new List<STSalesDetail>(),
                    DeliveryType = record.DeliveryType,
                    SalesType = SalesTypeEnum.ClientOrder
                };

                foreach (var del in record.OrderedItems)
                {

                    var whStock = _context.WHStocks
                                    .Where(p => p.STOrderDetailId == del.Id && p.ItemId == del.ItemId).FirstOrDefault();

                    if (whStock != null)
                    {

                        //  Mark record as released
                        del.ReleaseStatus = ReleaseStatusEnum.Released;
                        del.DateReleased = DateTime.Now;

                        //  Mark record as delivered
                        del.DeliveryStatus = DeliveryStatusEnum.Delivered;
                        del.DateUpdated = DateTime.Now;

                        //  Assign release status and delivery status
                        whStock.ReleaseStatus = ReleaseStatusEnum.Released;
                        whStock.DateReleased = DateTime.Now;
                        whStock.DeliveryStatus = DeliveryStatusEnum.Delivered;
                        whStock.DateUpdated = DateTime.Now;

                        _context.WHStocks.Update(whStock);
                        _context.SaveChanges();

                        //  Add sales details
                        var item = new STSalesDetail
                        {
                            Id = 0,
                            ItemId = del.ItemId,
                            Quantity = del.ApprovedQuantity,
                            DeliveryStatus = DeliveryStatusEnum.Delivered
                        };

                        sales.SoldItems.Add(item);

                        //  Check order it for Pickup or for Delivery
                        if (record.DeliveryType == DeliveryTypeEnum.Pickup)
                        {
                            //  Add each record to STStock
                            var stStock = new STStock
                            {
                                StoreId = record.StoreId,
                                STOrderDetailId = del.Id,
                                ItemId = del.ItemId,
                                OnHand = del.ApprovedQuantity,
                                DeliveryStatus = DeliveryStatusEnum.Delivered
                            };

                            stockService.InsertSTStock(stStock);
                        }
                    }
                }

                record.ReleaseDate = DateTime.Now;
                record.DateUpdated = DateTime.Now;

                _context.STOrders.Update(record);
                _context.SaveChanges();

                if (sales.SoldItems.Count > 0)
                {
                    salesService.InsertSales(sales);

                    //  Check order it for Pickup or for Delivery
                    if (record.DeliveryType == DeliveryTypeEnum.Pickup)
                    {
                        //  Deduct each record from STSTock
                        foreach (var item in sales.SoldItems)
                        {
                            var stStock = new STStock
                            {
                                StoreId = record.StoreId,
                                STSalesDetailId = item.Id,
                                ItemId = item.ItemId,
                                OnHand = -item.Quantity,
                                DeliveryStatus = DeliveryStatusEnum.Delivered,
                                ReleaseStatus = ReleaseStatusEnum.Released
                            };

                            stockService.InsertSTStock(stStock);
                        }
                    }

                }
            }

        }


        /// <summary>
        /// Get client's order for releasing by id
        /// </summary>
        /// <param name="id">STOrder.Id</param>
        /// <returns>STOrder</returns>
        public STOrder GetClientOrderReleasingByIdAndWarehouseId(int? id, int? warehouseId)
        {
            var rec = _context.STOrders
                        .Where(p => p.WarehouseId == warehouseId &&
                                    p.Id == id &&
                                    p.OrderType == OrderTypeEnum.ClientOrder &&
                                    p.DeliveryType == DeliveryTypeEnum.Pickup)
                        .Include(p => p.Store)
                        .Include(p => p.Warehouse)
                        .Include(p => p.OrderedItems)
                            .ThenInclude(p => p.Item)
                                .ThenInclude(p => p.Size).AsNoTracking()
                        .FirstOrDefault();

            STOrder retRecord = null;

            if (rec != null)
            {
                if (rec.OrderedItems != null && rec.OrderedItems.Count() > 0)
                {
                    retRecord = new STOrder
                    {
                        Id = rec.Id,
                        TransactionNo = rec.TransactionNo,
                        StoreId = rec.StoreId,
                        Store = _context.Stores.Where(p => p.Id == rec.StoreId).FirstOrDefault(),
                        TransactionType = rec.TransactionType,
                        DeliveryType = rec.DeliveryType,
                        OrderType = rec.OrderType,
                        WarehouseId = rec.WarehouseId,
                        Warehouse = _context.Warehouses.Where(p => p.Id == rec.WarehouseId).FirstOrDefault(),
                        PONumber = rec.PONumber,
                        PODate = rec.PODate,
                        Remarks = rec.Remarks,
                        RequestStatus = rec.RequestStatus,
                        ClientName = rec.ClientName,
                        ContactNumber = rec.ContactNumber,
                        Address1 = rec.Address1,
                        Address2 = rec.Address2,
                        Address3 = rec.Address3,
                        DateCreated = rec.DateCreated,
                        OrderToStoreId = rec.OrderToStoreId,
                        STTransferId = rec.STTransferId
                    };

                    retRecord.OrderedItems = new List<STOrderDetail>();

                    foreach (var ord in rec.OrderedItems)
                    {
                        var orderObj = new STOrderDetail
                        {
                            Id = ord.Id,
                            STOrderId = ord.STOrderId,
                            ItemId = ord.ItemId,
                            Item = _context.Items.Where(p => p.Id == ord.ItemId).FirstOrDefault(),
                            RequestedQuantity = ord.RequestedQuantity,
                            RequestedRemarks = ord.RequestedRemarks,
                            ApprovedQuantity = ord.ApprovedQuantity,
                            ApprovedRemarks = ord.ApprovedRemarks,
                            DeliveryStatus = ord.DeliveryStatus
                        };

                        //  Get record form WHStock
                        var whStock = _context.WHStocks
                                            .Include(p => p.Item)
                                                .ThenInclude(p => p.Size)
                                            .Where(p => p.STOrderDetailId == ord.Id && p.ItemId == ord.ItemId && p.ReleaseStatus == ReleaseStatusEnum.Waiting).FirstOrDefault();
                        if (whStock != null)
                        {
                            retRecord.OrderedItems.Add(orderObj);
                        }
                    }
                }

                if (retRecord.OrderedItems != null && retRecord.OrderedItems.Count() > 0)
                {
                    return retRecord;
                }
            }



            return null;
        }


        public void AddClientSINumber(AddClientSIDTO dto, int? storeId)
        {


            var order = _context.STOrders.Where(p => p.Id == dto.Id && p.StoreId == storeId).FirstOrDefault();

            if(order != null)
            {
                order.ClientSINumber = dto.ClientSINumber;    
            }



            _context.STOrders.Update(order);
            _context.SaveChanges();

        }


        private int getPOCount(string PONumber)
        {
            var poCount = _context.STOrders.Where(p => p.PONumber.ToLower() == PONumber.ToLower()).Count();

            return poCount;
        }

        private string getPONumber(string stCode,int? seriesNumber, bool? isTransfer)
        {
            var cdFormat = "-{0}";

            if(isTransfer == true)
            {
                cdFormat = "-TR{0}";
            }

           return string.Format(stCode + cdFormat, seriesNumber.ToString().PadLeft(6, '0'));
        }


    }
}
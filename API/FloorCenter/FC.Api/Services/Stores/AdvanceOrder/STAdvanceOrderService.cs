using FC.Api.DTOs.Store;
using FC.Api.DTOs.Store.AdvanceOrder;
using FC.Api.DTOs.Store.AdvanceOrders;
using FC.Api.DTOs.Warehouse.AllocateAdvanceOrder;
using FC.Api.Helpers;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Services.Stores.AdvanceOrder
{
    public class STAdvanceOrderService : ISTAdvanceOrderService
    {


        private DataContext _context;

        public STAdvanceOrderService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return this._context;
        }


        public void ApproveAdvanceOrder(STAdvanceOrderDTO order)
        {
            var advOrder = _context.STAdvanceOrder
                                    .Include(p => p.AdvanceOrderDetails)
                                        .Where(p => p.Id == order.Id).SingleOrDefault();

            advOrder.RequestStatus = order.RequestStatus;
            advOrder.DateUpdated = DateTime.Now;
            advOrder.ApproveDate = DateTime.Now;
 
            if(order.OrderStatus != null)
            {
                
                advOrder.OrderStatus = order.OrderStatus;
            }
            else
            {
                advOrder.OrderStatus = (order.RequestStatus != RequestStatusEnum.Cancelled) ? OrderStatusEnum.Incomplete : OrderStatusEnum.Cancelled;
            }

            if (!String.IsNullOrEmpty(order.ChangeStatusReason))
            {       
                advOrder.changeStatusReasons = order.ChangeStatusReason;
            }

            _context.STAdvanceOrder.Update(advOrder);
            _context.SaveChanges();
            if (advOrder.RequestStatus == RequestStatusEnum.Approved && order.OrderStatus == null)
            {
                foreach (var item in order.AdvanceOrderDetails)
                {
                    var orderedItem = (item.isCustom == true) 
                        ?
                        advOrder.AdvanceOrderDetails.FirstOrDefault(x => x.Id == item.Id
                                                                                     && x.STAdvanceOrderId == item.STAdvanceOrderId
                                                                                     && x.Code == item.Code && x.sizeId == item.sizeId)
                        :
                        advOrder.AdvanceOrderDetails.FirstOrDefault(x => x.Id == item.Id
                                                                                     && x.STAdvanceOrderId == item.STAdvanceOrderId
                                                                                     && x.ItemId == item.ItemId);

                    if (orderedItem == null)
                    {
                        continue;
                    }

                    orderedItem.ApprovedQuantity = item.ApprovedQuantity;
                    orderedItem.DateUpdated = DateTime.Now;

                    _context.STAdvanceOrderDetails.Update(orderedItem);
                    _context.SaveChanges();
                }

            }


        }

        public void AllocateAdvanceOrder(int? id, WHAllocateAdvanceOrder order, AppSettings appSettings)
        {
            //Get advance order details
            var advOrder = _context.STAdvanceOrder
                                        .Where(p => p.Id == order.StAdvanceOrderId && p.RequestStatus == RequestStatusEnum.Approved)
                                            .Include(p => p.AdvanceOrderDetails)
                                                .ThenInclude(p => p.Item)
                                            .FirstOrDefault();

            var totalRecordCount = Convert.ToInt32(this._context.WHAllocateAdvanceOrder.Count() + 1).ToString();
            order.AllocationNumber = string.Format("AL{0}", totalRecordCount.PadLeft(6, '0'));
            order.DateCreated = DateTime.Now;
            order.AllocationDate = DateTime.Now;
            order.StoreId = advOrder.StoreId;

            var aoDetail = new List<WHAllocateAdvanceOrderDetail>();
            foreach(var item in order.AllocateAdvanceOrderDetails)
            {
                if(item.ItemId.HasValue)
                {
                    item.DateCreated = DateTime.Now;
                    aoDetail.Add(item);
                }
            }

            order.AllocateAdvanceOrderDetails = aoDetail;

            _context.WHAllocateAdvanceOrder.Add(order);
            _context.SaveChanges();


            var whStockService = new WHStockService(_context);

            //Create client Order
            if (advOrder != null)
            {
                STOrder newOrder = new STOrder();

                newOrder.OrderType = OrderTypeEnum.ClientOrder;
                newOrder.DeliveryType = DeliveryTypeEnum.Delivery;
                newOrder.StoreId = advOrder.StoreId;
                newOrder.WarehouseId = advOrder.WarehouseId;
                newOrder.SalesAgent = advOrder.SalesAgent;
                newOrder.ClientName = advOrder.ClientName;
                newOrder.Address1 = advOrder.Address1;
                newOrder.Address2 = advOrder.Address2;
                newOrder.Address3 = advOrder.Address3;
                newOrder.ContactNumber = advOrder.ContactNumber;
                newOrder.Remarks = advOrder.Remarks;
                newOrder.isAdvanceOrderFlg = true;
                newOrder.STAdvanceOrderId = advOrder.Id;
                newOrder.PODate = advOrder.DateCreated;
                newOrder.OrderedItems = new List<STOrderDetail>();

                foreach(var items in order.AllocateAdvanceOrderDetails)
                {
                    STOrderDetail details = new STOrderDetail();
                    if(items.ItemId.HasValue)
                    {
                        details.RequestedQuantity = items.AllocatedQuantity;
                        details.ItemId = items.ItemId;
                        details.RequestedRemarks = items.Remarks;

                        details.DateCreated = DateTime.Now;

                        newOrder.OrderedItems.Add(details);

                        var whstock = new WHStock
                        {
                            WarehouseId = order.WarehouseId,
                            WHAllocateAdvanceOrderDetailId = items.Id,
                            ItemId = items.ItemId,
                            OnHand = items.AllocatedQuantity,
                            Reserved = -items.AllocatedQuantity,
                            TransactionType = TransactionTypeEnum.PO,
                            DeliveryStatus = DeliveryStatusEnum.Delivered
                        };

                        whStockService.InsertStock(whstock);
                    }
                }

                var stOrderService = new STOrderService(_context);


                if(newOrder != null)
                {
                    stOrderService.InsertOrder(newOrder, appSettings);
                }

                //Checking if all the quantity has been allocated the mark it as complete
                var appQty = advOrder.AdvanceOrderDetails.Sum(p => p.ApprovedQuantity);

                var allQty = _context.WHAllocateAdvanceOrder
                                .Include(p => p.AllocateAdvanceOrderDetails)
                                   .Where(p => p.StAdvanceOrderId == order.StAdvanceOrderId)
                                      .SelectMany(p => p.AllocateAdvanceOrderDetails)
                                           .Sum(x => x.AllocatedQuantity);

                if(appQty == allQty)
                {
                    advOrder.OrderStatus = OrderStatusEnum.Completed;
                    _context.STAdvanceOrder.Update(advOrder);
                    _context.SaveChanges();
                }








            }
        }

        public object GetApprovedAdvanceOrderList(SearchApproveRequests search, AppSettings appSettings)
        {
            IQueryable<STAdvanceOrder> query = _context.STAdvanceOrder
                                                        .Include(p => p.Store)
                                                        .Include(p => p.Warehouse)
                                                        .Include(p => p.AdvanceOrderDetails)
                                                            .ThenInclude(p => p.Item)
                                                                .ThenInclude(p => p.Size);

            if (search.WarehouseId.HasValue)
            {
                query = query.Where(p => p.WarehouseId == search.WarehouseId && p.RequestStatus == RequestStatusEnum.Approved);

                //if allocate advance order page will not include cancelled orders
                if(search.isAllocatePage == true)
                {
                    query = query.Where(p => p.OrderStatus != OrderStatusEnum.Cancelled);
                }

            }

            if(search.StoreId.HasValue)
            {
                query = query.Where(p => p.StoreId == search.StoreId);

                if(search.RequestStatus != null && search.RequestStatus.Count() > 0)
                {
                    query = query.Where(p => search.RequestStatus.Contains(p.RequestStatus));
                }
            }

            if(search.Orderstatus != null && search.Orderstatus.Count() > 0)
            {
                if(!search.StoreId.HasValue)
                {
                    query = query.Where(p => search.Orderstatus.Contains(p.OrderStatus));
                }
                
            }

            if (!string.IsNullOrWhiteSpace(search.PONumber))
            {
                query = query.Where(p => p.PONumber.ToLower() == search.PONumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.AONumber))
            {
                query = query.Where(p => p.AONumber.ToLower() == search.AONumber.ToLower());
            }

            if (search.RequestDateFrom.HasValue)
            {
                query = query.Where(p => search.RequestDateFrom.Value <= p.DateCreated);
            }

            if (search.RequestDateTo.HasValue)
            {
                query = query.Where(p => search.RequestDateTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59) >= p.DateCreated);
            }

            if (!string.IsNullOrWhiteSpace(search.SiNumber))
            {
                query = query.Where(p => p.SINumber.ToLower() == search.SiNumber.ToLower());
            }

            if(!string.IsNullOrWhiteSpace(search.ClientName))
            {
                query = query.Where(p => p.ClientName.ToLower() == search.ClientName.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ItemCode))
            {
                query = query.Where(p => p.AdvanceOrderDetails.Where(x => x.Code.Contains(search.ItemCode)).Count() > 0);
            }
            if (!string.IsNullOrWhiteSpace(search.OrderedBy))
            {
                query = query.Where(p => p.Store.Name.ToLower() == search.OrderedBy.ToLower());
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


        

            var allocatedOrder = _context.WHAllocateAdvanceOrder
                                   .Include(p => p.AllocateAdvanceOrderDetails);

            var records = from x in query
                          select new
                          {
                              x.Id,
                              x.AONumber,
                              x.SINumber,
                              x.PONumber,
                              x.RequestStatus,
                              RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), x.RequestStatus)),
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
                              x.ApproveDate,
                              x.StoreId,
                              DeliveryStatusStr = x.DeliveryStatus != null ? EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), x.DeliveryStatus)) : null,
                              x.SalesAgent,
                              x.OrderStatus,
                              x.PaymentMode,
                              PaymentModeStr = x.PaymentMode != null ? EnumExtensions.SplitName(Enum.GetName(typeof(PaymentModeEnum), x.PaymentMode)) : null,
                              OrderStatusStr = x.OrderStatus != null ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), x.OrderStatus)) : null,
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
                                  RemainingForAllocationQty = p.ApprovedQuantity - allocatedOrder.Where(c => c.StAdvanceOrderId == x.Id)
                                                                            .Sum(c => c.AllocateAdvanceOrderDetails.Where(ad =>
                                                                             ((p.isCustom == true)
                                                                             ? (ad.Code == p.Code && ad.SizeId == p.sizeId)
                                                                             : (ad.ItemId == p.ItemId))
                                                                                                          ).Select(ad => ad.AllocatedQuantity).Sum()),
                                  p.Remarks,
                                  //p.ChangeStatusReason
                              })

                          };




            response.List.AddRange(records);

            return response;

        }

        public object GetAdvanceOrderById(int? id)
        {
            var advOrder = _context.STAdvanceOrder
                                    .Include(p => p.AdvanceOrderDetails)
                                        .ThenInclude(p => p.Item)
                                            .ThenInclude(p => p.Size)
                                    .Include(p => p.Store)
                                    .Include(p => p.Warehouse)
                                    .Where(p => p.Id == id).FirstOrDefault();
          var allocatedOrder =  _context.WHAllocateAdvanceOrder
                                    .Include(p => p.AllocateAdvanceOrderDetails)
                                        .Where(p => p.StAdvanceOrderId == id);


            var records = new 
                          {
                              advOrder.Id,
                              advOrder.AONumber,
                              advOrder.SINumber,
                              advOrder.PONumber,
                              advOrder.RequestStatus,
                              RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), advOrder.RequestStatus)),
                              OrderedBy = advOrder.Store.Name,
                              OrderedTo = advOrder.Warehouse.Name,
                              advOrder.ClientName,
                              advOrder.ContactNumber,
                              advOrder.Address1,
                              advOrder.Address2,
                              advOrder.Address3,
                              advOrder.Remarks,
                              advOrder.DateCreated,
                              advOrder.DeliveryStatus,
                              advOrder.StoreId,
                              advOrder.WarehouseId,
                              DeliveryStatusStr = advOrder.DeliveryStatus != null ? EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), advOrder.DeliveryStatus)) : null,
                              advOrder.SalesAgent,
                              advOrder.PaymentMode,
                              PaymentModeStr = advOrder.PaymentMode != null ? EnumExtensions.SplitName(Enum.GetName(typeof(PaymentModeEnum), advOrder.PaymentMode)) : null,
                advOrder.OrderStatus,
                              OrderStatusStr = advOrder.OrderStatus != null ? EnumExtensions.SplitName(Enum.GetName(typeof(OrderStatusEnum), advOrder.OrderStatus)) : null,
                              AdvanceOrderDetails = advOrder.AdvanceOrderDetails.Select(p => new
                              {
                                  p.Id,
                                  p.STAdvanceOrderId,
                                  ItemId = (p.isCustom == true) ? 0 : p.ItemId,
                                  SerialNumber = (p.isCustom == true) ? "" : p.Item.SerialNumber,
                                  ItemCode = (p.isCustom == true) ? p.Code : p.Item.Code,
                                  ItemName = (p.isCustom == true) ? "" : p.Item.Name,
                                  SizeName = (p.isCustom == true) ? _context.Sizes.Where(c => c.Id == p.sizeId).Select(c => c.Name).FirstOrDefault() : p.Item.Size.Name,
                                  Tonality = (p.isCustom == true) ? p.tonality : p.Item.Tonality,
                                  //deduct the allocated quantity for display
                                  p.Quantity,
                                  p.isCustom,
                                  sizeId = (p.isCustom == true) ? p.sizeId : p.Item.SizeId,
                                  p.ApprovedQuantity,
                                  ForAllocationQty = p.ApprovedQuantity - allocatedOrder.Sum(c => c.AllocateAdvanceOrderDetails.Where(x =>
                                                                              ((p.isCustom == true)
                                                                              ? (x.Code == p.Code && x.SizeId == p.sizeId)
                                                                              : (x.ItemId == p.ItemId))
                                                                                                           ).Select(x => x.AllocatedQuantity).Sum()),
                                  AllocatedQty = allocatedOrder.Sum(c => c.AllocateAdvanceOrderDetails.Where(x =>
                                                                              ((p.isCustom == true)
                                                                              ? (x.Code == p.Code && x.SizeId == p.sizeId)
                                                                              : (x.ItemId == p.ItemId))
                                                                                                           ).Select(x => x.AllocatedQuantity).Sum()),
                                  p.Remarks,
                                  
                              }).OrderBy(p => p.Id)

                          };

            return records;


        }

        public string UpdateDelivery(ModifyAdvanceOrderDTO details, AppSettings appSettings,Boolean isDealer)
        {
            var orderService = new STOrderService(_context);
            var order = _context.STOrders.Where(p => p.Id == details.Id && p.RequestStatus == RequestStatusEnum.Pending).FirstOrDefault();
            var payment = _context.STAdvanceOrder.Where(p => p.Id == order.STAdvanceOrderId).FirstOrDefault();

            if (order != null)
            {
                if(String.IsNullOrEmpty(order.PONumber))
                {
                    var poNumber = orderService.GeneratePoNumber(order, appSettings, true);
                    order.PONumber = poNumber;
                }

                if (isDealer == true)
                {
                    order.PONumber = details.PONumber;
                    order.isDealerOrder = isDealer;
                }
              
                order.PaymentMode = details.PaymentMode;
                payment.PaymentMode = details.PaymentMode;
                order.StoreId = details.StoreId;
                order.PODate = details.PODate;
                order.DeliveryType = details.DeliveryType;
                _context.STOrders.Update(order);
                _context.SaveChanges();
            }

            return order.PONumber;
        }


        public int? GetAdvanceOrderForAllocationQuantity(STAdvanceOrderDetails advanceOrderDetails,int? itemId, int? stAdvanceOrderId)
        {
            if(advanceOrderDetails != null && stAdvanceOrderId != null)
            {
                var allocateDetails = _context.WHAllocateAdvanceOrder
                                   .Include(p => p.AllocateAdvanceOrderDetails)
                                   .Where(p => p.StAdvanceOrderId == stAdvanceOrderId);

                var allocatedQty = new int?();

                if(advanceOrderDetails != null && advanceOrderDetails.isCustom == true)
                {
                     allocatedQty = allocateDetails.Sum(c => c.AllocateAdvanceOrderDetails
                                                .Where(ad => (ad.Code == advanceOrderDetails.Code 
                                                              && ad.SizeId == advanceOrderDetails.sizeId))
                                                                    .Select(ad => ad.AllocatedQuantity).Sum());
                }
                else
                {
                    allocatedQty = allocateDetails.Sum(c => c.AllocateAdvanceOrderDetails
                                                .Where(ad => (ad.ItemId == advanceOrderDetails.ItemId))
                                                                    .Select(ad => ad.AllocatedQuantity).Sum());
                  
                }

                return advanceOrderDetails.ApprovedQuantity - allocatedQty;

            }

            return 0;
      
        }


        public int? GetAdvanceOrderAllocatedQuantity(STAdvanceOrderDetails advanceOrderDetails, int? itemId, int? stAdvanceOrderId)
        {
            if (advanceOrderDetails != null && stAdvanceOrderId != null)
            {
                var allocateDetails = _context.WHAllocateAdvanceOrder
                                   .Include(p => p.AllocateAdvanceOrderDetails)
                                   .Where(p => p.StAdvanceOrderId == stAdvanceOrderId);

                var allocatedQty = new int?();

                if (advanceOrderDetails.isCustom == true)
                {
                    allocatedQty = allocateDetails.Sum(c => c.AllocateAdvanceOrderDetails
                                               .Where(ad => (ad.Code == advanceOrderDetails.Code
                                                             && ad.SizeId == advanceOrderDetails.sizeId))
                                                                   .Select(ad => ad.AllocatedQuantity).Sum());
                }
                else
                {
                    allocatedQty = allocateDetails.Sum(c => c.AllocateAdvanceOrderDetails
                                                .Where(ad => (ad.ItemId == advanceOrderDetails.ItemId))
                                                                    .Select(ad => ad.AllocatedQuantity).Sum());

                }

                return allocatedQty;

            }

            return 0;

        }




    }
}

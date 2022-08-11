using FC.Api.DTOs.Store.Returns.ClientReturn;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Services.Stores.Returns
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

        public IEnumerable<object> GetAll(SearchClientReturn search)
        {
            IQueryable<STSales> query = _context.STSales
                                                .Include(p => p.SoldItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                .Where
                                                (p =>
                                                    p.StoreId == search.StoreId &&
                                                    (p.SalesType == SalesTypeEnum.Releasing ?  p.ORNumber != null : p.ReleaseDate != null)

                                                );


            if (!string.IsNullOrWhiteSpace(search.ClientName))
            {
                query = query.Where(p => p.ClientName.ToLower() == search.ClientName.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.SINumber))
            {
                query = query.Where(p => p.SINumber.ToLower() == search.SINumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ORNumber))
            {
                query = query.Where(p => p.ORNumber.ToLower() == search.ORNumber.ToLower());
            }

            query = query.OrderByDescending(p => p.Id);

            var retList = new List<object>();
            foreach (var x in query)
            {

                var soldItems = x.SoldItems.ToList();
                if (soldItems != null && soldItems.Count > 0)
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
                        x.DateUpdated,
                        DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                        SoldItems = soldItems.Select(p => new
                        {
                            p.Id,
                            p.STSalesId,
                            p.ItemId,
                            p.Item.Code,
                            ItemName = p.Item.Name,
                            SizeName = p.Item.Size.Name,
                            p.Item.Tonality,
                            Quantity = p.Quantity - Convert.ToInt32(
                                            _context.STClientReturns
                                                    .Where(q => q.STSalesDetailId == p.Id
                                                                && (
                                                                    q.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                                    || q.DeliveryStatus == DeliveryStatusEnum.Pending
                                                                ))
                                                    .Sum(q => q.Quantity)
                                         )
                        })
                    };

                    if (obj.SoldItems.Sum(p => p.Quantity) != 0)
                    {
                        if(obj.DeliveryType == DeliveryTypeEnum.Delivery && obj.SalesType == SalesTypeEnum.ClientOrder && obj.DateUpdated == null)
                        {
                            continue;
                        }
                        retList.Add(obj);
                    }

                    
                }
            }

            return retList;

        }


        public object GetAllPaged(SearchClientReturn search,AppSettings appSettings)
        {
            IQueryable<STSales> query = _context.STSales
                                                .Include(p => p.SoldItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                .Where
                                                (p =>
                                                    (p.StoreId == search.StoreId &&
                                                    // replace ornumber to client name for searching in client return
                                                    (p.SalesType == SalesTypeEnum.Releasing ? p.ClientName != null : p.ReleaseDate != null))

                                                );


            if (!string.IsNullOrWhiteSpace(search.ClientName))
            {
                query = query.Where(p => p.ClientName.ToLower() == search.ClientName.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.SINumber))
            {
                query = query.Where(p => p.SINumber.ToLower() == search.SINumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ORNumber))
            {
                query = query.Where(p => p.ORNumber.ToLower() == search.ORNumber.ToLower());
            }

            query = query.OrderByDescending(p => p.Id);

            var retList = new List<object>();
            foreach (var x in query)
            {

                var soldItems = x.SoldItems.ToList();
                if (soldItems != null && soldItems.Count > 0)
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
                        x.DateUpdated,
                        DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                        SoldItems = soldItems.Select(p => new
                        {
                            p.Id,
                            p.STSalesId,
                            p.ItemId,
                            p.Item.Code,
                            ItemName = p.Item.Name,
                            SizeName = p.Item.Size.Name,
                            p.Item.Tonality,
                            Quantity = p.Quantity - Convert.ToInt32(
                                            _context.STClientReturns
                                                    .Where(q => q.STSalesDetailId == p.Id
                                                                && (
                                                                    q.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                                    || q.DeliveryStatus == DeliveryStatusEnum.Pending
                                                                ))
                                                    .Sum(q => q.Quantity)
                                         )
                        })
                    };

                    if (obj.SoldItems.Sum(p => p.Quantity) != 0)
                    {
                        if (((obj.DeliveryType == DeliveryTypeEnum.Delivery 
                            && (obj.SalesType == SalesTypeEnum.ClientOrder 
                            || obj.SalesType == SalesTypeEnum.SalesOrder)) 
                            && obj.DateUpdated == null)
                            ||
                            ((obj.SalesType == SalesTypeEnum.Intercompany || obj.SalesType == SalesTypeEnum.Interbranch) && obj.DeliveryType == DeliveryTypeEnum.ShowroomPickup && string.IsNullOrEmpty(obj.SINumber))
                            )
                        {
                            continue;
                        }
                        retList.Add(obj);
                    }


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

        public object GetByIdAndStoreId(int id, int? storeId)
        {
            var record = _context.STSales
                                 .Include(p => p.SoldItems)
                                        .ThenInclude(p => p.Item)
                                            .ThenInclude(p => p.Size)
                                 .Where(p => p.StoreId == storeId
                                             && p.Id == id)
                                 .FirstOrDefault();

            if(record != null)
            {
                var soldItems = record.SoldItems.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered).ToList();
                if (soldItems != null && soldItems.Count > 0)
                {
                    return new
                    {

                        record.Id,
                        record.TransactionNo,
                        record.SINumber,
                        record.ORNumber,
                        record.DRNumber,
                        record.ReleaseDate,
                        record.ClientName,
                        record.Address1,
                        record.Address2,
                        record.Address3,
                        record.SalesType,
                        record.SalesDate,
                        SalesTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(SalesTypeEnum), record.SalesType)),
                        record.SalesAgent,
                        record.Remarks,
                        record.ContactNumber,
                        record.DeliveryType,
                        DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), record.DeliveryType)),
                        SoldItems = soldItems.Select(p => new
                        {
                            p.Id,
                            p.STSalesId,
                            p.ItemId,
                            p.Item.Code,
                            p.Item.SerialNumber,
                            ItemName = p.Item.Name,
                            SizeName = p.Item.Size.Name,
                            p.Item.Tonality,

                            Quantity = p.Quantity - Convert.ToInt32(
                                            _context.STClientReturns
                                                    .Where(q => q.STSalesDetailId == p.Id
                                                                && (
                                                                    q.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                                    || q.DeliveryStatus == DeliveryStatusEnum.Pending
                                                                ))
                                                    .Sum(q => q.Quantity)
                                         )
                        }).OrderBy(p => p.Id)
                    };
                }
            }

            return null;
        }

        public string AddClientReturn(AddClientReturnDTO param)
        {
            var objSTReturn = new STReturn
            {
                STSalesId = param.Id,
                StoreId = param.StoreId,
                ReturnType = ReturnTypeEnum.ClientReturn,
                ClientReturnType = param.ClientReturnType,
                //Added for optimization
                OrderStatus = OrderStatusEnum.Incomplete,
                ClientPurchasedItems = new List<STClientReturn>()
            };

            if(param.ClientReturnType == ClientReturnTypeEnum.RequestPickup)
            {
                objSTReturn.PickupDate = param.PickupDate;
                objSTReturn.ReturnDRNumber = param.ReturnDRNumber;
                objSTReturn.RequestStatus = RequestStatusEnum.Approved;
                objSTReturn.Remarks = param.Remarks;
            }

            var totalRecordCount = Convert.ToInt32
                                   (
                                        this._context.STReturns
                                                     .Where(x => x.ReturnType == objSTReturn.ReturnType)
                                                     .Count() + 1
                                    ).ToString();

            objSTReturn.TransactionNo = string.Format("RT{0}", totalRecordCount.PadLeft(6, '0'));

            objSTReturn.DateCreated = DateTime.Now;
            if (param.ClientReturnType != ClientReturnTypeEnum.RequestPickup)
            {
                objSTReturn.RequestStatus = RequestStatusEnum.Pending;
                objSTReturn.ReturnDRNumber = param.ReturnDRNumber;
            }
       

            foreach (var retItem in param.PurchasedItems)
            {
                var objSTClientReturn = new STClientReturn
                {
                    STSalesDetailId = retItem.Id,
                    ItemId = retItem.ItemId,
                    Quantity = retItem.Quantity,
                    ReturnReason = retItem.ReturnReason,
                    Remarks = retItem.Remarks,
                    DateCreated = DateTime.Now,
                    DeliveryStatus = DeliveryStatusEnum.Pending
                };

                objSTReturn.ClientPurchasedItems.Add(objSTClientReturn);
            }

            _context.STReturns.Add(objSTReturn);
            _context.SaveChanges();

            return objSTReturn.TransactionNo;
        }

    }
}

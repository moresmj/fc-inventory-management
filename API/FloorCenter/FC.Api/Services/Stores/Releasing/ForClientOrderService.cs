using System;
using System.Collections.Generic;
using System.Linq;
using FC.Api.DTOs.Store.Releasing;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;

namespace FC.Api.Services.Stores.Releasing
{
    public class ForClientOrderService : IForClientOrderService
    {

        private DataContext _context;

        public ForClientOrderService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return this._context;
        }

        public IEnumerable<object> GetAll(SearchForClientOrder search)
        {
            IQueryable<STSales> query = _context.STSales
                                                .Include(p => p.Order)
                                                .Include(p => p.SoldItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                .Where
                                                (p =>
                                                    p.StoreId == search.StoreId
                                                    && (
                                                        p.SalesType == SalesTypeEnum.ClientOrder
                                                        || p.SalesType == SalesTypeEnum.Interbranch
                                                        || p.SalesType == SalesTypeEnum.Intercompany
                                                    )
                                                    && p.DeliveryType == DeliveryTypeEnum.ShowroomPickup 
                                                ).OrderByDescending(p => p.Id);


            if (!string.IsNullOrWhiteSpace(search.SINumber))
            {
                query = query.Where(p => p.SINumber.ToLower() == search.SINumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ORNumber))
            {
                query = query.Where(p => p.ORNumber.ToLower() == search.ORNumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ClientName))
            {
                query = query.Where(p => p.ClientName.ToLower() == search.ClientName.ToLower());
            }

            if (!string.IsNullOrEmpty(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }


            var retList = new List<object>();
            foreach (var x in query)
            {
                var toBeAdded = false;

                if (x.STOrderId.HasValue)
                {
                    toBeAdded = (string.IsNullOrEmpty(search.PONumber)) ? true :
                                        (x.Order.PONumber.ToLower() == search.PONumber.ToLower());
                }

                var obj = new
                {
                    x.Id,
                    PONumber = (x.STOrderId.HasValue) ? x.Order.PONumber : null,
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
                    DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                    SoldItems = x.SoldItems.Select(p => new
                    {
                        p.Item.Code,
                        ItemName = p.Item.Name,
                        SizeName = p.Item.Size.Name,
                        p.Item.Tonality,
                        p.Quantity,
                        p.Id
                    }).OrderBy(p => p.Id),
                    PaymentMode = x.Order.PaymentMode != null ? EnumExtensions.SplitName(Enum.GetName(typeof(PaymentModeEnum),x.Order?.PaymentMode)) : ""
                };

                if (toBeAdded)
                {
                    retList.Add(obj);
                }
            }
            
            return retList;
            
        }


        public object GetAllPaged(SearchForClientOrder search, AppSettings appSettings)
        {
            IQueryable<STSales> query = _context.STSales
                                                .Include(p => p.Order)
                                                .Include(p => p.SoldItems)
                                                    .ThenInclude(p => p.Item)
                                                        .ThenInclude(p => p.Size)
                                                .Where
                                                (p =>
                                                    p.StoreId == search.StoreId
                                                    && (
                                                        p.SalesType == SalesTypeEnum.ClientOrder
                                                        || p.SalesType == SalesTypeEnum.Interbranch
                                                        || p.SalesType == SalesTypeEnum.Intercompany
                                                    )
                                                    && p.DeliveryType == DeliveryTypeEnum.ShowroomPickup
                                                    && p.STOrderId.HasValue
                                                ).OrderByDescending(p => p.Id);


            if (!string.IsNullOrWhiteSpace(search.SINumber))
            {
                query = query.Where(p => p.SINumber.ToLower() == search.SINumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ORNumber))
            {
                query = query.Where(p => p.ORNumber.ToLower() == search.ORNumber.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(search.ClientName))
            {
                query = query.Where(p => p.ClientName.ToLower() == search.ClientName.ToLower());
            }

            if (!string.IsNullOrEmpty(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }

            if(!string.IsNullOrEmpty(search.PONumber))
            {
                query.Where(p => p.Order.PONumber.ToLower() == search.PONumber.ToLower());
            }

            if (search.ReleaseStatus != null && search.ReleaseStatus.Count() == 1)
            {
                if (search.ReleaseStatus[0] == ReleaseStatusEnum.Waiting)
                {
                    query = query.Where(p => string.IsNullOrEmpty(p.SINumber));

                }
                else if (search.ReleaseStatus[0] == ReleaseStatusEnum.Released)
                {
                    query = query.Where(p => !string.IsNullOrEmpty(p.SINumber));
                }
            }

            GetAllResponse response = null;

            if (search.ShowAll == false)
            {

                response = new GetAllResponse(query.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);

                if(search.CurrentPage > response.TotalPage)
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

            //Format items to be returned
            foreach (var x in query)
            {
                var obj = new
                {
                    x.Id,
                    PONumber = (x.STOrderId.HasValue) ? x.Order.PONumber : null,
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
                    DeliveryTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryTypeEnum), x.DeliveryType)),
                    SoldItems = x.SoldItems.Select(p => new
                    {
                        p.Item.Code,
                        ItemName = p.Item.Name,
                        SizeName = p.Item.Size.Name,
                        p.Item.Tonality,
                        p.Quantity,
                        p.Id,
                    }).OrderBy(p => p.Id),
                    PaymentMode = x.Order.PaymentMode != null ? EnumExtensions.SplitName(Enum.GetName(typeof(PaymentModeEnum), x.Order?.PaymentMode)) : ""
                };

                retList.Add(obj);
            }

            response.List.AddRange(retList);

            return response;

        }

        public STSales GetByIdAndStoreId(int? id, int? storeId)
        {
            var record = _context.STSales
                                 .Where
                                 (p => p.StoreId == storeId
                                       && p.Id == id
                                       && (
                                            p.SalesType == SalesTypeEnum.ClientOrder
                                            || p.SalesType == SalesTypeEnum.Interbranch
                                            || p.SalesType == SalesTypeEnum.Intercompany
                                       )
                                       && p.DeliveryType == DeliveryTypeEnum.ShowroomPickup
                                 )
                                .Include(p => p.Order)
                                .Include(p => p.SoldItems)
                                    .ThenInclude(p => p.Item)
                                        .ThenInclude(p => p.Size)
                             .FirstOrDefault();

            return record;
        }

        public void Update(ISTStockService stockService, STSales param)
        {
            var obj = this.GetByIdAndStoreId(param.Id, param.StoreId);
            if (obj != null)
            {
                //  For the fields to be updated
                //  refer ReleaseForClientOrderDTO.cs
                obj.SINumber = param.SINumber;
                obj.ORNumber = param.ORNumber;
                obj.DRNumber = param.DRNumber;
                obj.Remarks = param.Remarks;
                obj.SalesAgent = param.SalesAgent;

                obj.DateUpdated = DateTime.Now;

                //  Set release date to date today
                obj.ReleaseDate = DateTime.Now;

                foreach (var soldItem in obj.SoldItems)
                {
                    soldItem.DeliveryStatus = DeliveryStatusEnum.Delivered;
                    soldItem.DateUpdated = DateTime.Now;

                    var stStock = _context.STStocks
                                          .Where
                                          (p => p.StoreId == obj.StoreId
                                                && p.STSalesDetailId == soldItem.Id
                                                && p.ItemId == soldItem.ItemId
                                          )
                                          .FirstOrDefault();

                    if (stStock != null)
                    {
                        stStock.ReleaseStatus = ReleaseStatusEnum.Released;
                        stStock.DeliveryStatus = DeliveryStatusEnum.Delivered;

                        stockService.UpdateSTStock(stStock);
                    }

                }

                _context.STSales.Update(obj);
                _context.SaveChanges();

            }
        }
    }
}

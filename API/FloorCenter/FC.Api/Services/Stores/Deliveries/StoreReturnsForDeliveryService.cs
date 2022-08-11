using FC.Api.DTOs.Store.Deliveries;
using FC.Api.Helpers;
using FC.Api.Services.Warehouses.Returns;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Services.Stores.Deliveries
{
    public class StoreReturnsForDeliveryService : IStoreReturnsForDeliveryService
    {

        private DataContext _context;

        public StoreReturnsForDeliveryService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<object> GetAll(SearchReturnsForDeliveries search)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.Store)
                                                 .Include(p => p.ClientPurchasedItems)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                 .Where
                                                 (p =>
                                                    p.ReturnType == ReturnTypeEnum.ClientReturn
                                                    && p.ClientReturnType == ClientReturnTypeEnum.RequestPickup
                                                 ).OrderByDescending(p => p.Id);

            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                query = query.Where(p => p.ReturnDRNumber.ToLower() == search.DRNumber.ToLower());
            }
            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }
            //  Check if DeliveryDateFrom search criteria has value
            if (search.DeliveryDateFrom.HasValue)
            {
                //  Searched by DeliveryDateFrom <= DeliveryDate
                query = query.Where(y => search.DeliveryDateFrom <= y.PickupDate);
            }

            //  Check if DeliveryDateTo search criteria has value
            if (search.DeliveryDateTo.HasValue)
            {
                //  Searched by DeliveryDateTo >= DeliveryDate
                query = query.Where(y => search.DeliveryDateTo >= y.PickupDate);
            }

            var records = new List<object>();
            foreach (var q in query)
            {
                if(q.ClientPurchasedItems != null && q.ClientPurchasedItems.Count > 0)
                {
                    var deliveryStatus = (q.ClientPurchasedItems.Where(p => p.STReturnId == q.Id).Count() > 0)
                                                    ? q.ClientPurchasedItems.Where(p => p.STReturnId == q.Id).Select(p => p.DeliveryStatus).FirstOrDefault() : null;

                    var salesDetail = _context.STSales.AsNoTracking().Where(p => p.Id == q.STSalesId).FirstOrDefault();

                    var obj = new
                        {
                            q.Id,
                            Address = $"{salesDetail.Address1} {salesDetail.Address2} {salesDetail.Address3}",
                            DRNumber = q.ReturnType == ReturnTypeEnum.ClientReturn ? "" : q.ReturnDRNumber,
                            q.PickupDate,
                            q.TransactionNo,
                            returnTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), q.ReturnType)),
                            requestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), q.RequestStatus)),
                            q.DateCreated,
                            q.Remarks,
                            q.ReturnFormNumber,
                            ReturnedBy = salesDetail.ClientName,
                            DeliverTo = q.Store.Name,
                            DeliverToAddress = q.Store.Address,
                            DeliverToContact = q.Store.ContactNumber,
                            DeliveryQty = q.ClientPurchasedItems.Sum(p => p.Quantity),
                            Deliveries = q.ClientPurchasedItems.Select(p => new
                            {
                                p.Item.SerialNumber,
                                p.Item.Code,
                                ItemName = p.Item.Name,
                                SizeName = p.Item.Size.Name,
                                p.Item.Tonality,
                                p.Quantity

                            }),
                            DeliveryStatus = (deliveryStatus.HasValue) ? deliveryStatus : null,
                            DeliveryStatusStr = (deliveryStatus.HasValue) ? this.GetDeliveryStatusStr(deliveryStatus) : null,
                            q.ApprovedDeliveryDate,
                            q.DriverName,
                            q.PlateNumber,
                        };

                    // Delivery Status Filtering
                    if (search.DeliveryStatus != null)
                    {
                        if (deliveryStatus.HasValue && search.DeliveryStatus.Contains(deliveryStatus))
                        {
                            records.Add(obj);
                        }
                    }
                    else
                    {
                        records.Add(obj);
                    }                
                }
            }



            return records;
        }


        public object GetAllPaged(SearchReturnsForDeliveries search, AppSettings appSettings)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.Store)
                                                 .Include(p => p.ClientPurchasedItems)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                 .Where
                                                 (p =>
                                                    p.ReturnType == ReturnTypeEnum.ClientReturn
                                                    && p.ClientReturnType == ClientReturnTypeEnum.RequestPickup
                                                 ).OrderByDescending(p => p.Id);

            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                query = query.Where(p => p.ReturnDRNumber.ToLower() == search.DRNumber.ToLower());
            }
            if (!string.IsNullOrWhiteSpace(search.TransactionNo))
            {
                query = query.Where(p => p.TransactionNo.ToLower() == search.TransactionNo.ToLower());
            }
            //  Check if DeliveryDateFrom search criteria has value
            if (search.DeliveryDateFrom.HasValue)
            {
                //  Searched by DeliveryDateFrom <= DeliveryDate
                query = query.Where(y => search.DeliveryDateFrom <= y.PickupDate);
            }

            //  Check if DeliveryDateTo search criteria has value
            if (search.DeliveryDateTo.HasValue)
            {
                //  Searched by DeliveryDateTo >= DeliveryDate
                query = query.Where(y => search.DeliveryDateTo >= y.PickupDate);
            }

            if(search.DeliveryStatus != null)
            {
                var test = query.Where(p => p.ClientPurchasedItems.Any(x => search.DeliveryStatus.Contains(x.DeliveryStatus))).ToList();
            }

            var records = new List<object>();
            foreach (var q in query)
            {
                if (q.ClientPurchasedItems != null && q.ClientPurchasedItems.Count > 0)
                {
                    var deliveryStatus = (q.ClientPurchasedItems.Where(p => p.STReturnId == q.Id).Count() > 0)
                                                    ? q.ClientPurchasedItems.Where(p => p.STReturnId == q.Id).Select(p => p.DeliveryStatus).FirstOrDefault() : null;

                    var salesDetail = _context.STSales.AsNoTracking().Where(p => p.Id == q.STSalesId).FirstOrDefault();

                    var obj = new
                    {
                        q.Id,
                        Address = $"{salesDetail.Address1} {salesDetail.Address2} {salesDetail.Address3}",
                        DRNumber = q.ReturnType == ReturnTypeEnum.ClientReturn ? "" : q.ReturnDRNumber,
                        q.PickupDate,
                        q.TransactionNo,
                        returnTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), q.ReturnType)),
                        requestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), q.RequestStatus)),
                        q.DateCreated,
                        q.Remarks,
                        q.ReturnFormNumber,
                        ReturnedBy = salesDetail.ClientName,
                        DeliverTo = q.Store.Name,
                        DeliverToAddress = q.Store.Address,
                        DeliverToContact = q.Store.ContactNumber,
                        DeliveryQty = q.ClientPurchasedItems.Sum(p => p.Quantity),
                        Deliveries = q.ClientPurchasedItems.Select(p => new
                        {
                            p.Item.SerialNumber,
                            p.Item.Code,
                            ItemName = p.Item.Name,
                            SizeName = p.Item.Size.Name,
                            p.Item.Tonality,
                            p.Quantity,
                            p.isTonalityAny,

                        }),
                        DeliveryStatus = (deliveryStatus.HasValue) ? deliveryStatus : null,
                        DeliveryStatusStr = (deliveryStatus.HasValue) ? this.GetDeliveryStatusStr(deliveryStatus) : null,
                        q.ApprovedDeliveryDate,
                        q.DriverName,
                        q.PlateNumber,
                    };

                    // Delivery Status Filtering
                    if (search.DeliveryStatus != null)
                    {
                        if (deliveryStatus.HasValue && search.DeliveryStatus.Contains(deliveryStatus))
                        {
                            records.Add(obj);
                        }
                    }
                    else
                    {
                        records.Add(obj);
                    }
                }
            }

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

        public void UpdateStoreReturnsDelivery(UpdateStoreReturnsDeliveryDTO param)
        {
            var delivery = _context.STReturns
                                   .Include(p => p.ClientPurchasedItems)
                                   .Where(p => p.Id == param.Id)
                                   .FirstOrDefault();


            delivery.ApprovedDeliveryDate = param.ApprovedDeliveryDate;
            delivery.DriverName = param.DriverName;
            delivery.PlateNumber = param.PlateNumber;
            delivery.DateUpdated = DateTime.Now;

            foreach (var delItem in delivery.ClientPurchasedItems)
            {
                //  Change DeliveryStatus from Pending to Waiting
                delItem.DeliveryStatus = DeliveryStatusEnum.Waiting;
                delItem.DateUpdated = DateTime.Now;
            }

            _context.STReturns.Update(delivery);
            _context.SaveChanges();
        }

        private string GetDeliveryStatusStr(DeliveryStatusEnum? deliveryStatus)
        {
            return EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), deliveryStatus));
        }
    }
}

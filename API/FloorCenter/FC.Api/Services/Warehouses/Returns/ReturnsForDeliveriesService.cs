using FC.Api.DTOs.Warehouse.Delivery;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FC.Core.Domain.Warehouses;
using FC.Core.Helper.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Services.Warehouses.Returns
{
    public class ReturnsForDeliveriesService : IReturnsForDeliveriesService
    {
        
        private DataContext _context;

        public ReturnsForDeliveriesService(DataContext context)
        {
            _context = context;
        }

        public DataContext DataContext()
        {
            return _context;
        }

        public IEnumerable<object> GetAll(SearchReturnsForDeliveries search)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.Deliveries)
                                                    .ThenInclude(p => p.WarehouseDeliveries)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                                .Include(p => p.Store)
                                                                    .Include(p => p.Warehouse)
                                                 .Where
                                                 (p =>
                                                    (p.RequestStatus == RequestStatusEnum.Approved) || (p.RequestStatus == RequestStatusEnum.Pending && p.ReturnType == ReturnTypeEnum.Breakage)
                                                 );

            var records = new List<object>();
            foreach (var q in query.OrderByDescending(p => p.Id))
            {

                if (!string.IsNullOrWhiteSpace(search.DRNumber))
                {
                    q.Deliveries = q.Deliveries.Where(p => p.DRNumber.ToLower() == search.DRNumber.ToLower()).ToList();
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

                    foreach (var delivery in q.Deliveries.OrderByDescending(p => p.Id))
                    {
                        var deliveryStatus = (delivery.WarehouseDeliveries.Where(p => p.WHDeliveryId == delivery.Id).Count() > 0)
                                                    ? delivery.WarehouseDeliveries.Where(p => p.WHDeliveryId == delivery.Id).Select(p => p.DeliveryStatus).FirstOrDefault() : null;

                        var obj = new
                        {
                            delivery.Id,
                            delivery.DRNumber,
                            delivery.DeliveryDate,
                            q.TransactionNo,
                            q.ReturnFormNumber,
                            q.ReturnType,
                            q.Remarks,
                            DeliveryRemarks = delivery.Remarks,
                            ReturnTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), q.ReturnType)),
                            RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), q.RequestStatus)),
                            delivery.ApprovedDeliveryDate,
                            delivery.DriverName,
                            delivery.PlateNumber,
                            ReturnedBy = q.Store.Name,
                            DeliverTo = q.Warehouse.Name,
                            DeliverToAddress = q.Warehouse.Address,
                            DeliverToContact = q.Warehouse.ContactNumber,
                            delivery.DateCreated,
                            DeliverfromStore = q.Store.Name,
                            DeliveryQty = delivery.WarehouseDeliveries.Sum(p => p.Quantity),
                            Deliveries = delivery.WarehouseDeliveries.Select(p => new
                            {
                                p.Item.SerialNumber,
                                p.Item.Code,
                                ItemName = p.Item.Name,
                                SizeName = p.Item.Size.Name,
                                p.Item.Tonality,
                                p.Quantity,
                                DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus))

                            }),
                            DeliveryStatus = (deliveryStatus.HasValue) ? deliveryStatus : null,
                            DeliveryStatusStr = (deliveryStatus.HasValue) ? this.GetDeliveryStatusStr(deliveryStatus) : null

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

            }


            return records;

        }

        public object GetAllPaged(SearchReturnsForDeliveries search, AppSettings appSettings)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.Deliveries)
                                                    .ThenInclude(p => p.WarehouseDeliveries)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                                .Include(p => p.Store)
                                                                    .Include(p => p.Warehouse)
                                                 .Where
                                                 (p =>
                                                    (p.RequestStatus == RequestStatusEnum.Approved) || (p.RequestStatus == RequestStatusEnum.Pending && p.ReturnType == ReturnTypeEnum.Breakage)
                                                 );

            var records = new List<object>();
            foreach (var q in query.OrderByDescending(p => p.Id))
            {

                if (!string.IsNullOrWhiteSpace(search.DRNumber))
                {
                    q.Deliveries = q.Deliveries.Where(p => p.DRNumber.ToLower() == search.DRNumber.ToLower()).ToList();
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

                    foreach (var delivery in q.Deliveries.OrderByDescending(p => p.Id))
                    {
                        var deliveryStatus = (delivery.WarehouseDeliveries.Where(p => p.WHDeliveryId == delivery.Id).Count() > 0)
                                                    ? delivery.WarehouseDeliveries.Where(p => p.WHDeliveryId == delivery.Id).Select(p => p.DeliveryStatus).FirstOrDefault() : null;

                        var obj = new
                        {
                            delivery.Id,
                            delivery.DRNumber,
                            delivery.DeliveryDate,
                            q.TransactionNo,
                            q.ReturnFormNumber,
                            q.ReturnType,
                            q.Remarks,
                            DeliveryRemarks = delivery.Remarks,
                            ReturnTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), q.ReturnType)),
                            RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), q.RequestStatus)),
                            delivery.ApprovedDeliveryDate,
                            delivery.DriverName,
                            delivery.PlateNumber,
                            ReturnedBy = q.Store.Name,
                            DeliverTo = q.Warehouse.Name,
                            DeliverToAddress = q.Warehouse.Address,
                            DeliverToContact = q.Warehouse.ContactNumber,
                            delivery.DateCreated,
                            DeliverfromStore = q.Store.Name,
                            DeliveryQty = delivery.WarehouseDeliveries.Sum(p => p.Quantity),
                            Deliveries = delivery.WarehouseDeliveries.Select(p => new
                            {
                                p.Item.SerialNumber,
                                p.Item.Code,
                                ItemName = p.Item.Name,
                                SizeName = p.Item.Size.Name,
                                p.Item.Tonality,
                                p.Quantity,
                                DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus))

                            }),
                            DeliveryStatus = (deliveryStatus.HasValue) ? deliveryStatus : null,
                            DeliveryStatusStr = (deliveryStatus.HasValue) ? this.GetDeliveryStatusStr(deliveryStatus) : null

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



        public object GetAllPaged2(SearchReturnsForDeliveries search, AppSettings appSettings)
        {
            IQueryable<STReturn> query = _context.STReturns
                                                 .Include(p => p.Deliveries)
                                                    .ThenInclude(p => p.WarehouseDeliveries)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                                .Include(p => p.Store)
                                                                    .Include(p => p.Warehouse)
                                                 .Where
                                                 (p =>
                                                    (p.RequestStatus == RequestStatusEnum.Approved) || (p.RequestStatus == RequestStatusEnum.Pending && p.ReturnType == ReturnTypeEnum.Breakage)
                                                 );

            IQueryable<WHDelivery> whDel = query.SelectMany(p => p.Deliveries)
                                                    .Include(p => p.WarehouseDeliveries)
                                                        .ThenInclude(p => p.Item)
                                                            .ThenInclude(p => p.Size)
                                                                .Include(p => p.Store);

            var records = new List<object>();
            if (search.DeliveryStatus != null)
            {
                whDel = whDel.Where(p => p.WarehouseDeliveries.Any(w => search.DeliveryStatus.Contains(w.DeliveryStatus)));
                    //query.Where(p => p.Deliveries.Any(x => x.WarehouseDeliveries.Any(w => search.DeliveryStatus.Contains(w.DeliveryStatus))));
            }

            if (!string.IsNullOrWhiteSpace(search.DRNumber))
            {
                whDel = whDel.Where(p => p.DRNumber.ToLower() == search.DRNumber.ToLower());
            }

            //  Check if DeliveryDateFrom search criteria has value
            if (search.DeliveryDateFrom.HasValue)
            {
                //  Searched by DeliveryDateFrom <= DeliveryDate
                //q.Deliveries = q.Deliveries.Where(y => search.DeliveryDateFrom <= y.DeliveryDate).ToList();
                whDel = whDel.Where(p => search.DeliveryDateFrom <= p.DeliveryDate);
            }

            //  Check if DeliveryDateTo search criteria has value
            if (search.DeliveryDateTo.HasValue)
            {
                //  Searched by DeliveryDateTo >= DeliveryDate
                //q.Deliveries = q.Deliveries.Where(y => search.DeliveryDateTo >= y.DeliveryDate).ToList();

                whDel = whDel.Where(p => search.DeliveryDateTo >= p.DeliveryDate);
            }

            whDel = whDel.OrderByDescending(p => p.Id);


            GetAllResponse response = null;
            if (search.ShowAll == false)
            {
                response = new GetAllResponse(whDel.Count(), search.CurrentPage, appSettings.RecordDisplayPerPage);


                if (search.CurrentPage > response.TotalPage)
                {
                    var error = new ErrorResponse();
                    error.ErrorMessages.Add(MessageHelper.NoRecordFound);

                    return error;
                }

                whDel = whDel.Skip((search.CurrentPage - 1) * appSettings.RecordDisplayPerPage)
                                .Take(appSettings.RecordDisplayPerPage);
            }
            else
            {
                response = new GetAllResponse(whDel.Count());
            }


            foreach (var delivery in whDel)
            {
                var deliveryStatus = delivery.WarehouseDeliveries.Where(p => p.WHDeliveryId == delivery.Id).Select(p => p.DeliveryStatus).FirstOrDefault();

                //var deliveryStatus = (delivery.WarehouseDeliveries.Where(p => p.WHDeliveryId == delivery.Id).Count() > 0)
                //                            ? delivery.WarehouseDeliveries.Where(p => p.WHDeliveryId == delivery.Id).Select(p => p.DeliveryStatus).FirstOrDefault() : null;
                var returnDetails = query.Where(p => p.Id == delivery.STReturnId).FirstOrDefault();

                var obj = new
                {
                    delivery.Id,
                    delivery.DRNumber,
                    delivery.DeliveryDate,
                    returnDetails.TransactionNo,
                    returnDetails.ReturnFormNumber,
                    returnDetails.ReturnType,
                    returnDetails.Remarks,
                    DeliveryRemarks = delivery.Remarks,
                    ReturnTypeStr = EnumExtensions.SplitName(Enum.GetName(typeof(ReturnTypeEnum), returnDetails.ReturnType)),
                    RequestStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(RequestStatusEnum), returnDetails.RequestStatus)),
                    delivery.ApprovedDeliveryDate,
                    delivery.DriverName,
                    delivery.PlateNumber,
                    ReturnedBy = returnDetails.Store.Name,
                    DeliverTo = returnDetails.Warehouse.Name,
                    DeliverToAddress = returnDetails.Warehouse.Address,
                    DeliverToContact = returnDetails.Warehouse.ContactNumber,
                    delivery.DateCreated,
                    DeliverfromStore = returnDetails.Store.Name,
                    DeliveryQty = delivery.WarehouseDeliveries.Sum(p => p.Quantity),
                    Deliveries = delivery.WarehouseDeliveries.Select(p => new
                    {
                        p.Item.SerialNumber,
                        p.Item.Code,
                        ItemName = p.Item.Name,
                        SizeName = p.Item.Size.Name,
                        p.Item.Tonality,
                        p.Quantity,
                        DeliveryStatusStr = EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), p.DeliveryStatus)),
                        p.isTonalityAny
                    }),
                    DeliveryStatus = (deliveryStatus.HasValue) ? deliveryStatus : null,
                    DeliveryStatusStr = (deliveryStatus.HasValue) ? this.GetDeliveryStatusStr(deliveryStatus) : null

                };

                records.Add(obj);


            }
     

           

            response.List.AddRange(records);


            return response;

        }

        public void UpdateReturnsDelivery(UpdateReturnsDeliveryDTO param)
        {
            var delivery = _context.WHDeliveries
                                   .Include(p => p.WarehouseDeliveries)
                                   .Where(p => p.Id == param.Id
                                               /*&& p.ApprovedDeliveryDate.HasValue == false*/)
                                   .FirstOrDefault();


            delivery.ApprovedDeliveryDate = param.ApprovedDeliveryDate;
            delivery.DriverName = param.DriverName;
            delivery.PlateNumber = param.PlateNumber;
            delivery.DateUpdated = DateTime.Now;

            foreach(var delItem in delivery.WarehouseDeliveries)
            {
                //  Change DeliveryStatus from Pending to Waiting
                delItem.DeliveryStatus = DeliveryStatusEnum.Waiting;
                delItem.ReleaseStatus = ReleaseStatusEnum.Waiting;
                delItem.DateUpdated = DateTime.Now;
            }

            _context.WHDeliveries.Update(delivery);
            _context.SaveChanges();
        }

        public string GetDeliveryStatusStr(DeliveryStatusEnum? deliveryStatus)
        {
            return EnumExtensions.SplitName(Enum.GetName(typeof(DeliveryStatusEnum), deliveryStatus));
        }
    }
}

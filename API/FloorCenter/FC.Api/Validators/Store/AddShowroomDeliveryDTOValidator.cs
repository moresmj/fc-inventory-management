using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class AddShowroomDeliveryDTOValidator : AbstractValidator<AddShowroomDeliveryDTO>
    {

        private readonly DataContext _context;

        public AddShowroomDeliveryDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Id
            RuleFor(p => p.Id)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid");

            //removed for ticket #115327
            //  Validate DRNumber
            //RuleFor(p => p.DRNumber)
            //   .NotEmpty();
            //RuleFor(p => p)
            //   .Must(CheckDr)
            //       .WithMessage("DR number is required");


            //  Validate Delivery Date
            RuleFor(p => p.DeliveryDate)
                .NotEmpty();

            //removed for ticket #260
            // Validate DRNumber and Deliery Date
            //RuleFor(p => p)
            //    .Must(DRNumberAndDeliveryDateNotYetRegistered)
            //        .WithMessage("DR Number and Delivery Date already registered")
            //        .WithName("DRNumber and DeliveryDate");


            //  Validat Showroom items for delivery
            RuleFor(p => p.ShowroomDeliveries)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .Must(ShouldHaveAtLeastOneItemToBeRequestedForDelivery)
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .SetCollectionValidator(new AddShowroomDeliveryItemsValidator(context));

            RuleFor(p => p)
                .Must(IdShouldMatchListSTOrderId)
                    .WithMessage("Invalid item(s) found on the list.")
                    .WithName("Id & ShowroomDeliveries.STOrderId");

        }
        
        private bool ValidRecord(int id)
        {
            var obj = (from x in _context.STOrders.Where(x => x.Id == id
                                                              && x.DeliveryType != DeliveryTypeEnum.Pickup
                                                              && x.RequestStatus == RequestStatusEnum.Approved)
                      .Include(y => y.OrderedItems)
                       select new STOrder
                       {
                           Id = x.Id,
                           StoreId = x.StoreId,
                           WarehouseId = x.WarehouseId,
                           DeliveryType = x.DeliveryType,
                           OrderType = x.OrderType,
                           PONumber = x.PONumber,
                           TransactionType = x.TransactionType,
                           Remarks = x.Remarks,
                           RequestStatus = x.RequestStatus,
                           PODate = x.PODate,
                           OrderedItems = x.OrderedItems.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList()
                       }).Where(y => y.OrderedItems.Count > 0).FirstOrDefault();

            if (obj == null)
            {
                return false;
            }
            else
            {

                //  Check if the order is for client and
                //  the delivery type is not showroom pickup
                if(obj.OrderType == OrderTypeEnum.ClientOrder &&
                   obj.DeliveryType != DeliveryTypeEnum.ShowroomPickup)
                {
                    return false;
                }

                return true;
            }
        }

        private bool DRNumberAndDeliveryDateNotYetRegistered(AddShowroomDeliveryDTO model)
        {
            if (!string.IsNullOrWhiteSpace(model.DRNumber) && model.DeliveryDate.HasValue)
            {
                var count = _context.STDeliveries.Where(p => p.DRNumber == model.DRNumber && p.DeliveryDate == model.DeliveryDate).Count();
                return (count == 0);
            }

            return true;
        }

        private bool CheckDr(AddShowroomDeliveryDTO model)
        {
            var obj = (from x in _context.STOrders.Where(x => x.Id == model.Id
                                                              && x.DeliveryType != DeliveryTypeEnum.Pickup
                                                              && x.RequestStatus == RequestStatusEnum.Approved)
                      .Include(y => y.OrderedItems)
                       select new STOrder
                       {
                           Id = x.Id,
                           StoreId = x.StoreId,
                           WarehouseId = x.WarehouseId,
                           DeliveryType = x.DeliveryType,
                           OrderType = x.OrderType,
                           PONumber = x.PONumber,
                           TransactionType = x.TransactionType,
                           Remarks = x.Remarks,
                           RequestStatus = x.RequestStatus,
                           PODate = x.PODate,
                           OrderedItems = x.OrderedItems.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList()
                       }).Where(y => y.OrderedItems.Count > 0).FirstOrDefault();


            if(obj.DeliveryType == DeliveryTypeEnum.ShowroomPickup)
            {
                
               if(!String.IsNullOrEmpty(model.DRNumber))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private bool ShouldHaveAtLeastOneItemToBeRequestedForDelivery(ICollection<AddShowroomDeliveryItems> showroomDeliveries)
        {
            if (showroomDeliveries != null && showroomDeliveries.Count > 0)
            {
                return (showroomDeliveries.Where(p => p.Quantity > 0).Count() > 0);
            }

            return true;
        }

        private bool IdShouldMatchListSTOrderId(AddShowroomDeliveryDTO model)
        {
            if (model.ShowroomDeliveries != null && model.ShowroomDeliveries.Count > 0)
            {
                return (model.ShowroomDeliveries.Where(p => p.STOrderId != model.Id).Count() == 0);
            }

            return true;
        }

    }
}

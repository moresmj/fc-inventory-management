using FC.Api.DTOs.Store.Returns;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Warehouse
{
    public class AddPurchaseReturnDeliveryDTOValidator : AbstractValidator<AddPurchaseReturnDeliveryDTO>
    {

        private readonly DataContext _context;

        public AddPurchaseReturnDeliveryDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Id
            RuleFor(p => p.Id)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid");


            //  Validate DRNumber
            RuleFor(p => p.DRNumber)
                .NotEmpty()
                .MaximumLength(50);


            //  Validate Delivery Date
            RuleFor(p => p.DeliveryDate)
                .NotEmpty();


            // Validate DRNumber and Deliery Date
            RuleFor(p => p)
                .Must(DRNumberAndDeliveryDateNotYetRegistered)
                    .WithMessage("DR Number and Delivery Date already registered")
                    .WithName("DRNumber and DeliveryDate");


            //  Validate warehouse items for delivery
            RuleFor(p => p.WarehouseDeliveries)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .Must(ShouldHaveAtLeastOneItemToBeRequestedForDelivery)
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .SetCollectionValidator(new AddPurchaseReturnDeliveryItemsValidator(context));

            RuleFor(p => p)
                .Must(IdShouldMatchListSTOrderId)
                    .WithMessage("Invalid item(s) found on the list.")
                    .WithName("Id & WarehouseDeliveries.STReturnId");

        }

        private bool ValidRecord(int id)
        {
            var obj = (from x in _context.STReturns.Where(x => x.Id == id
                                                              && ((x.ReturnType == ReturnTypeEnum.RTV
                                                              && x.RequestStatus == RequestStatusEnum.Approved) || (x.ReturnType == ReturnTypeEnum.Breakage)))
                      .Include(y => y.PurchasedItems)
                       select new 
                       {
                           PurchasedItems = x.PurchasedItems.Where(z => (z.DeliveryStatus == DeliveryStatusEnum.Waiting || (z.BrokenQuantity >0 && z.DeliveryStatus == DeliveryStatusEnum.Pending))).ToList()
                       }).Where(y => y.PurchasedItems.Count > 0).FirstOrDefault();

            if (obj == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool DRNumberAndDeliveryDateNotYetRegistered(AddPurchaseReturnDeliveryDTO model)
        {
            if (!string.IsNullOrWhiteSpace(model.DRNumber) && model.DeliveryDate.HasValue)
            {
                var count = _context.WHDeliveries.Where(p => p.DRNumber == model.DRNumber && p.DeliveryDate == model.DeliveryDate).Count();
                return (count == 0);
            }

            return true;
        }

        private bool ShouldHaveAtLeastOneItemToBeRequestedForDelivery(ICollection<AddPurchaseReturnDeliveryItems> deliveries)
        {
            if (deliveries != null && deliveries.Count > 0)
            {
                return (deliveries.Where(p => p.Quantity > 0).Count() > 0);
            }

            return true;
        }

        private bool IdShouldMatchListSTOrderId(AddPurchaseReturnDeliveryDTO model)
        {
            if (model.WarehouseDeliveries != null && model.WarehouseDeliveries.Count > 0)
            {
                return (model.WarehouseDeliveries.Where(p => p.STReturnId != model.Id).Count() == 0);
            }

            return true;
        }
    }
}

using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventorySystemAPI.Validators.Store.Request
{
    public class ForDeliveryUpdateItemsValidator : AbstractValidator<STDeliveryDetail>
    {

        private readonly FloorCenterContext _context;

        public ForDeliveryUpdateItemsValidator(FloorCenterContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            //  Validate Id
            RuleFor(p => p.Id)
                .NotEmpty();


            //  Validate STDeliveryId
            RuleFor(p => p.STDeliveryId)
                .NotEmpty();


            //  Validate STInventoryDetailId
            RuleFor(p => p.STInventoryDetailId)
                .NotEmpty();


            //  Validate ItemId
            RuleFor(p => p.ItemId)
                .NotEmpty();


            //  Validate Record
            RuleFor(p => p)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid");


            //  Validate DeliveredQuantity
            RuleFor(p => p.DeliveredQuantity)
                .NotEmpty();

            //TODO: Delivered Quantity should not be more than quantity
            //RuleFor(p => p)
            //    .Must(DeliveredQuantityNotMoreThanQuantity)
            //        .WithMessage("Quantity is not valid");


        }

        private bool ValidRecord(STDeliveryDetail model)
        {
            var count = this._context.STDeliveryDetails.Where(x => x.Id == model.Id && x.STDeliveryId == model.STDeliveryId && x.STInventoryDetailId == model.STInventoryDetailId && x.ItemId == model.ItemId).Count();
            if(count < 1)
            {
                return false;
            }

            return true;
        }

        private bool DeliveredQuantityNotMoreThanQuantity(STDeliveryDetail model)
        {
            if (model.DeliveredQuantity > 0)
            {
                var obj = this._context.STDeliveryDetails.Where(x => x.Id == model.Id && x.STDeliveryId == model.STDeliveryId && x.STInventoryDetailId == model.STInventoryDetailId && x.ItemId == model.ItemId).FirstOrDefault();
                if (obj != null)
                {
                    if (model.DeliveredQuantity > obj.Quantity)
                    {
                        return false;
                    }

                    return true;
                }
            }

            return true;

        }

    }
}

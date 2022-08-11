using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Request;
using System;
using System.Linq;

namespace InventorySystemAPI.Validators.Store.Delivery
{
    public class STDeliveryDetailValidator : AbstractValidator<Models.Store.Request.STDeliveryDetail>
    {

        private readonly FloorCenterContext _context;

        public STDeliveryDetailValidator(FloorCenterContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Quantity
            RuleFor(p => p.STInventoryDetailId)
                    .NotEmpty()
                        .WithMessage("'Inventory Detail Id' should not be empty.");


            //  Validate Item
            RuleFor(p => p)
                    .NotEmpty()
                    .Must(ValidItem)
                        .WithMessage("Item is not valid");


            //  Validate Quantity
            RuleFor(p => p.Quantity)
                    .NotEmpty();

            RuleFor(p => p)
                    .Must(QuantityNotMoreThanApprovedQuantity)
                        .WithMessage("Quantity is not valid");
        }


        private bool ValidItem(STDeliveryDetail model)
        {
            var item = this._context.STInventoryDetails.Where(x => x.Id == model.STInventoryDetailId && x.ItemId == model.ItemId).Count();
            if (item != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool QuantityNotMoreThanApprovedQuantity(STDeliveryDetail model)
        {
            var item = this._context.STInventoryDetails.Where(x => x.Id == model.STInventoryDetailId && x.ItemId == model.ItemId).FirstOrDefault();

            var deliveredQuantity = Convert.ToInt32(this._context.STDeliveryDetails.Where
                                                        (
                                                            x => x.STInventoryDetailId == model.STInventoryDetailId
                                                            && x.ItemId == model.ItemId
                                                        )
                                                        .Sum(x => x.DeliveredQuantity)
                                                    ) + model.Quantity;

            if (item != null)
            {
                if (deliveredQuantity > item.ApprovedQuantity)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

    }
}

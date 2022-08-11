using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Inventory;
using System;
using System.Linq;

namespace InventorySystemAPI.Validators.Request
{
    public class PendingOrderItemsValidator : AbstractValidator<Models.Store.Inventory.STInventoryDetail>
    {

        private readonly FloorCenterContext _context;

        public PendingOrderItemsValidator(FloorCenterContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;
            
            //  Validate ItemId
            RuleFor(p => p)
                    .NotEmpty()
                    .Must(ValidItem)
                        .WithMessage("Item is not valid");


            //  Validate Approved Quantity
            RuleFor(p => p.ApprovedQuantity)
                    .NotEmpty();

            RuleFor(p => p)
                .Must(ApprovedQuantityNotMoreThanRequestedQuantity)
                    .WithMessage("Approved Quantity should not be more than the Requested Quantity");

        }

        private bool ValidItem(STInventoryDetail model)
        {
            var item = this._context.STInventoryDetails.Where(x => x.STInventoryId == model.STInventoryId && x.Id == model.Id && x.ItemId == model.ItemId).Count();
            if (item != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool ApprovedQuantityNotMoreThanRequestedQuantity(STInventoryDetail model)
        {
            var item = this._context.STInventoryDetails.Where(x => x.STInventoryId == model.STInventoryId && x.Id == model.Id && x.ItemId == model.ItemId).FirstOrDefault();
            if(item !=null)
            {
                if(model.ApprovedQuantity > item.RequestedQuantity)
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

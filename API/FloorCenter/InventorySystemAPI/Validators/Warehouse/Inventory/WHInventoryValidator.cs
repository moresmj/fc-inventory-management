using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Warehouse.Inventory;
using InventorySystemAPI.Validators.User;
using System.Linq;

namespace InventorySystemAPI.Validators.Warehouse.Inventory
{
    public class WHInventoryValidator : AbstractValidator<WHInventory>
    {

        private readonly FloorCenterContext _context;

        public WHInventoryValidator(FloorCenterContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            
            //  Validate PONumber
            RuleFor(p => p.PONumber)
                    .NotEmpty();


            //  Validate PODate
            RuleFor(p => p.PODate)
                    .NotEmpty();


            //  Validate DRNumber
            RuleFor(p => p.DRNumber)
                    .NotEmpty();


            //  Validate DRDate
            RuleFor(p => p.DRDate)
                    .NotEmpty();

            
            //  Validate PONumber & DRNumber
            RuleFor(x => x)
                .Must(PONumberDRNumberNotYetRegistered)
                    .WithMessage("Record is already registered")
                    .WithName("PONumber and DRNumber");


            //  Validate ReceivedDate
            RuleFor(p => p.ReceivedDate)
                    .NotEmpty();


            //  Validate CheckedBy
            RuleFor(p => p.CheckedBy)
                    .NotEmpty()
                    .Must(new UserValidator(context, false).IdValid)
                        .WithMessage("{PropertyName} is not valid");


            //  Validate Items
            RuleFor(p => p.DeliveredItems)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be saved.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be saved.")
                .SetCollectionValidator(new WHInventoryDetailValidator(context));

        }


        private bool PONumberDRNumberNotYetRegistered(WHInventory model)
        {
            if(!string.IsNullOrEmpty(model.PONumber) && !string.IsNullOrEmpty(model.DRNumber))
            {
                var count = this._context.WHInventories.Where(x => x.PONumber.ToLower() == model.PONumber.ToLower() && x.DRNumber.ToLower() == model.DRNumber.ToLower()).Count();

                if (count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

    }
}

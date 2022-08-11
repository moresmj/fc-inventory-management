using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Validators.Item;

namespace InventorySystemAPI.Validators.Store.Inventory
{
    public class STInventoryDetailValidator: AbstractValidator<Models.Store.Inventory.STInventoryDetail>
    {

        private readonly FloorCenterContext _context;

        public STInventoryDetailValidator(FloorCenterContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ItemId
            RuleFor(p => p.ItemId)
                    .NotEmpty()
                    .Must(new ItemValidator(context, false).IdValid)
                        .WithMessage("Item is not valid");

            //  Validate Quantity
            RuleFor(p => p.RequestedQuantity)
                    .NotEmpty()
                    .GreaterThan(0);

        }

    }
}

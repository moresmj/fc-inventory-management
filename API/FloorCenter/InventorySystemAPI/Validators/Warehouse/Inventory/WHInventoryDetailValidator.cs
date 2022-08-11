using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Validators.Item;

namespace InventorySystemAPI.Validators.Warehouse.Inventory
{
    public class WHInventoryDetailValidator : AbstractValidator<Models.Warehouse.Inventory.WHInventoryDetail>
    {

        private readonly FloorCenterContext _context;

        public WHInventoryDetailValidator(FloorCenterContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ItemId
            RuleFor(p => p.ItemId)
                    .NotEmpty()
                    .Must(new ItemValidator(context, false).IdValid)
                        .WithMessage("Item is not valid");

            //  Validate Quantity
            RuleFor(p => p.Quantity)
                    .NotEmpty()
                    .GreaterThan(0);

        }
        
    }
}

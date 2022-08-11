using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Releasing;
using InventorySystemAPI.Models.Store.Sales;

namespace InventorySystemAPI.Validators.Store.Releasing
{
    public class STReleaseValidator : AbstractValidator<STRelease>
    {
        private readonly FloorCenterContext _context;

        public STReleaseValidator(FloorCenterContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            //  Validate Delivery Type
            RuleFor(p => p.DeliveryType)
                .NotEmpty()
                .IsInEnum();
            
            //  Validate Date Released only if DeliveryType is Pickup
            RuleFor(p => p.DateReleased)
                .NotEmpty()
                    .When(p => p.DeliveryType == DeliveryTypeEnum.Pickup);


            //  Validate Date Released only if DeliveryType is not Pickup
            RuleFor(p => p.DRNo)
                .NotEmpty()
                    .When(p => p.DeliveryType != DeliveryTypeEnum.Pickup);

            //  Validate Date Released only if DeliveryType is not Pickup
            RuleFor(p => p.DeliveryDate)
                .NotEmpty()
                    .When(p => p.DeliveryType != DeliveryTypeEnum.Pickup);


            //  Validate Released Items only if DeliveryType is Pickup
            RuleFor(p => p.ReleasedItems)
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be picked up.")
                .SetCollectionValidator(new STReleaseDetailValidator(context, DeliveryTypeEnum.Pickup))
                    .When(p => p.DeliveryType == DeliveryTypeEnum.Pickup);


            RuleFor(p => p.ReleasedItems)
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be delivered.")
                .SetCollectionValidator(new STReleaseDetailValidator(context, DeliveryTypeEnum.Store))
                    .When(p => p.DeliveryType == DeliveryTypeEnum.Store);


            RuleFor(p => p.ReleasedItems)
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be delivered.")
                .SetCollectionValidator(new STReleaseDetailValidator(context, DeliveryTypeEnum.Warehouse))
                    .When(p => p.DeliveryType == DeliveryTypeEnum.Warehouse);
        }
    }
}

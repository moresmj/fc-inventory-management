using FC.Api.DTOs.Warehouse;
using FC.Api.Helpers;
using FC.Api.Validators.Item;
using FluentValidation;

namespace FC.Api.Validators.Warehouse
{
    public class WHReceiveDetailDTOValidator : AbstractValidator<WHReceiveDetailDTO>
    {

        private readonly DataContext _context;

        public WHReceiveDetailDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ItemId
            RuleFor(p => p.ItemId)
                    .NotEmpty()
                    .Must(new ItemDTOValidator(context, false).IdValid)
                        .WithMessage("Item is not valid");

            //  Validate Quantity
            RuleFor(p => p.Quantity)
                    .NotEmpty()
                    .GreaterThan(0);


            RuleFor(p => p)
                .Must(ReserveQtyMustNotGreaterReceivedQty)
                    .WithMessage("Reserve quantity must not be greater than received quantity");

        }

        private bool ReserveQtyMustNotGreaterReceivedQty(WHReceiveDetailDTO model)
        {
            if(model.ReservedQuantity == null)
            {
                model.ReservedQuantity = 0;
            }

            if(model.ReservedQuantity > model.Quantity)
            {
                return false;
            }


            return true;
        }

    }
}

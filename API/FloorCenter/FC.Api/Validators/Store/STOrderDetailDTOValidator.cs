using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Validators.Item;
using FluentValidation;

namespace FC.Api.Validators.Store
{
    public class STOrderDetailDTOValidator : AbstractValidator<STOrderDetailDTO>
    {

        private readonly DataContext _context;

        public STOrderDetailDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ItemId
            RuleFor(p => p.ItemId)
                    .NotEmpty()
                    .Must(new ItemDTOValidator(context, false).IdValid)
                        .WithMessage("Item is not valid");

            //  Validate Quantity
            RuleFor(p => p.RequestedQuantity)
                    .NotEmpty()
                    .GreaterThan(0);
            //Validation for quantity
            RuleFor(p => p)
                .Must(orderQtyNotMoreOnStockQty)
                    .WithMessage("Requested Quantity must not more than the Available Quantity");


          

        }


        private bool orderQtyNotMoreOnStockQty(STOrderDetailDTO model)
        {
            if(model.RequestedQuantity > model.AvailableQty)
            {
                return false;
            }
            return true;
        }

    }
}

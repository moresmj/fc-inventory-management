using System.Linq;
using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Validators.Warehouse;
using FluentValidation;

namespace FC.Api.Validators.Store
{
    public class STOrderDTOValidator : AbstractValidator<STOrderDTO>
    {

        private readonly DataContext _context;

        public STOrderDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            //  Validate Vendor
            RuleFor(p => p.WarehouseId)
                    .NotEmpty()
                    .Must(new WarehouseDTOValidator(context, false).IdValid)
                        .WithMessage("{PropertyName} is not valid");


            //Validate PONumber
            RuleFor(p => p.PONumber)
                    .NotEmpty()
                    .Must(NotYetRegistered)
                        .WithMessage("{PropertyName} '{PropertyValue}' is already registered")
                    .When(p => p.isDealer == true);


            //  Validate PODate
            RuleFor(p => p.PODate)
                    .NotEmpty();

            //  Validate Ordered Items
            RuleFor(p => p.OrderedItems)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be requested.")
                .Must(list => list.Count > 0) // true
                    .WithMessage("Please add at least one item to be requested.")
                .Must(list => list.Count > 0 && list.Count <= 7)
                    .WithMessage("Only 7 items are allowed per purchase")
                .SetCollectionValidator(new STOrderDetailDTOValidator(context));

        }

        private bool NotYetRegistered(string poNumber)
        {
            var count = _context.STOrders.Where(p => p.PONumber.ToLower() == poNumber.ToLower()).Count();
            if(count != 0)
            {
                return false;
            }

            return true;
        }
    }
}

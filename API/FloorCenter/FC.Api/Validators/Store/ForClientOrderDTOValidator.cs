using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Validators.Warehouse;
using FC.Core.Domain.Common;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class ForClientOrderDTOValidator : AbstractValidator<ForClientOrderDTO>
    {

        private readonly DataContext _context;

        public ForClientOrderDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            //  Validate Vendor
            RuleFor(p => p.WarehouseId)
                    .NotEmpty()
                    .Must(new WarehouseDTOValidator(context, false).IdValid)
                        .WithMessage("{PropertyName} is not valid");


            //  Validate PONumber
            RuleFor(p => p.PONumber)
                    .NotEmpty()
                    .Must(NotYetRegistered)
                        .WithMessage("{PropertyName} '{PropertyValue}' is already registered")
                    .When(p => p.isDealer == true); ;


            //  Validate PODate
            RuleFor(p => p.PODate)
                    .NotEmpty();


            //  Validate Delivery Mode
            RuleFor(p => p.DeliveryType)
                    .NotEmpty()
                    .IsInEnum();


            //  Validate Client's Name
            RuleFor(p => p.ClientName)
                    .NotEmpty();

            //  Validate Address if order is for delivery
            RuleFor(p => p.Address1)
                .NotEmpty()
                    .When(p => p.DeliveryType == DeliveryTypeEnum.Delivery)
                        .WithMessage("'Address' should not be empty");


            //  Validate Contact Number
            RuleFor(p => p.ContactNumber)
                .Matches("[0-9]")
                    .When(p => p.ContactNumber != string.Empty)
                .NotEmpty()
                    .When(p => p.DeliveryType == DeliveryTypeEnum.Delivery)
                .MinimumLength(6)
                    .WithMessage("'Contact Number' should have atleast 6 characters");

            //  Validate Ordered Items
            RuleFor(p => p.OrderedItems)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be requested.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be requested.")
                .Must(list => list.Count > 0 && list.Count <= 7)
                    .WithMessage("Only 7 items are allowed per purchase")
                .SetCollectionValidator(new STOrderDetailDTOValidator(context));

            RuleFor(p => p.SalesAgent)
                    .NotEmpty();

        }

        private bool NotYetRegistered(string poNumber)
        {
            var count = _context.STOrders.Where(p => p.PONumber.ToLower() == poNumber.ToLower()).Count();
            if (count != 0)
            {
                return false;
            }

            return true;
        }

    }
}

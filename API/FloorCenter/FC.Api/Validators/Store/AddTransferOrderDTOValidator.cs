using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FC.Core.Domain.Common;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class AddTransferOrderDTOValidator : AbstractValidator<AddTransferOrderDTO>
    {
        
        private readonly DataContext _context;
        private ISTStockService _stockService;

        public AddTransferOrderDTOValidator(DataContext context, ISTStockService stockService)
        {
            
            this._context = context;
            _stockService = stockService;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.PONumber)
            .NotEmpty()
            .Must(NotYetRegistered)
                .WithMessage("{PropertyName} '{PropertyValue}' is already registered")
            .When(p => p.isDealer == true);


            //  Validate PODate
            RuleFor(p => p.PODate)
                .NotEmpty();


            //  Validate PaymentMode
            RuleFor(p => p.PaymentMode)
                    .NotEmpty()
                    .IsInEnum();


            //  Validate DeliveryType
            RuleFor(p => p.DeliveryType)
                    .NotEmpty()
                    .IsInEnum();

            //  Validate ClientName
            RuleFor(p => p.SalesAgent)
                .NotEmpty();

            //  Validate ClientName
            RuleFor(p => p.ClientName)
                .NotEmpty();

            //  Validate Contact Number
            RuleFor(p => p.ContactNumber)
                .Matches("[0-9]")
                    .When(p => p.ContactNumber != string.Empty)
                .NotEmpty()
                    .When(p => p.DeliveryType == DeliveryTypeEnum.Delivery)
                .MinimumLength(6)
                    .WithMessage("'Contact Number' should have atleast 6 characters");

            //  Validate Address
            RuleFor(p => p.Address1)
                .NotEmpty()
                    .When(p => p.DeliveryType == DeliveryTypeEnum.Delivery)
                        .WithMessage("'Address' should not be empty");


            //  Validate TransferredItems
            RuleFor(p => p.TransferOrders)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be processed.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be processed.")
                .Must(list => list.Count > 0 && list.Count <= 7)
                    .WithMessage("Only 7 items are allowed per purchase")
                .SetCollectionValidator(new AddTransferOrderStoreValidator(context, _stockService));

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

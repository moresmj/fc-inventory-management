using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FC.Api.Validators.Item;
using FC.Core.Domain.Stores;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class AddTransferOrderStoreValidator : AbstractValidator<AddTransferOrderStoreDTO>
    {
        
        private readonly DataContext _context;
        private ISTStockService _stockService;

        public AddTransferOrderStoreValidator(DataContext context, ISTStockService stockService)
        {
            
            this._context = context;
            _stockService = stockService;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate StoreId
            RuleFor(p => p.StoreId)
                    .NotEmpty()
                    .Must(IsStoreExist)
                        .WithMessage("Store do not exist");

            RuleFor(p => p.TransferredItems)
                    .NotEmpty()
                        .WithMessage("Please add at least one item to be transferred")
                    .Must(list => list.Count > 0)
                        .WithMessage("Please add at least one item to be transferred")
                    .SetCollectionValidator(new AddTransferOrderStoreItemsValidator(context, _stockService));


        }

        private bool IsStoreExist(int? storeId)
        {
            var count = _context.Stores.Where(p => p.Id == storeId).Count();
            if (count == 0)
            {
                return false;
            }

            return true;
        }

    }
}

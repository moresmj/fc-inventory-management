using System.Linq;
using FC.Api.DTOs.Store.ReceiveItems;
using FC.Api.Helpers;
using FluentValidation;

namespace FC.Api.Validators.Store.ReceiveItems
{
    public class ReceiveClientReturnsDTOValidator : AbstractValidator<ReceiveClientReturnsDTO>
    {
        private DataContext _context;

        public ReceiveClientReturnsDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Id
            RuleFor(p => p)
                .Must(ValidRecord)
                    .When(p => p.StoreId.HasValue)
                    .WithMessage("Record is not valid")
                    .WithName("Id & StoreId");

            RuleFor(p => p.ClientPurchasedItems)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be received.")
                    .WithName("ClientPurchasedItems")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be received.")
                    .WithName("ClientPurchasedItems")
                .SetCollectionValidator(new ReceiveClientReturnsItemsValidator(context));

            RuleFor(p => p)
                .Must(AllItemsAreValid)
                    .WithMessage("Found invalid item(s) on the list")
                    .WithName("StoreId & Purchased Items");

        }

        private bool ValidRecord(ReceiveClientReturnsDTO model)
        {
            var service = new Services.Stores.ReceiveItems.ClientReturnService(_context);
            if(service.GetByIdAndStoreId(model.Id, model.StoreId) != null)
            {
                return true;
            }

            return false;
        }

        private bool AllItemsAreValid(ReceiveClientReturnsDTO model)
        {
            if (model.ClientPurchasedItems != null && model.ClientPurchasedItems.Count() > 0)
            {
                return (model.ClientPurchasedItems.Where(p => p.STReturnId != model.Id).Count() == 0);
            }

            return true;
        }

    }
}

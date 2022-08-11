using FC.Api.DTOs.Store.Returns;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Validators.Store.Returns
{
    public class AddPurchaseReturnDTOValidator : AbstractValidator<AddPurchaseReturnDTO>
    {

        private readonly DataContext _context;

        public AddPurchaseReturnDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ReturnFormNumber
            RuleFor(p => p.ReturnFormNumber)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(p => p)
                .Must(ReturnFormNumberNotYetRegistered)
                    .WithMessage("Return Form Number is already registered")
                    .WithName("ReturnFormNumber");


            RuleFor(p => p)
                .Must(AllItemsAreRegisteredInStore)
                    .WithMessage("Found invalid item(s) on the list")
                    .WithName("StoreId & Purchased Items");


            RuleFor(p => p.PurchasedItems)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be returned.")
                    .WithName("PurchasedItems")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be returned.")
                    .WithName("PurchasedItems")
                .Must(ShouldHaveAtLeastOneItemToBeRequestedForReturn)
                    .WithMessage("Please add at least one item to be returned")
                    .WithName("PurchasedItems")
                .SetCollectionValidator(new AddPurchaseReturnItemsValidator(context));

        }

        private bool ReturnFormNumberNotYetRegistered(AddPurchaseReturnDTO model)
        {
            if(model.StoreId.HasValue && !string.IsNullOrWhiteSpace(model.ReturnFormNumber))
            {
                var count = _context.STReturns
                                    .Where(p => p.StoreId == model.StoreId
                                                && p.ReturnFormNumber == model.ReturnFormNumber
                                                && p.RequestStatus != RequestStatusEnum.Cancelled)
                                    .Count();

                if(count != 0)
                {
                    return false;
                }

                return true;
            }

            return true;
        }

        private bool AllItemsAreRegisteredInStore(AddPurchaseReturnDTO model)
        {
            if (model.StoreId.HasValue && (model.PurchasedItems != null && model.PurchasedItems.Count() > 0))
            {
                bool allItemsAreValid = true;

                foreach (var item in model.PurchasedItems)
                {
                    var count = _context.STStocks
                                        .Where(p => p.StoreId == model.StoreId
                                                    && p.ItemId == item.ItemId)
                                        .Count();

                    if (count == 0)
                    {
                        allItemsAreValid = false;
                        break;
                    }
                }

                return allItemsAreValid;
            }

            return true;
        }

        private bool ShouldHaveAtLeastOneItemToBeRequestedForReturn(ICollection<AddPurchaseReturnItems> model)
        {
            if (model != null && model.Count > 0)
            {
                return (model.Where(p => (p.GoodQuantity + p.BrokenQuantity) > 0).Count() > 0);
            }

            return true;
        }
    }
}

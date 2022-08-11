using FC.Api.DTOs.Store.Returns.ClientReturn;
using FC.Api.Helpers;
using FC.Api.Services.Stores.Returns;
using FC.Core.Domain.Common;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Validators.Store.Returns
{
    public class AddClientReturnDTOValidator : AbstractValidator<AddClientReturnDTO>
    {

        private readonly DataContext _context;

        public AddClientReturnDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Id & StoreId
            RuleFor(p => p)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid")
                    .WithName("Id & StoreId");

            //  Validate ClientReturnType
            RuleFor(p => p.ClientReturnType)
                .NotEmpty()
                .IsInEnum();


            RuleFor(p => p.PickupDate)
                .NotEmpty()
                    .When(p => p.ClientReturnType == ClientReturnTypeEnum.RequestPickup);


            RuleFor(p => p.ReturnDRNumber)
                .NotEmpty()
                    .When(p => p.ClientReturnType == ClientReturnTypeEnum.RequestPickup);


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
                .SetCollectionValidator(new AddClientReturnItemsValidator(context));

            RuleFor(p => p)
                .Must(AllItemsAreValid)
                    .WithMessage("Found invalid item(s) on the list")
                    .WithName("StoreId & Purchased Items");

        }

        private bool ValidRecord(AddClientReturnDTO model)
        {
            if(model.StoreId.HasValue)
            {
                var service = new ClientReturnService(_context);
                if(service.GetByIdAndStoreId(model.Id, model.StoreId) == null)
                {
                    return false;
                }
            }

            return true;
        }
        
        private bool AllItemsAreValid(AddClientReturnDTO model)
        {
            if (model.PurchasedItems != null && model.PurchasedItems.Count() > 0)
            {
                return (model.PurchasedItems.Where(p => p.STSalesId != model.Id).Count() == 0);
            }

            return true;
        }

        private bool ShouldHaveAtLeastOneItemToBeRequestedForReturn(ICollection<AddClientReturnItems> model)
        {
            if (model != null && model.Count > 0)
            {
                return (model.Where(p => p.Quantity > 0).Count() > 0);
            }

            return true;
        }
    }
}

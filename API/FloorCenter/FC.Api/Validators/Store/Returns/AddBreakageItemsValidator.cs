using FC.Api.DTOs.Store.Returns;
using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Linq;

namespace FC.Api.Validators.Store.Returns
{
    public class AddBreakageItemsValidator : AbstractValidator<AddBreakageItems>
    {

        private readonly DataContext _context;

        public AddBreakageItemsValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            RuleFor(p => p)
                    .Must(ValidateItem)
                        .WithMessage("Item is not valid")
                        .WithName("StoreId & ItemId");


           
            //  Validate BrokenQuantity
            RuleFor(p => p.BrokenQuantity)
                    .GreaterThanOrEqualTo(0);


            //  Validate Total Quantity
            RuleFor(p => p)
                    .Must(GoodAndBrokenQuantityMustBeMoreThanZero)
                        .When(p => p.GoodQuantity.HasValue && p.BrokenQuantity.HasValue)
                        .WithMessage("Total Quantity to be returned should be more than 0")
                        .WithName("GoodQuantity & BrokenQuantity");

            RuleFor(p => p)
                    .Must(GoodAndBrokenQuantityMustNotBeMoreThanAvailableQuantity)
                        .WithMessage("Total Quantity to be returned should not be more than the available quantity")
                        .WithName("GoodQuantity & BrokenQuantity");

        }
        
        private bool ValidateItem(AddBreakageItems model)
        {
            if (model.StoreId.HasValue)
            {
                var item = this._context.STStocks
                                        .Where(x => x.ItemId == model.ItemId 
                                                    && x.StoreId == model.StoreId)
                                        .Count();
                if (item == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }

        private bool GoodAndBrokenQuantityMustBeMoreThanZero(AddBreakageItems model)
        {
            if((model.BrokenQuantity) > 0)
            {
                return true;
            }

            return false;
        }

        private bool GoodAndBrokenQuantityMustNotBeMoreThanAvailableQuantity(AddBreakageItems model)
        {
            if (model.StoreId.HasValue)
            {
                var service = new STStockService(_context);
                service.stStock = _context.STStocks;
                service.stSales = _context.STSales;
                var stockQty = service.GetItemAvailableQuantity(model.ItemId, model.StoreId, true);
                if (model.BrokenQuantity > stockQty)
                {
                    return false;
                }
            }

            return true;
        }

    }
}

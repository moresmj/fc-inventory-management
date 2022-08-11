using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class AddSalesDTOValidator : AbstractValidator<AddSalesDTO>
    {

        private readonly DataContext _context;

        public AddSalesDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            //  Validate SINumber
            RuleFor(p => p.SINumber)
                    .NotEmpty()
                    .MaximumLength(50);


            //  Validate ReleaseDate
            RuleFor(p => p.ReleaseDate)
                    .NotEmpty();


            //  Validate SalesAgent
            RuleFor(p => p.SalesAgent)
                    .NotEmpty();


            RuleFor(p => p)
                .Must(AllItemsAreRegisteredInStore)
                    .WithMessage("Found invalid item(s) on the list")
                    .WithName("StoreId & Sold Items");
            

            //  Validate SoldItems
            RuleFor(p => p.SoldItems)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be sold.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be sold.")
                .SetCollectionValidator(new AddSalesItemsValidator(context));

        }

        private bool AllItemsAreRegisteredInStore(AddSalesDTO model)
        {
            if(model.StoreId.HasValue && (model.SoldItems != null && model.SoldItems.Count() > 0))
            {
                bool foundInvalid = false;
                
                foreach (var item in model.SoldItems)
                {
                    var count = _context.STStocks
                                        .Where(p => p.StoreId == model.StoreId
                                                    && p.ItemId == item.ItemId)
                                        .Count();

                    if(count > 0)
                    {
                        foundInvalid = true;
                        break;
                    }
                }

                return foundInvalid;
            }

            return true;    
        }
    }
}

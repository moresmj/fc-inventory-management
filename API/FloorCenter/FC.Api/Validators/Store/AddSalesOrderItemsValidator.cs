using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class AddSalesOrderItemsValidator : AbstractValidator<AddSalesOrderItems>
    {

        private readonly DataContext _context;

        public AddSalesOrderItemsValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Item
            RuleFor(p => p)
                    .NotEmpty()
                    .Must(ValidItem)
                        .WithMessage("Item is not valid")
                        .WithName("ItemId");

            //  Validate Quantity
            RuleFor(p => p.Quantity)
                    .NotEmpty()
                    .GreaterThan(0);

            RuleFor(p => p)
                    .Must(NotMoreThanAvailableQuantity)
                        .WithMessage("'Quantity' should not be more than the Available Quantity")
                        .WithName("Quantity");

        }

        private bool NotMoreThanAvailableQuantity(AddSalesOrderItems model)
        {
            if (model.StoreId.HasValue)
            {
                var service = new STStockService(_context);
                service.stStock = _context.STStocks;
                service.stSales = _context.STSales;
                if (model.Quantity > service.GetItemAvailableQuantity(model.ItemId, model.StoreId, true))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidItem(AddSalesOrderItems model)
        {
            if (model.StoreId.HasValue)
            {
                var item = this._context.STStocks.Where(x => x.ItemId == model.ItemId && x.StoreId == model.StoreId).Count();
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

    }
}

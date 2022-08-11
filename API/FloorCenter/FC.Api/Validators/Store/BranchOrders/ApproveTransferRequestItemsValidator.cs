using FC.Api.DTOs.Store.BranchOrders;
using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Store.BranchOrders
{
    public class ApproveTransferRequestItemsValidator : AbstractValidator<ApproveTransferRequestItems>
    {

        private readonly DataContext _context;

        public ApproveTransferRequestItemsValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ItemId
            RuleFor(p => p)
                    .NotEmpty()
                    .Must(ValidItem)
                        .WithMessage("Item is not valid")
                        .WithName("ItemId");

            //  Validate ApprovedQuantity
            RuleFor(p => p.ApprovedQuantity)
                    .NotEmpty()
                    .GreaterThanOrEqualTo(0);

            RuleFor(p => p)
                    .Must(NotMoreThanRequestedQuantity)
                        .WithMessage("'Approved Quantity' should not be more than the 'Requested Quantity'")
                        .WithName("ApprovedQuantity")
                    .Must(NotMoreThanAvailableQuantity)
                        .WithMessage("'Quantity' should not be more than the Available Quantity")
                        .WithName("ApprovedQuantity");
        }

        private bool NotMoreThanRequestedQuantity(ApproveTransferRequestItems model)
        {
            var record = _context.STOrderDetails.Where(p => p.Id == model.Id && p.ItemId == model.ItemId).FirstOrDefault();
            if (record != null)
            {
                if (model.ApprovedQuantity > record.RequestedQuantity)
                {
                    return false;
                }
            }

            return true;
        }

        private bool NotMoreThanAvailableQuantity(ApproveTransferRequestItems model)
        {
            if (model.StoreId.HasValue)
            {
                var service = new STStockService(_context);
                if (model.ApprovedQuantity > service.GetItemAvailableQuantity(model.ItemId, model.StoreId, true))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidItem(ApproveTransferRequestItems model)
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

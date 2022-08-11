using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Inventory;
using System.Linq;

namespace InventorySystemAPI.Validators.Request
{
    public class PendingOrderValidator : AbstractValidator<Models.Request.PendingOrder>
    {

        private readonly FloorCenterContext _context;

        public PendingOrderValidator(FloorCenterContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.Id)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid");
            

            RuleFor(p => p.RequestedItems)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be approved.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be approved.")
                .SetCollectionValidator(new PendingOrderItemsValidator(context));
        }

        private bool ValidRecord(int id)
        {
            var count = this._context.STInventories.Where(x => x.Id == id && x.RequestStatus == RequestStatusEnum.Pending).Count();
            if (count != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

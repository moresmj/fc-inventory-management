using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Warehouse.Stock;
using InventorySystemAPI.Validators.Warehouse;

namespace InventorySystemAPI.Validators.Store.Inventory
{
    public class STInventoryValidator : AbstractValidator<Models.Store.Inventory.STInventory>
    {

        private readonly FloorCenterContext _context;

        public STInventoryValidator(FloorCenterContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Transaction Type
            RuleFor(p => p.TransactionType)
                    .NotEmpty()
                    .IsInEnum();


            //  Validate Vendor
            RuleFor(p => p.WarehouseId)
                    .NotEmpty()
                        .When(p => p.TransactionType == TransactionTypeEnum.PO || p.TransactionType == TransactionTypeEnum.Transfer)
                    .Must(new WarehouseValidator(context, false).IdValid)
                        .WithMessage("{PropertyName} is not valid");


            //  Validate PONumber
            RuleFor(p => p.PONumber)
                    .NotEmpty()
                        .When(p => p.TransactionType == TransactionTypeEnum.PO || p.TransactionType == TransactionTypeEnum.Transfer);


            //  Validate PODate
            RuleFor(p => p.PODate)
                    .NotEmpty()
                        .When(p => p.TransactionType == TransactionTypeEnum.PO || p.TransactionType == TransactionTypeEnum.Transfer);


            //  Validate Requested Items
            RuleFor(p => p.RequestedItems)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be requested.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be requested.")
                .SetCollectionValidator(new STInventoryDetailValidator(context));

        }

    }
}

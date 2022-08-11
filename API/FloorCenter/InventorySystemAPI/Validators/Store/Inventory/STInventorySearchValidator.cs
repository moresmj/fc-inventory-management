using FluentValidation;

namespace InventorySystemAPI.Validators.Store.Inventory
{
    public class STInventorySearchValidator : AbstractValidator<Models.Store.Inventory.STInventorySearch>
    {

        public STInventorySearchValidator()
        {

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Transaction Type
            RuleFor(p => p.TransactionType)
                    .IsInEnum();


            //  Validate Request Status
            RuleFor(p => p.RequestStatus)
                    .IsInEnum();

        }

    }
}

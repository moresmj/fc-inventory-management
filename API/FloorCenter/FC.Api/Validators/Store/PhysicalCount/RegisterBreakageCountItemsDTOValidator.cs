using FC.Api.DTOs.Store.PhysicalCount;
using FC.Api.Helpers;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Store.PhysicalCount
{
    public class RegisterBreakageCountItemsDTOValidator : AbstractValidator<RegisterStoreBreakageItems>
    {

        private readonly DataContext _context;

        public RegisterBreakageCountItemsDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ItemId
            RuleFor(p => p)
                .Must(ValidItem)
                    .WithMessage("Record is not valid")
                    .WithName("ItemId");

            //  Validate PhysicalCount
            RuleFor(p => p.Broken)
                .NotEmpty();

        }


        private bool ValidItem(RegisterStoreBreakageItems model)
        {
            if (model.StoreId.HasValue)
            {
                var count = _context.Items
                                    .Where(p => p.Id == model.ItemId)
                                    .Count();

                if (count == 0)
                {
                    return false;
                }
            }

            return true;
        }

    }
}

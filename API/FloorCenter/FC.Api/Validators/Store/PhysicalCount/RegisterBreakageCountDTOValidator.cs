using FC.Api.DTOs.Store.PhysicalCount;
using FC.Api.Helpers;
using FluentValidation;

namespace FC.Api.Validators.Store.PhysicalCount
{
    public class RegisterBreakageCountDTOValidator : AbstractValidator<RegisterStoreBreakageDTO>
    {

        private readonly DataContext _context;

        public RegisterBreakageCountDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.Details)
                .NotEmpty()
                    .WithMessage("Please add at least one record to be imported.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one record to be imported.")
                .SetCollectionValidator(new RegisterBreakageCountItemsDTOValidator(context));

        }
    }
}

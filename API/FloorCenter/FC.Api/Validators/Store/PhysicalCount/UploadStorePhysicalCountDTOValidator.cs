using FC.Api.DTOs.Store.PhysicalCount;
using FC.Api.Helpers;
using FluentValidation;

namespace FC.Api.Validators.Store.PhysicalCount
{
    public class UploadStorePhysicalCountDTOValidator : AbstractValidator<UploadStorePhysicalCountDTO>
    {

        private readonly DataContext _context;

        public UploadStorePhysicalCountDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.Details)
                .NotEmpty()
                    .WithMessage("Please add at least one record to be imported.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one record to be imported.")
                .SetCollectionValidator(new UploadStorePhysicalCountItemsValidator(context));

        }
    }
}

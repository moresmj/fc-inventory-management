using FC.Api.DTOs.Store.Releasing;
using FC.Api.Helpers;
using FC.Api.Services.Stores.Releasing;
using FluentValidation;

namespace FC.Api.Validators.Store.Releasing
{
    public class ReleaseForClientOrderDTOValidator : AbstractValidator<ReleaseForClientOrderDTO>
    {

        private readonly DataContext _context;

        public ReleaseForClientOrderDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Id
            RuleFor(p => p)
                .Must(RecordExist)
                    .WithMessage("Record is not valid")
                    .WithName("Id");


            //  Validate SINumber
            RuleFor(p => p.SINumber)
                    .NotEmpty()
                    .MaximumLength(50);


            //  Validate ORNumber
            RuleFor(p => p.ORNumber)
                    .NotEmpty()
                    .MaximumLength(50);

            //  Validate DRNumber
            RuleFor(p => p.DRNumber)
                    .NotEmpty()
                    .MaximumLength(50);

            //Validate Sales agent
            RuleFor(p => p.SalesAgent)
                    .NotEmpty();
        }

        private bool RecordExist(ReleaseForClientOrderDTO model)
        {
            if (model.StoreId.HasValue)
            {
                var service = new ForClientOrderService(_context);
                if (service.GetByIdAndStoreId(model.Id, model.StoreId) == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

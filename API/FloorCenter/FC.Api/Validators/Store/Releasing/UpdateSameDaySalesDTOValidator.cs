using FC.Api.DTOs.Store.Releasing;
using FC.Api.Helpers;
using FC.Api.Services.Stores.Releasing;
using FluentValidation;

namespace FC.Api.Validators.Store.Releasing
{
    public class UpdateSameDaySalesDTOValidator : AbstractValidator<UpdateSameDaySalesDTO>
    {

        private readonly DataContext _context;

        public UpdateSameDaySalesDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Id
            RuleFor(p => p)
                .Must(RecordExist)
                    .WithMessage("Record is not valid")
                    .WithName("Id");
            

            //  Validate ORNumber
            RuleFor(p => p.ORNumber)
                    //.NotEmpty()
                    .MaximumLength(50);


            //  Validate ClientName
            RuleFor(p => p.ClientName)
                    .NotEmpty();


            RuleFor(p => p.DRNumber)
                    //.NotEmpty()
                    .MaximumLength(50);


            //Validate Sales agent
            RuleFor(p => p.SalesAgent)
                    .NotEmpty();


            //  Validate Contact Number
            RuleFor(p => p.ContactNumber)
                .Matches("[0-9]")
                    .When(p => p.ContactNumber != string.Empty);

        }

        private bool RecordExist(UpdateSameDaySalesDTO model)
        {
            if (model.StoreId.HasValue)
            {
                var service = new SameDaySalesService(_context);
                if (service.GetByIdAndStoreId(model.Id, model.StoreId) == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
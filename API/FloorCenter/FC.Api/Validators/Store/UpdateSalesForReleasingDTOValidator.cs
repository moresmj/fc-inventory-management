using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FluentValidation;

namespace FC.Api.Validators.Store
{
    public class UpdateSalesForReleasingDTOValidator : AbstractValidator<UpdateSalesForReleasingDTO>
    {

        private readonly DataContext _context;

        public UpdateSalesForReleasingDTOValidator(DataContext context)
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
            //RuleFor(p => p.DRNumber)
            //        .NotEmpty();


            //  Validate ClientName
            RuleFor(p => p.ClientName)
                    .NotEmpty();

            //Validate Sales agent
            RuleFor(p => p.SalesAgent)
                    .NotEmpty();


            //  Validate Contact Number
            RuleFor(p => p.ContactNumber)
                .Matches("[0-9]")
                    .When(p => p.ContactNumber != string.Empty);



        }

        private bool RecordExist(UpdateSalesForReleasingDTO model)
        {
            if (model.StoreId.HasValue)
            {
                var service = new STSalesService(_context);
                if (service.GetSalesForReleasingById(model.Id, model.StoreId) == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

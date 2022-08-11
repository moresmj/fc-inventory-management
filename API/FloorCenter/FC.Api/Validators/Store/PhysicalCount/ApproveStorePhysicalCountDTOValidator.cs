using System.Linq;
using FC.Api.DTOs.Store.PhysicalCount;
using FC.Api.Helpers;
using FC.Api.Services.Stores.PhysicalCount;
using FluentValidation;

namespace FC.Api.Validators.Store.PhysicalCount
{
    public class ApproveStorePhysicalCountDTOValidator : AbstractValidator<ApproveStorePhysicalCountDTO>
    {

        private readonly DataContext _context;

        public ApproveStorePhysicalCountDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Id
            RuleFor(p => p.Id)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid")
                    .WithName("Id");

            RuleFor(p => p.Details)
                .NotEmpty()
                    .WithMessage("Please add at least one record to be approved.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one record to be approved.")
                .SetCollectionValidator(new ApproveStorePhysicalCountItemsValidator(context));

            RuleFor(p => p)
                .Must(IdShouldMatchListSTImportId)
                    .WithMessage("Invalid item(s) found on the list.")
                    .WithName("Id & Details.STImportId");

        }



        private bool ValidRecord(int Id)
        {
            var service = new UploadPhysicalCountService(_context);
            if(service.GetById(Id) == null)
            {
                return false;
            }

            return true;
        }

        private bool IdShouldMatchListSTImportId(ApproveStorePhysicalCountDTO model)
        {
            if (model.Details != null && model.Details.Count > 0)
            {
                return (model.Details.Where(p => p.STImportId != model.Id).Count() == 0);
            }

            return true;
        }
    }
}

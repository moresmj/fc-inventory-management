using FC.Api.DTOs.Store.PhysicalCount;
using FC.Api.Helpers;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Store.PhysicalCount
{
    public class ApproveStorePhysicalCountItemsValidator : AbstractValidator<ApproveStorePhysicalCountItems>
    {

        private readonly DataContext _context;

        public ApproveStorePhysicalCountItemsValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ItemId
            RuleFor(p => p)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid")
                    .WithName("Id, STImportId & ItemId");

            RuleFor(p => p.AllowUpdate)
                .NotEmpty();

        }

        private bool ValidRecord(ApproveStorePhysicalCountItems model)
        {
            var count = _context.STImportDetails
                                .Where(p => p.Id == model.Id
                                            && p.STImportId == model.STImportId
                                            && p.ItemId == model.ItemId)
                                .Count();

            if (count == 0)
            {
                return false;
            }

            return true;
        }

        
    }
}

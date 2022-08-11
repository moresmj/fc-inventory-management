using FC.Api.DTOs.Warehouse.PhysicalCount;
using FC.Api.Helpers;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Warehouse.PhysicalCount
{
    public class ApproveWarehousePhysicalCountItemsValidator : AbstractValidator<ApproveWarehousePhysicalCountItems>
    {

        private readonly DataContext _context;

        public ApproveWarehousePhysicalCountItemsValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ItemId
            RuleFor(p => p)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid")
                    .WithName("Id, WHImportId & ItemId");

            RuleFor(p => p.AllowUpdate)
                .NotEmpty();

        }

        private bool ValidRecord(ApproveWarehousePhysicalCountItems model)
        {
            var count = _context.WHImportDetails
                                .Where(p => p.Id == model.Id
                                            && p.WHImportId == model.WHImportId
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

using FC.Api.DTOs.Warehouse.PhysicalCount;
using FC.Api.Helpers;
using FluentValidation;
using System.Linq;
using System.Text.RegularExpressions;

namespace FC.Api.Validators.Warehouse.PhysicalCount
{
    public class UploadWarehousePhysicalCountItemsValidator : AbstractValidator<UploadWarehousePhysicalCountItems>
    {

        private readonly DataContext _context;

        public UploadWarehousePhysicalCountItemsValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ItemId
            //insert new items
            RuleFor(p => p)
                .Must(ValidItem)
                    .WithMessage(p => string.Format("Record with ID {0} is not valid", p.ItemId))
                    .WithName("ItemId");

            //  Validate SystemCount
            //RuleFor(p => p.SystemCount)
            //    .NotEmpty()
            //    .WithMessage(p => string.Format("Record with ID {0} 'System Count' should not be empty.", p.ItemId));

            //  Validate PhysicalCount
            RuleFor(p => p.PhysicalCount)
                .NotEmpty()
                .WithMessage(p => string.Format("Record with ID {0} 'Physical Count' should not be empty.", p.ItemId));


            RuleFor(p => p)
                .Must(CheckForDecimal)
                .WithMessage("Record must only contain whole numbers")
                .WithName("Physical Count");

            //Added requested by client
            RuleFor(p => p.Remarks)
                .NotEmpty();

            //RuleFor(p => p)
            //.Must(brokenMustNotGreaterPC)
            //    .WithMessage(p => string.Format("Record with ID {0} Broken item must not be greater than Physical count", p.ItemId));

            //RuleFor(p => p)
            //    .Must(SystemAndPhysicalCountMustBeGreaterThanZero)
            //        .WithMessage("System Count & Physical Count not valid")
            //        .WithName("SystemCount & PhysicalCount");

        }

        //private bool ValidItem(UploadWarehousePhysicalCountItems model)
        //{
        //    if(model.WarehouseId.HasValue)
        //    {
        //        var count = _context.WHStocks
        //                            .Where(p => p.WarehouseId == model.WarehouseId
        //                                        && p.ItemId == model.ItemId)
        //                            .Count();

        //        if(count == 0)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        private bool ValidItem(UploadWarehousePhysicalCountItems model)
        {
            if (model.WarehouseId.HasValue)
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

        private bool SystemAndPhysicalCountMustBeGreaterThanZero(UploadWarehousePhysicalCountItems model)
        {
            if(model.PhysicalCount <= 0 && model.SystemCount <= 0)
            {
                return false;
            }

            return true;
        }

        private bool CheckForDecimal(UploadWarehousePhysicalCountItems model)
        {
            Regex regex = new Regex(@"^[0-9]+$");
            if (model.PhysicalCount.HasValue)
            {
                return regex.IsMatch(model.PhysicalCount.ToString());
            }
            return false;
        }

        //private bool brokenMustNotGreaterPC(UploadWarehousePhysicalCountItems model)
        //{

        //        if (model.Broken > model.PhysicalCount)
        //        {
        //            return false;
        //        }

        //        return true;
        //}
    }
}

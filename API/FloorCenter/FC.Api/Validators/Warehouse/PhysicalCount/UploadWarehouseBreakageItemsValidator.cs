using FC.Api.DTOs.Warehouse.PhysicalCount;
using FC.Api.Helpers;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Warehouse.PhysicalCount
{
    public class UploadWarehouseBreakageItemsValidator : AbstractValidator<UploadWarehouseBreakageItems>
    {

        private readonly DataContext _context;

        public UploadWarehouseBreakageItemsValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ItemId
            //insert new items
            //RuleFor(p => p)
            //    .Must(ValidItem)
            //        .WithMessage(p => string.Format("Record with ID {0} is not valid", p.ItemId))
            //        .WithName("ItemId");

            //  Validate SystemCount
            //RuleFor(p => p.SystemCount)
            //    .NotEmpty()
            //    .WithMessage(p => string.Format("Record with ID {0} 'System Count' should not be empty.", p.ItemId));

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

        private bool SystemAndPhysicalCountMustBeGreaterThanZero(UploadWarehouseBreakageItems model)
        {
            if(model.SystemCount <= 0 && model.SystemCount <= 0)
            {
                return false;
            }

            return true;
        }

        private bool brokenMustNotGreaterPC(UploadWarehouseBreakageItems model)
        {
         
                if (model.Broken > model.SystemCount)
                {
                    return false;
                }

                return true;
        }
    }
}

using FC.Api.DTOs.Warehouse.PhysicalCount;
using FC.Api.Helpers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Warehouse.PhysicalCount
{
    public class AdjustReservedItemQuantityDetailsValidator : AbstractValidator<AdjustReservedItemQuantityDetails>
    {

        private readonly DataContext _context;

        public AdjustReservedItemQuantityDetailsValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            RuleFor(p => p)
                .Must(ValidItem)
                    .WithMessage(p => string.Format("Record with ID {0} is not valid", p.ItemId))
                    .WithName("ItemId");


            RuleFor(p => p.Adjustment)
                .NotEmpty()
                    .GreaterThan(0);

            RuleFor(p => p)
                .Must(AdjustmentNotGreaterThanReserved)
                    .WithMessage(p => string.Format("Adjustment quantity should not be greater than reserved quantity"));

        }


        private bool ValidItem(AdjustReservedItemQuantityDetails model)
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


        private bool AdjustmentNotGreaterThanReserved(AdjustReservedItemQuantityDetails model)
        {
            if (model.WarehouseId.HasValue)
            {
                var reserved = _context.WHStockSummary.Where(p => p.WarehouseId == model.WarehouseId && p.ItemId == model.ItemId).OrderByDescending(p => p.Id).Select(p => p.Reserved).FirstOrDefault();

                if (model.Adjustment  > reserved)
                {
                    return false;
                }
            }

            return true;
        }




    }
}

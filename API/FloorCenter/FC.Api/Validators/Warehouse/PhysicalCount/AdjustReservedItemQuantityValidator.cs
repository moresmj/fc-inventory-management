using FC.Api.DTOs.Warehouse.PhysicalCount;
using FC.Api.Helpers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Warehouse.PhysicalCount
{
    public class AdjustReservedItemQuantityValidator : AbstractValidator<AdjustReservedItemQuantity>
    {
        private readonly DataContext _context;

        public AdjustReservedItemQuantityValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.Details)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be adjusted.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be adjusted.")
                .SetCollectionValidator(new AdjustReservedItemQuantityDetailsValidator(context));
        }
    }
}

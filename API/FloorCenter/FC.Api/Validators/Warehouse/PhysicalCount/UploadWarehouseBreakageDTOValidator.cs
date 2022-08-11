using FC.Api.DTOs.Warehouse.PhysicalCount;
using FC.Api.Helpers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Warehouse.PhysicalCount
{
    public class UploadWarehouseBreakageDTOValidator : AbstractValidator<UploadWarehouseBreakageDTO>
    {

        private readonly DataContext _context;

        public UploadWarehouseBreakageDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.Details)
                .NotEmpty()
                    .WithMessage("Please add at least one record to be imported.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one record to be imported.")
                .SetCollectionValidator(new UploadWarehouseBreakageItemsValidator(context));




        }


    }
}

using FC.Api.DTOs.Store.AdvanceOrder;
using FC.Api.Helpers;
using FC.Api.Validators.Item;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Store.AdvanceOrders
{
    public class STAdvanceOrderDetailsDTOValidator : AbstractValidator<STAdvanceOrderDetailsDTO>
    {

        private readonly DataContext _context;


        public STAdvanceOrderDetailsDTOValidator(DataContext context)
        {
            this._context = context;


            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.ItemId)
                .NotEmpty()
                    .Must(new ItemDTOValidator(context, false).IdValid)
                        .When(p => p.isCustom == false)
                        .WithMessage("Item is not valid");

            RuleFor(p => p.Code)
                .NotEmpty()
                    .When(p => p.isCustom == true);

            RuleFor(p => p.sizeId)
                .NotEmpty()
                    .When(p => p.isCustom == true);

            RuleFor(p => p.Quantity)
                .NotEmpty()
                    .When(p => p.forUpdate == false);

            RuleFor(p => p.ApprovedQuantity)
                .NotEmpty()
                    .When(p => p.forUpdate == true);
        }
    }
}

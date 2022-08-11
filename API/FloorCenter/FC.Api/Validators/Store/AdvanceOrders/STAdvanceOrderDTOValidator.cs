using FC.Api.DTOs.Store.AdvanceOrder;
using FC.Api.Helpers;
using FC.Api.Validators.Warehouse;
using FluentValidation;
using FC.Core.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Store.AdvanceOrders
{
    public class STAdvanceOrderDTOValidator : AbstractValidator<STAdvanceOrderDTO>
    {

        private readonly DataContext _context;

        public STAdvanceOrderDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            RuleFor(p => p.WarehouseId)
                    .NotEmpty()
                    .Must(new WarehouseDTOValidator(context, false).IdValid)
                        .WithMessage("{PropertyName} is not Valid")
                     .When(p => p.forUpdate == false);


            RuleFor(p => p.ClientName)
                    .NotEmpty()
                    .When(p => p.forUpdate == false);

            RuleFor(p => p.SalesAgent)
                    .NotEmpty()
                    .When(p => p.forUpdate == false);

            RuleFor(p => p.Address1)
                    .NotEmpty()
                    .When(p => p.forUpdate == false);

            RuleFor(p => p.ContactNumber)
                   .NotEmpty()
                   .MinimumLength(6)
                   .WithMessage("'Contact Number' should have atleast 6 characters")
                   .When(p => p.forUpdate == false);

            //  Validate Ordered Items
            RuleFor(p => p.AdvanceOrderDetails)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be requested.")
                .Must(list => list.Count > 0) // true
                    .WithMessage("Please add at least one item to be requested.")
                .Must(list => list.Count > 0 && list.Count <= 7)
                    .WithMessage("Only 7 items are allowed per purchase")
                .SetCollectionValidator(new STAdvanceOrderDetailsDTOValidator(context));

            RuleFor(p => p.ChangeStatusReason)
                .NotEmpty()
                .When(p => p.forUpdate == true)
                .When(p => p.OrderStatus != null)
                    .WithMessage("'Change Status Reason' should not be empty");

        }
    }
}

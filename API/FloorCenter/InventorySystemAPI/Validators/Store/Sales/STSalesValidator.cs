using System;
using System.Linq;
using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Sales;
using InventorySystemAPI.Validators.User;

namespace InventorySystemAPI.Validators.Store.Sales
{
    public class STSalesValidator : AbstractValidator<STSales>
    {

        private readonly FloorCenterContext _context;

        public STSalesValidator(FloorCenterContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate SIPONumber
            RuleFor(p => p.SIPONumber)
                    .NotEmpty()
                    .Must(NotAlreadyRegistered)
                        .WithMessage("{PropertyName} is already registered");


            //  Validate SIPODate
            RuleFor(p => p.SIPODate)
                    .NotEmpty();


            //  Validate PaymentType
            RuleFor(p => p.PaymentType)
                    .NotEmpty()
                    .IsInEnum();


            //  Validate Delivery Type
            RuleFor(p => p.DeliveryType)
                    .NotEmpty()
                    .IsInEnum();


            //  Validate Agent
            RuleFor(p => p.Agent)
                .NotEmpty()
                .Must(new UserValidator(context, false).IdValid)
                    .WithMessage("{PropertyName} is not valid");


            //  Validate OrderedItems
            RuleFor(p => p.OrderedItems)
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be sold.")
                .SetCollectionValidator(new STSalesDetailValidator(context));


        }

        private bool NotAlreadyRegistered(string SIPONumber)
        {
            var obj = this._context.STSales.Where(x => x.SIPONumber.ToLower() == SIPONumber.ToLower()).Count();

            if (obj >= 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

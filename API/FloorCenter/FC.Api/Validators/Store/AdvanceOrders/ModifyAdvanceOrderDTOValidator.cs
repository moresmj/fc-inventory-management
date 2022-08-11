using FC.Api.DTOs.Store.AdvanceOrders;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Store.AdvanceOrders
{
    public class ModifyAdvanceOrderDTOValidator : AbstractValidator<ModifyAdvanceOrderDTO>
    {
        private readonly DataContext _context;


        public ModifyAdvanceOrderDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //Validate PONumber
            RuleFor(p => new { p.PONumber, p.StoreId})
                    //.NotEmpty()
                    .Must(  x => NotYetRegistered(x.PONumber, x.StoreId))
                        .WithMessage("PO Number is already registered");
                   // .When(p => p.isDealer == true);

            //Validate DeliveryType
            RuleFor(p => p.DeliveryType)
                .NotEmpty();


            RuleFor(p => p)
                .Must(CheckRecord)
                .WithMessage("Cannot modify order that is already approved");

        }

        private bool NotYetRegistered(string poNumber, int? storeId)
        {
            if(poNumber != null)
            {
                var count = _context.STOrders.Where(p => p.PONumber.ToLower() == poNumber.ToLower() && p.StoreId == storeId).Count();
                if (count != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckRecord(ModifyAdvanceOrderDTO model)
        {
            var rec = _context.STOrders.Where(p => p.Id == model.Id && p.RequestStatus == RequestStatusEnum.Pending).FirstOrDefault();

            return (rec != null);

        }
    }
}

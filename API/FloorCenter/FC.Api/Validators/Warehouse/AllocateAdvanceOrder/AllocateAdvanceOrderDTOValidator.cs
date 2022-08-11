using FC.Api.DTOs.Warehouse.AllocateAdvanceOrder;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Warehouse.AllocateAdvanceOrder
{
    public class AllocateAdvanceOrderDTOValidator : AbstractValidator<AllocateAdvanceOrderDTO>
    {

        private readonly DataContext _context;


        public AllocateAdvanceOrderDTOValidator(DataContext context)
        {
            this._context = context;


            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.stAdvanceOrderId)
                .NotEmpty()
                    .Must(CheckAdvanceOrder)
                        .WithMessage("Advance order is not valid");

            RuleFor(p => p)
                .Must(CheckItemsAllocated)
                    .WithMessage("Please select atleast 1 item to be allocated");


            RuleFor(p => p.AllocateAdvanceOrderDetails)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be requested.")
                .Must(list => list.Count > 0) // true
                    .WithMessage("Please add at least one item to be requested.")
                .Must(list => list.Count > 0 && list.Count <= 7)
                    .WithMessage("Only 7 items are allowed per purchase")
                .SetCollectionValidator(new AllocateAdvanceOrderDetailsDTOValidator(context));


        }


        private bool CheckAdvanceOrder(int? id)
        {
            var order = _context.STAdvanceOrder.Where(p => p.Id == id && p.RequestStatus == RequestStatusEnum.Approved).FirstOrDefault();

            if(order == null)
            {
                return false;
            }

            return true;
        }


        private bool CheckItemsAllocated(AllocateAdvanceOrderDTO model)
        {
            var count = model.AllocateAdvanceOrderDetails.Where(p => p.ItemId.HasValue).Count();
            
            return count > 0;
        }

  
    }
}

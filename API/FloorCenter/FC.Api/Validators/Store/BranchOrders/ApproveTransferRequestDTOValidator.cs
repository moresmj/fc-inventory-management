using System.Linq;
using FC.Api.DTOs.Store.BranchOrders;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FluentValidation;

namespace FC.Api.Validators.Store.BranchOrders
{
    public class ApproveTransferRequestDTOValidator : AbstractValidator<ApproveTransferRequestDTO>
    {

        private readonly DataContext _context;

        public ApproveTransferRequestDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid")
                    .WithName("Id");


            RuleFor(p => p.TransferredItems)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be approved.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be approved.")
                .SetCollectionValidator(new ApproveTransferRequestItemsValidator(context));

            RuleFor(p => p)
                .Must(IdShouldMatchListSTOrderId)
                    .WithMessage("Invalid item(s) found on the list.")
                    .WithName("Id & ShowroomDeliveries.STOrderId");

        }

        private bool ValidRecord(ApproveTransferRequestDTO model)
        {
            if (model.Id.HasValue)
            {
                var record = _context.STOrders
                               .Where(p => p.Id == model.Id && p.RequestStatus == RequestStatusEnum.Pending)
                               .FirstOrDefault();

                return record.Id == model.Id;
            }

            return true;
        }

        private bool IdShouldMatchListSTOrderId(ApproveTransferRequestDTO model)
        {
            if (model.TransferredItems != null && model.TransferredItems.Count > 0)
            {
                return (model.TransferredItems.Where(p => p.STOrderId != model.Id).Count() == 0);
            }

            return true;
        }


    }

}

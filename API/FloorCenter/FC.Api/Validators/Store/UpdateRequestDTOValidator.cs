using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class UpdateRequestDTOValidator : AbstractValidator<UpdateRequestDTO>
    {

        private readonly DataContext _context;

        public UpdateRequestDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.Id)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid");


            RuleFor(p => p.OrderedItems)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be approved.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be approved.")
                .SetCollectionValidator(new UpdateRequestOrderedItemsDTOValidator(context));

            RuleFor(p => p)
                .Must(IdShouldMatchListSTOrderId)
                    .WithMessage("Invalid item(s) found on the list.")
                    .WithName("Id & OrderedItems.STOrderId");
        }


        /// <summary>
        /// This is used to check if record is still valid
        /// RequestStatus should be Pending
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>False if not valid, otherwise True</returns>
        private bool ValidRecord(int id)
        {
            var count = this._context.STOrders.Where(x => x.Id == id 
                                                          && x.OrderType != OrderTypeEnum.InterbranchOrIntercompanyOrder 
                                                          && x.RequestStatus == RequestStatusEnum.Pending).Count();
            if (count != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IdShouldMatchListSTOrderId(UpdateRequestDTO model)
        {
            if (model.OrderedItems != null && model.OrderedItems.Count > 0)
            {
                return (model.OrderedItems.Where(p => p.STOrderId != model.Id).Count() == 0);
            }

            return true;
        }

    }
}

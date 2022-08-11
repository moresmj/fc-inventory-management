using FC.Api.DTOs.Store.Deliveries;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Store.Deliveries
{
    public class UpdateStoreReturnsDeliveryDTOValidator : AbstractValidator<UpdateStoreReturnsDeliveryDTO>
    {

        private readonly DataContext _context;

        public UpdateStoreReturnsDeliveryDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid")
                    .WithName("Id");


            RuleFor(p => p.ApprovedDeliveryDate)
                .NotEmpty();


            RuleFor(p => p.DriverName)
                .NotEmpty()
                .MaximumLength(100);


            RuleFor(p => p.PlateNumber)
                .NotEmpty()
                .MaximumLength(10);
        }

        private bool ValidRecord(UpdateStoreReturnsDeliveryDTO model)
        {
            //var count = this._context.STReturns
            //                         .Where(x => x.Id == model.Id
            //                                     && x.ApprovedDeliveryDate.HasValue == false)
            //                         .Count();
            if (!model.DeliveryStatus.HasValue || model.DeliveryStatus == DeliveryStatusEnum.Delivered)
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


using FC.Api.DTOs.Warehouse.Delivery;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Linq;

namespace FC.Api.Validators.Warehouse.Delivery
{
    public class UpdateReturnsDeliveryDTOValidator : AbstractValidator<UpdateReturnsDeliveryDTO>
    {

        private readonly DataContext _context;

        public UpdateReturnsDeliveryDTOValidator(DataContext context)
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
                .MaximumLength(100)
                .NotEmpty();


            RuleFor(p => p.PlateNumber)
                .MaximumLength(10)
                .NotEmpty();
        }

        private bool ValidRecord(UpdateReturnsDeliveryDTO model)
        {
            //var count = this._context.WHDeliveries
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

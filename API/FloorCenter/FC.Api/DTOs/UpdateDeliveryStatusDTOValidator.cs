using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.DTOs
{
    public class UpdateDeliveryStatusDTOValidator : AbstractValidator<UpdateDeliveryStatusDTO>
    {

        private readonly DataContext _context;

        public UpdateDeliveryStatusDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid")
                    .WithName("Id");


            RuleFor(p => p.IsDelivered)
                .NotEmpty();

        }

        private bool ValidRecord(UpdateDeliveryStatusDTO model)
        {
            var count = this._context.STDeliveries.Where(x => x.Id == model.Id && x.Delivered == DeliveryStatusEnum.Waiting).Count();
            if (count != 1)
            {
                return false;
            }
            return true;

        }

    }
}

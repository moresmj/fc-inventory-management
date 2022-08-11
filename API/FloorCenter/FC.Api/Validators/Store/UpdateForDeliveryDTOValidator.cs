using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class UpdateForDeliveryDTOValidator : AbstractValidator<UpdateForDeliveryDTO>
    {

        private readonly DataContext _context;

        public UpdateForDeliveryDTOValidator(DataContext context)
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

        private bool ValidRecord(UpdateForDeliveryDTO model)
        {
            var count = this._context.STDeliveries.Where(x => x.STOrderId == model.STOrderId && x.Id == model.Id).Count();
            if (count != 1)
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

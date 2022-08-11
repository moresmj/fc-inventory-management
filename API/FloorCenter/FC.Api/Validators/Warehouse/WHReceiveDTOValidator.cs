using FC.Api.DTOs.Warehouse;
using FC.Api.Helpers;
using FC.Api.Validators.User;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Warehouse
{
    public class WHReceiveDTOValidator : AbstractValidator<WHReceiveDTO>
    {

        private readonly DataContext _context;

        public WHReceiveDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            //  Validate PONumber
            RuleFor(p => p.PONumber)
                    .NotEmpty()
                    .MaximumLength(50);


            //  Validate PODate
            RuleFor(p => p.PODate)
                    .NotEmpty();


            //  Validate DRNumber
            RuleFor(p => p.DRNumber)
                    .NotEmpty()
                    .MaximumLength(50);


            //  Validate DRDate
            RuleFor(p => p.DRDate)
                    .NotEmpty();


            //  Validate PONumber & DRNumber
            RuleFor(x => x)
                .Must(PONumberDRNumberNotYetRegistered)
                    .WithMessage("Record is already registered")
                    .WithName("PONumber and DRNumber");


            //  Validate ReceivedDate
            RuleFor(p => p.ReceivedDate)
                    .NotEmpty();


            //  Validate CheckedBy
            RuleFor(p => p.UserId)
                    .NotEmpty()
                    .Must(new UserDTOValidator(context, false).IdValid)
                        .WithMessage("{PropertyName} is not valid");


            //  Validate Items
            RuleFor(p => p.ReceivedItems)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be saved.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be saved.")
                .SetCollectionValidator(new WHReceiveDetailDTOValidator(context));

        }


        private bool PONumberDRNumberNotYetRegistered(WHReceiveDTO model)
        {
            if (!string.IsNullOrEmpty(model.PONumber) && !string.IsNullOrEmpty(model.DRNumber))
            {
                var count = this._context.WHReceives.Where(x => x.PONumber.ToLower() == model.PONumber.ToLower() && x.DRNumber.ToLower() == model.DRNumber.ToLower()).Count();

                if (count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

    }
}

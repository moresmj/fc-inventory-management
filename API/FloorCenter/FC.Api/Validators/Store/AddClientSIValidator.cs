using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Store
{
    public class AddClientSIValidator : AbstractValidator<AddClientSIDTO>
    {
        private readonly DataContext _context;



        public AddClientSIValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;



            //Validate SI Number if it is still available
            RuleFor(p => p)
               .NotEmpty()
                .Must(ClientSINotYetRegistered)
                  .WithMessage("Client SI Number  already registered")
                  .WithName("DRNumber and DeliveryDate")
                  .Must(d => d.ClientSINumber.ToString().Length < 50)
                  .WithMessage("The length of 'Client SI Number' must be 50 characters or fewer");

        }


        private bool ClientSINotYetRegistered(AddClientSIDTO dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.ClientSINumber))
            {
                var TRNumber = _context.STOrders.Where(x => x.Id == dto.Id).Select(p => p.TRNumber).FirstOrDefault();
                var obj = _context.STOrders.Where(x => x.ClientSINumber == dto.ClientSINumber && x.TRNumber != TRNumber);
                return (obj.Count() == 0);

            }
            return false;
        }


    }
}

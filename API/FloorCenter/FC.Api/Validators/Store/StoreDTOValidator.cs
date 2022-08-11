using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Validators.Company;
using FC.Api.Validators.Warehouse;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class StoreDTOValidator : AbstractValidator<StoreDTO>
    {

        private readonly DataContext _context;

        public StoreDTOValidator(DataContext context, bool runDefaultValidations = true)
        {

            this._context = context;

            if (runDefaultValidations)
            {
                CascadeMode = CascadeMode.StopOnFirstFailure;

                //  Validate Id
                RuleFor(p => p.Id)
                    .Must(RecordExist)
                        .When(p => p.Id != 0)
                        .WithMessage("Record is not valid");

                //  Validate Store Code
                RuleFor(p => p.Code)
                    .NotEmpty()
                       .MaximumLength(20)
                        .When(r => r.Id == 0)
                    .Must(CodeStillAvailable)
                        .When(r => r.Id == 0)
                        .WithMessage("Store Code '{PropertyValue}' is already registered");


                //  Validate Store Name
                RuleFor(p => p.Name)
                    .MaximumLength(255)
                    .NotEmpty();


                //  Validate Company ID
                RuleFor(p => p.CompanyId)
                    .NotEmpty()
                    .Must(new CompanyDTOValidator(context, false).IdValid)
                        .WithMessage("{PropertyName} is not valid");


                //  Validate Contact Number
                RuleFor(p => p.ContactNumber)
                    .Matches("[0-9]")
                        .When(p => p.ContactNumber != string.Empty);


                //  Validate Warehouse ID
                RuleFor(p => p.WarehouseId)
                    .NotEmpty()
                    .Must(new WarehouseDTOValidator(context, false).IdValid)
                        .WithMessage("{PropertyName} is not valid");
            }
        }

        /// <summary>
        /// This function is used to validate if selected Store Code is already registered.
        /// </summary>
        /// <param name="code">Store Code to be checked if already registered.</param>
        /// <returns>False if already registered, otherwise True.</returns>
        private bool CodeStillAvailable(string code)
        {

            var count = _context.Stores.Where(x => x.Code.ToLower() == code.ToLower()).Count();

            if (count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }


        /// <summary>
        /// This function is used to validate if selected Store ID is registered.
        /// </summary>
        /// <param name="id">ID to be checked if registered.</param>
        /// <returns>False if not valid means not registered, otherwise True</returns>
        public bool IdValid(int? id)
        {
            var count = this._context.Stores.Where(x => x.Id == id).Count();

            if (count != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool RecordExist(int id)
        {
            var count = this._context.Stores.Where(x => x.Id == id).Count();
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

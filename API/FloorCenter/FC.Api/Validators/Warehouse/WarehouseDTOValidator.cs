using FC.Api.DTOs.Warehouse;
using FC.Api.Helpers;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Warehouse
{
    public class WarehouseDTOValidator : AbstractValidator<WarehouseDTO>
    {

        private readonly DataContext _context;

        public WarehouseDTOValidator(DataContext context, bool runDefaultValidations = true)
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


                //  Validate Warehouse Code
                RuleFor(p => p.Code)
                    .NotEmpty()
                        .When(r => r.Id == 0)
                    .Must(CodeStillAvailable)
                        .When(r => r.Id == 0)
                        .WithMessage("{PropertyName} '{PropertyValue}' is already registered");


                //  Validate Warehouse Name
                RuleFor(p => p.Name)
                    .NotEmpty();


                //  Validate Contact Number
                RuleFor(p => p.ContactNumber)
                    .Matches("[0-9]")
                        .When(p => p.ContactNumber != string.Empty);
            }

        }

        /// <summary>
        /// This function is used to validate if selected Warehouse Code is already registered.
        /// </summary>
        /// <param name="code">Warehouse Code to be checked if already registered.</param>
        /// <returns>False if already registered, otherwise True.</returns>
        private bool CodeStillAvailable(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var count = _context.Warehouses.Where(x => x.Code.ToLower() == code.ToLower()).Count();

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


        /// <summary>
        /// This function is used to validate if selected Warehouse ID is registered.
        /// </summary>
        /// <param name="id">ID to be checked if registered.</param>
        /// <returns>False if not valid means not registered, otherwise True</returns>
        public bool IdValid(int? id)
        {
            if (id.HasValue)
            {
                var count = this._context.Warehouses.Where(x => x.Id == id).Count();
                if (count != 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }

            return true;
        }

        private bool RecordExist(int id)
        {
            var count = this._context.Warehouses.Where(x => x.Id == id).Count();
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

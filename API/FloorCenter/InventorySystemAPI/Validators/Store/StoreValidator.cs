using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Validators.Company;
using InventorySystemAPI.Validators.Warehouse;
using System.Linq;

namespace InventorySystemAPI.Validators.Store
{
    public class StoreValidator : AbstractValidator<Models.Store.Store>
    {
        private readonly FloorCenterContext _context;

        public StoreValidator(FloorCenterContext context, bool runValidations = true)
        {
            this._context = context;

            if (runValidations)
            {
                CascadeMode = CascadeMode.StopOnFirstFailure;

                //  Validate Store Code
                RuleFor(p => p.Code)
                    .NotEmpty()
                        .When(r => r.Id == 0)
                    .Must(CodeStillAvailable)
                        .When(r => r.Id == 0)
                        .WithMessage("Store Code '{PropertyValue}' is already registered");


                //  Validate Store Name
                RuleFor(p => p.Name)
                    .NotEmpty();


                //  Validate Company ID
                RuleFor(p => p.CompanyId)
                    .NotEmpty()
                    .Must(new CompanyValidator(context, false).IdValid)
                        .WithMessage("{PropertyName} is not valid");


                //  Validate Warehouse ID
                RuleFor(p => p.WarehouseId)
                    .NotEmpty()
                    .Must(new WarehouseValidator(context, false).IdValid)
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
            if (!string.IsNullOrEmpty(code))
            {
                var store = _context.Stores.Where(x => x.Code.ToLower() == code.ToLower()).SingleOrDefault();

                if (store == null)
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
        /// This function is used to validate if selected Store ID is registered.
        /// </summary>
        /// <param name="id">ID to be checked if registered.</param>
        /// <returns>False if not valid means not registered, otherwise True</returns>
        public bool IdValid(int? id)
        {
            if (id.HasValue)
            {
                var store = this._context.Stores.Where(x => x.Id == id).SingleOrDefault();

                if (store == null)
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
        

    }
}

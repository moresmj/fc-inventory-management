using FluentValidation;
using InventorySystemAPI.Context;
using System.Linq;

namespace InventorySystemAPI.Validators.Warehouse
{
    public class WarehouseValidator : AbstractValidator<Models.Warehouse.Warehouse>
    {

        private readonly FloorCenterContext _context;

        public WarehouseValidator(FloorCenterContext context, bool runValidations = true)
        {
            this._context = context;

            if (runValidations)
            {
                CascadeMode = CascadeMode.StopOnFirstFailure;

                //  Validate Warehouse Code
                RuleFor(p => p.Code)                    
                    .NotEmpty()
                        .When(r => r.Id == 0)
                    .Must(CodeStillAvailable)
                        .When(r => r.Id == 0)
                        .WithMessage("Warehouse Code '{PropertyValue}' is already registered");


                //  Validate Warehouse Name
                RuleFor(p => p.Name)
                    .NotEmpty();
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
                var warehouse = _context.Warehouses.Where(x => x.Code.ToLower() == code.ToLower()).SingleOrDefault();

                if (warehouse == null)
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
                var warehouse = this._context.Warehouses.Where(x => x.Id == id).SingleOrDefault();

                if (warehouse == null)
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

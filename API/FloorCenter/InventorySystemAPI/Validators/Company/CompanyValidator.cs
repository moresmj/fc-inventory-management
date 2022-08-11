using FluentValidation;
using InventorySystemAPI.Context;
using System.Linq;

namespace InventorySystemAPI.Validators.Company
{
    public class CompanyValidator : AbstractValidator<Models.Company.Company>
    {

        private readonly FloorCenterContext _context;

        public CompanyValidator(FloorCenterContext context, bool runValidations = true)
        {
            this._context = context;

            if (runValidations)
            {
                //  Validate Store Name
                RuleFor(p => p.Name)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                        .NotEmpty()
                        .Must(NameStillAvailable)
                            .WithMessage("Company Name '{PropertyValue}' is already registered");
            }

        }


        /// <summary>
        /// This function is used to validate if selected Company Name is already registered.
        /// </summary>
        /// <param name="name">Store Name to be checked if already registered.</param>
        /// <returns>False if already registered, otherwise True.</returns>
        private bool NameStillAvailable(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var company = _context.Companies.Where(x => x.Name.ToLower() == name.ToLower()).SingleOrDefault();

                if (company == null)
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
        /// This function is used to validate if selected Company ID is registered.
        /// </summary>
        /// <param name="id">ID to be checked if registered.</param>
        /// <returns>False if not valid means not registered, otherwise True</returns>
        public bool IdValid(int? id)
        {
            if (id.HasValue)
            {
                var company = this._context.Companies.Where(x => x.Id == id).SingleOrDefault();

                if (company == null)
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

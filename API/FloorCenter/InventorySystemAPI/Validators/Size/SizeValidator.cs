using FluentValidation;
using InventorySystemAPI.Context;
using System.Linq;

namespace InventorySystemAPI.Validators.Size
{
    public class SizeValidator : AbstractValidator<Models.Size.Size>
    {

        private readonly FloorCenterContext _context;

        public SizeValidator(FloorCenterContext context, bool runValidations = true)
        {
            this._context = context;

            if (runValidations)
            {

                //  Validate Store Name
                RuleFor(p => p.Name)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                        .NotEmpty()
                        .Must(NameStillAvailable)
                            .WithMessage("Size Name '{PropertyValue}' is already registered");

            }

        }


        /// <summary>
        /// This function is used to validate if selected Size Name is already registered.
        /// </summary>
        /// <param name="name">Size Name to be checked if already registered.</param>
        /// <returns>False if already registered, otherwise True.</returns>
        private bool NameStillAvailable(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var size = _context.Sizes.Where(x => x.Name.ToLower() == name.ToLower()).SingleOrDefault();

                if (size == null)
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
        /// This function is used to validate if selected Size ID is registered.
        /// </summary>
        /// <param name="id">ID to be checked if registered.</param>
        /// <returns>False if not valid means not registered, otherwise True</returns>
        public bool IdValid(int? id)
        {
            if (id.HasValue)
            {
                var size = this._context.Sizes.Where(x => x.Id == id).SingleOrDefault();

                if (size == null)
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

using FC.Api.DTOs.Size;
using FC.Api.Helpers;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Size
{
    public class SizeDTOValidator : AbstractValidator<SizeDTO>
    {

        private readonly DataContext _context;

        public SizeDTOValidator(DataContext context, bool runDefaultValidations = true)
        {
            this._context = context;

            if (runDefaultValidations)
            {

                //  Validate Id
                RuleFor(p => p.Id)
                    .Must(RecordExist)
                        .When(p => p.Id != 0)
                        .WithMessage("Record is not valid");


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
            var count = _context.Sizes.Where(x => x.Name.ToLower() == name.ToLower()).Count();
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
        /// This function is used to validate if selected Size ID is registered.
        /// </summary>
        /// <param name="id">ID to be checked if registered.</param>
        /// <returns>False if not valid means not registered, otherwise True</returns>
        public bool IdValid(int? id)
        {
            if (id.HasValue)
            {
                var count = this._context.Sizes.Where(x => x.Id == id).Count();
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
            var count = this._context.Sizes.Where(x => x.Id == id).Count();
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

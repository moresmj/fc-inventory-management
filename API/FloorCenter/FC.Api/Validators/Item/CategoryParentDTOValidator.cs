using FC.Api.DTOs.Item;
using FC.Api.Helpers;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Item
{
    public class CategoryParentDTOValidator : AbstractValidator<CategoryParentDTO>
    {

        private readonly DataContext _context;

        public CategoryParentDTOValidator(DataContext context, bool runDefaultValidations = true)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            if (runDefaultValidations)
            {

            }
        }

        /// <summary>
        /// This function is used to validate if selected Parent Category ID is registered.
        /// </summary>
        /// <param name="id">ID to be checked if registered.</param>
        /// <returns>False if not valid means not registered, otherwise True</returns>
        public bool IdValid(int? id)
        {
            if (id.HasValue)
            {
                var count = this._context.CategoryGrandChildren.Where(x => x.Id == id).Count();
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

    }
}

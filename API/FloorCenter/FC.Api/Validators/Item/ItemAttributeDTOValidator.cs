using FC.Api.DTOs.Item;
using FC.Api.Helpers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Item
{
    public class ItemAttributeDTOValidator : AbstractValidator<ItemAttributeDTO>
    {

        private readonly DataContext _context;

        public ItemAttributeDTOValidator(DataContext context, bool runDefaultValidations = true)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            if (runDefaultValidations)
            {
                //  Validate Id
                RuleFor(p => p.Id)
                    .Must(RecordExist)
                        .When(p => p.Id != 0)
                        .WithMessage("Record is not valid");


                //  Validate Purpose1
                RuleFor(p => p.Purpose1)
                    .NotEmpty()
                    .IsInEnum();


                //  Validate Purpose2
                RuleFor(p => p.Purpose2)
                    .NotEmpty()
                    .IsInEnum();


                //  Validate Traffic
                RuleFor(p => p.Traffic)
                    .IsInEnum();
            }
        }

        private bool RecordExist(int id)
        {
            var count = this._context.ItemAttributes.Where(x => x.Id == id).Count();
            if (count != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// This function is used to validate if selected Item Attribute ID is registered.
        /// </summary>
        /// <param name="id">ID to be checked if registered.</param>
        /// <returns>False if not valid means not registered, otherwise True</returns>
        public bool IdValid(int? id)
        {
            if (id.HasValue)
            {
                var count = this._context.ItemAttributes.Where(x => x.Id == id).Count();
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

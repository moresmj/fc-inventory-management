using System.Linq;
using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Validators.Item.Attribute;
using InventorySystemAPI.Validators.Size;

namespace InventorySystemAPI.Validators.Item
{
    public class ItemValidator : AbstractValidator<Models.Item.Item>
    {

        private readonly FloorCenterContext _context;

        public ItemValidator(FloorCenterContext context, bool runDefaultValidations = true)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            if (runDefaultValidations)
            {
                //  Validate Code
                RuleFor(p => p.Code)
                    .NotEmpty();


                //  Validate Serial Number
                RuleFor(x => x)
                    .NotEmpty()
                    .Must(SerialNumberStillAvailable)
                     .WithMessage("Selected Serial Number is already registered");
                //.WithMessage("{PropertyName} '{PropertyValue}' is already registered");


                //  Validate Name
                RuleFor(p => p.Name)
                    .NotEmpty();


                //  Validate Size ID
                RuleFor(p => p.SizeId)
                    .NotEmpty()
                    .Must(new SizeValidator(context, false).IdValid)
                        .WithMessage("{PropertyName} is not valid");


                //  Validate Tonality
                RuleFor(p => p.Tonality)
                    .NotEmpty();


                //  Validate Code & Tonality
                RuleFor(x => x)
                    .Must(CodeAndTonalityStillAvailable)
                        .WithMessage("Item is already registered");


                //  Validate Item Attribute
                RuleFor(x => x.ItemAttribute)
                    .SetValidator(new ItemAttributeValidator(context));
            }

        }


        /// <summary>
        /// This function is used to validate if an item with Code & Tonality is already registered.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>False if already registered, otherwise True</returns>
        private bool CodeAndTonalityStillAvailable(Models.Item.Item model)
        {
            if(!string.IsNullOrEmpty(model.Code) && !string.IsNullOrEmpty(model.Code))
            {
                var obj = this._context.Items.Where(x => x.Code.ToLower() == model.Code.ToLower() && x.Tonality.ToLower() == model.Tonality.ToLower() && x.Id != model.Id).SingleOrDefault();

                if(obj == null)
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
        /// This function is used to validate if Serial Number is already registered.
        /// </summary>
        /// <param name="serialNumber">Serial Number to be checked if already registered.</param>
        /// <returns>False if already registered, otherwise True</returns>
        private bool SerialNumberStillAvailable(Models.Item.Item model)
        {
            var obj = this._context.Items.Where(x => x.SerialNumber == model.SerialNumber && x.Id != model.Id).Count();

            if (obj >= 1)
            {
                return false;
            }
            return true;
           
        }


        /// <summary>
        /// This function is used to validate if selected Item ID is registered.
        /// </summary>
        /// <param name="id">ID to be checked if registered.</param>
        /// <returns>False if not valid means not registered, otherwise True</returns>
        public bool IdValid(int? id)
        {
            if (id.HasValue)
            {
                var count = this._context.Items.Where(x => x.Id == id).Count();
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

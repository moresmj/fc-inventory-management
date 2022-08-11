using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Item.Attribute;

namespace InventorySystemAPI.Validators.Item.Attribute
{
    public class ItemAttributeValidator : AbstractValidator<ItemAttribute>
    {

        private readonly FloorCenterContext _context;

        public ItemAttributeValidator(FloorCenterContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.Purpose1)
                .NotEmpty()
                .IsInEnum();

            RuleFor(p => p.Purpose2)
                .NotEmpty()
                .IsInEnum();

        }

    }
}

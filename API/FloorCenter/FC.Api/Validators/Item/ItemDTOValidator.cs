using FC.Api.DTOs.Item;
using FC.Api.Helpers;
using FC.Api.Validators.Size;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Item
{
    public class ItemDTOValidator : AbstractValidator<ItemDTO>
    {

        private readonly DataContext _context;

        public ItemDTOValidator(DataContext context, bool runDefaultValidations = true)
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


                //  Validate Serial Number
                RuleFor(p => p.SerialNumber)
                    .NotEmpty();


                //  Validate Code
                RuleFor(p => p.Code)
                    .NotEmpty();


                //  Validate Serial Number
                RuleFor(x => x)
                    .NotEmpty()
                    .Must(SerialNumberStillAvailable)
                        .WithMessage("Selected Serial Number is already registered")
                        .WithName("SerialNumber");


                //  Validate Name
                RuleFor(p => p.Name)
                    .NotEmpty();


                //  Validate SRP
                RuleFor(p => p.SRP)
                    .NotEmpty()
                    .GreaterThan(0);


                //  Validate Size ID
                RuleFor(p => p.SizeId)
                    .NotEmpty()
                    .Must(new SizeDTOValidator(context, false).IdValid)
                        .WithMessage("{PropertyName} is not valid");

                
                //  Validate Category Parent Id
                RuleFor(p => p.CategoryParentId)
                    .Must(new CategoryParentDTOValidator(context, false).IdValid)
                        .When(p => p.CategoryParentId.HasValue)
                        .WithMessage("Category is not valid");

                
                //  Validate Category Child Id
                RuleFor(p => p.CategoryChildId)
                    .Must(new CategoryChildDTOValidator(context, false).IdValid)
                        .When(p => p.CategoryChildId.HasValue)
                        .WithMessage("Sub Category is not valid");

                //  Validate Parent & Child Category
                RuleFor(p => p)
                    .Must(ChildCategoryIsRelatedWithParentCategory)
                        .WithMessage("Sub Category is not valid")
                        .WithName("CategoryChildId");



                //  Validate Category GrandChild Id
                RuleFor(p => p.CategoryGrandChildId)
                    .Must(new CategoryGrandChildDTOValidator(context, false).IdValid)
                        .When(p => p.CategoryGrandChildId.HasValue)
                        .WithMessage("Sub-sub Category is not valid");

                //  Validate Child & Grandchild Category
                RuleFor(p => p)
                    .Must(GrandChildCategoryIsRelatedWithChildCategory)
                        .WithMessage("Sub-sub Category is not valid")
                        .WithName("CategoryGrandChildId");



                //  Validate Tonality
                RuleFor(p => p.Tonality)
                    .NotEmpty();


                //  Validate Code & Tonality
                RuleFor(x => x)
                    .Must(CodeAndTonalityStillAvailable)
                        .WithMessage("Item is already registered");

            }

        }

        private bool RecordExist(int id)
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

        private bool CodeAndTonalityStillAvailable(ItemDTO model)
        {
            if (!string.IsNullOrEmpty(model.Code) && !string.IsNullOrEmpty(model.Code))
            {
                var obj = this._context.Items.Where(x => x.Code.ToLower() == model.Code.ToLower() && x.Tonality.ToLower() == model.Tonality.ToLower() && x.Id != model.Id).SingleOrDefault();

                if (obj == null)
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


        private bool SerialNumberStillAvailable(ItemDTO model)
        {

            if (!string.IsNullOrEmpty(model.SerialNumber))
            {
                var obj = this._context.Items.Where(x => x.SerialNumber == model.SerialNumber && x.Id != model.Id).Count();

                if (obj >= 1)
                {
                    return false;
                }
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


        private bool ChildCategoryIsRelatedWithParentCategory(ItemDTO model)
        {
            if(model.CategoryParentId.HasValue && model.CategoryChildId.HasValue)
            {
                var count = this._context.CategoryChildren.Where(x => x.Id == model.CategoryChildId && x.CategoryParentId == model.CategoryParentId).Count();
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

        private bool GrandChildCategoryIsRelatedWithChildCategory(ItemDTO model)
        {
            if (model.CategoryChildId.HasValue && model.CategoryGrandChildId.HasValue)
            {
                var count = this._context.CategoryGrandChildren.Where(x => x.Id == model.CategoryGrandChildId && x.CategoryChildId == model.CategoryChildId).Count();
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

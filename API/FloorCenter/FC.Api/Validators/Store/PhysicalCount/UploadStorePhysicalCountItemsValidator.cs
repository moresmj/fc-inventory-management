using FC.Api.DTOs.Store.PhysicalCount;
using FC.Api.Helpers;
using FluentValidation;
using System.Linq;
using System.Text.RegularExpressions;

namespace FC.Api.Validators.Store.PhysicalCount
{
    public class UploadStorePhysicalCountItemsValidator : AbstractValidator<UploadStorePhysicalCountItems>
    {

        private readonly DataContext _context;

        public UploadStorePhysicalCountItemsValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ItemId
            RuleFor(p => p)
                .Must(ValidItem)
                    .WithMessage("Record is not valid")
                    .WithName("ItemId");

            //  Validate PhysicalCount
            RuleFor(p => p.PhysicalCount)
                .NotEmpty();

            RuleFor(p => p)
                .Must(CheckForDecimal)
                .WithMessage("Record must only contain whole numbers")
                .WithName("Physical Count");

            //  Validate SystemCount
            //RuleFor(p => p.SystemCount)
            ///    .NotEmpty();

            //RuleFor(p => p)
            //    .Must(SystemAndPhysicalCountMustBeGreaterThanZero)
            //        .WithMessage("System Count & Physical Count not valid")
            //        .WithName("SystemCount & PhysicalCount");

        }

        //private bool ValidItem(UploadStorePhysicalCountItems model)
        //{
        //    if(model.StoreId.HasValue)
        //    {
        //        var count = _context.STStocks
        //                            .Where(p => p.StoreId == model.StoreId
        //                                        && p.ItemId == model.ItemId)
        //                            .Count();

        //        if(count == 0)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}


        private bool ValidItem(UploadStorePhysicalCountItems model)
        {
            if (model.StoreId.HasValue)
            {
                var count = _context.Items
                                    .Where(p => p.Id == model.ItemId)
                                    .Count();

                if (count == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private bool SystemAndPhysicalCountMustBeGreaterThanZero(UploadStorePhysicalCountItems model)
        {
            if(model.PhysicalCount <= 0 && model.SystemCount <= 0)
            {
                return false;
            }

            return true;
        }

        private bool CheckForDecimal(UploadStorePhysicalCountItems model)
        {
            Regex regex = new Regex(@"^[0-9]+$");
            if (model.PhysicalCount.HasValue)
            {
                return regex.IsMatch(model.PhysicalCount.ToString());
            }
            return false;
        }
    }
}

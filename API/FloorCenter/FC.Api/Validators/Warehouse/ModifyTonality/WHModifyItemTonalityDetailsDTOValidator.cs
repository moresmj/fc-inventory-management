using FC.Api.DTOs.Warehouse;
using FC.Api.DTOs.Warehouse.ModifyTonality;
using FC.Api.Helpers;
using FC.Api.Validators.Item;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Warehouse.ModifyTonality
{
    public class WHModifyItemTonalityDetailsDTOValidator : AbstractValidator<WHModifyItemTonalityDetailsDTO>
    {

        private readonly DataContext _context;

        public WHModifyItemTonalityDetailsDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ItemId
            RuleFor(p => p.ItemId)
                    .Must(new ItemDTOValidator(context, false).IdValid)
                        .WithMessage("Item is not valid");

            //  Validate Quantity
            RuleFor(p => p)
                .Must(orderQtyNotMoreOnStockQty)
                    .WithMessage("The selected tonality has insufficient quantity");

        }


        private bool orderQtyNotMoreOnStockQty(WHModifyItemTonalityDetailsDTO model)
        {
            if(model.ItemId.HasValue)
            {
                var avail = _context.WHStockSummary
                                   .Where(p => p.WarehouseId == model.WarehouseId && p.ItemId == model.ItemId)
                                       .OrderByDescending(p => p.Id)
                                           .Select(p => p.Available)
                                               .FirstOrDefault();
                if (model.requestedQty > avail)
                {
                    return false;
                }
            }
           
            return true;
        }

    }
}

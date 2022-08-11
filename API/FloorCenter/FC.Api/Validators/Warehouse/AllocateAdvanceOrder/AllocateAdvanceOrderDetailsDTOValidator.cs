using FC.Api.DTOs.Warehouse.AllocateAdvanceOrder;
using FC.Api.Helpers;
using FC.Api.Validators.Item;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Warehouse.AllocateAdvanceOrder
{
    public class AllocateAdvanceOrderDetailsDTOValidator : AbstractValidator<AllocateAdvanceOrderDetailsDTO>
    {

        private readonly DataContext _context;


        public AllocateAdvanceOrderDetailsDTOValidator(DataContext context)
        {
            this._context = context;


            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.ItemId)
                    .Must(new ItemDTOValidator(context, false).IdValid)
                        .WithMessage("Item is not valid")
                            .When(p => p.ItemId.HasValue);



            RuleFor(p => p.AllocatedQuantity)
                .NotEmpty()
                    .When(p => p.ItemId.HasValue)
                .GreaterThan(0)
                    .When(p => p.ItemId.HasValue);


            RuleFor(p => p)
                .Must(AllocatedQtyNotMoreThanReservedQty)
                .WithMessage("Allocated quantity must not greater than the Reserved quantity");
                
           RuleFor(p => p)
                .Must(CheckAllocatedItems)
                .WithMessage("Allocated quantity must not greater than the remaining quantity to be allocated");


        }

        private bool AllocatedQtyNotMoreThanReservedQty(AllocateAdvanceOrderDetailsDTO model)
        {
            if (model.ItemId.HasValue)
            {
                //var reservedQty = _context.WHStockSummary
                //                   .Where(p => p.WarehouseId == model.WarehouseId && p.ItemId == model.ItemId)
                //                       .OrderByDescending(p => p.Id)
                //                           .Select(p => p.Reserved)
                //                               .FirstOrDefault();

                var reservedQty = _context.CustomQuantity.FromSql("SELECT [dbo].[WHGetReserved]({0}, {1}) as Total", model.WarehouseId, model.ItemId).Sum(p => p.Total);

                if (model.AllocatedQuantity > reservedQty)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckAllocatedItems(AllocateAdvanceOrderDetailsDTO model)
        {
            if(model.ItemId.HasValue && model.STAdvanceOrderId.HasValue)
            {

                var totalRequestedQty = _context.STAdvanceOrderDetails.Where
                                                                       (
                                                                            p => p.STAdvanceOrderId == model.STAdvanceOrderId
                                                                            && ((model.isCustom == true) 
                                                                                ? (p.Code == model.Code && p.sizeId == model.SizeId)
                                                                                : (p.ItemId == model.ItemId))
                                                                       )
                                                                       .Sum(p => p.ApprovedQuantity);

                //var totalAllocatedQty = _context.WHAllocateAdvanceOrder.Where
                //                        (
                //                            p => p.StAdvanceOrderId == model.STAdvanceOrderId
                //                            && p.AllocateAdvanceOrderDetails.Any(x =>
                //                                                                ((model.isCustom == true)
                //                                                                ? (x.Code == model.Code && x.SizeId == model.SizeId)
                //                                                                : (x.ItemId == model.ItemId))
                //                                        )
                //                        )
                //                        .Sum(p => p.AllocateAdvanceOrderDetails.Sum(x => x.AllocatedQuantity));

                var allocatedDetails = _context.WHAllocateAdvanceOrder
                                                .Include(p => p.AllocateAdvanceOrderDetails)
                                                    .Where(p => p.StAdvanceOrderId == model.STAdvanceOrderId)
                                                        .SelectMany(p => p.AllocateAdvanceOrderDetails);

                var totalAllocatedQty = allocatedDetails.Where(p => ((model.isCustom == true)
                                                                        ? (p.Code == model.Code && p.SizeId == model.SizeId)
                                                                        : (p.ItemId == model.ItemId))
                                                               ).Select(x => x.AllocatedQuantity)
                                                                    .DefaultIfEmpty(0)
                                                                        .Sum();


                totalAllocatedQty = totalAllocatedQty + model.AllocatedQuantity;

                if (totalAllocatedQty > totalRequestedQty)
                {
                    return false;
                }

            }

            return true;
        }
    }
}

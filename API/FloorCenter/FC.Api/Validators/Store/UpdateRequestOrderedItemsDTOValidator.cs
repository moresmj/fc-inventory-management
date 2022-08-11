using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Services.Warehouses;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class UpdateRequestOrderedItemsDTOValidator : AbstractValidator<STOrderDetailDTO>
    {

        private readonly DataContext _context;

        public UpdateRequestOrderedItemsDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate ItemId
            RuleFor(p => p)
                    .NotEmpty()
                    .Must(ValidItem)
                        .WithMessage("Item is not valid");


            //  Validate Approved Quantity
            RuleFor(p => p.ApprovedQuantity)
                    .NotEmpty()
                    .GreaterThanOrEqualTo(0);

            RuleFor(p => p)
                .Must(ApprovedQuantityNotMoreThanRequestedQuantity)
                    .WithMessage("Approved Quantity should not be more than the Requested Quantity");
                //.Must(ShouldNotBeMoreThanTheAvailableQuantity)
                //    .WithMessage("Approved Quantity should not be more than the available Quantity");

        }

        private bool ValidItem(STOrderDetailDTO model)
        {
            var item = this._context.STOrderDetails.Where(x => x.STOrderId == model.STOrderId && x.Id == model.Id && x.ItemId == model.ItemId).Count();
            if (item != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool ApprovedQuantityNotMoreThanRequestedQuantity(STOrderDetailDTO model)
        {
            var item = this._context.STOrderDetails.Where(x => x.STOrderId == model.STOrderId && x.Id == model.Id && x.ItemId == model.ItemId).FirstOrDefault();
            if (item != null)
            {
                if (model.ApprovedQuantity > item.RequestedQuantity)
                {
                    return false;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ShouldNotBeMoreThanTheAvailableQuantity(STOrderDetailDTO model)
        {
            var order = _context.STOrders.Where(p => p.Id == model.STOrderId).FirstOrDefault();

            
            
            if (order != null)
            {
                if (!_context.Warehouses.Where(P=>P.Id == order.WarehouseId).SingleOrDefault().Vendor)
                {
                    var service = new WHStockService(_context);
                    service.whStock = _context.WHStocks;
                    service.stOrder = _context.STOrders;

                    //added false parameter because of the new order for release behaviour
                    if (model.ApprovedQuantity > service.GetItemAvailableQuantity(model.ItemId, order.WarehouseId,false))
                    {
                        return false;
                    }
                }
            }

            return true;

        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using FC.Api.DTOs.Warehouse.Receive_Items;
using FC.Api.Helpers;
using FC.Api.Services.Warehouses.Receive_Items;
using FluentValidation;

namespace FC.Api.Validators.Warehouse.Receive_Items
{
    public class ReceiveReturnsDTOValidator : AbstractValidator<ReceiveReturnsDTO>
    {
        private DataContext _context;

        public ReceiveReturnsDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid")
                    .WithName("Id & WarehouseId");


            //  Validate Items
            RuleFor(p => p.DeliveredItems)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be received.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be received.")
                .Must(ShouldHaveAtLeastOneItemToBeReceived)
                    .WithMessage("Please add at least one item to be received.")
                .SetCollectionValidator(new ReceiveReturnsItemsValidator(context));

            RuleFor(p => p)
                .Must(IdShouldMatchListWHDeliveryId)
                    .WithMessage("Invalid item(s) found on the list.")
                    .WithName("Id & DeliveredItems.WHDeliveryId");

        }

        private bool ValidRecord(ReceiveReturnsDTO model)
        {
            if(model.WarehouseId.HasValue)
            {
                var service = new ReturnsForReceivingService(_context);
                if(service.GetReturnsForReceivingByIdAndWarehouseId(model.Id, model.WarehouseId) == null)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ShouldHaveAtLeastOneItemToBeReceived(ICollection<ReceiveReturnsItems> deliveries)
        {
            if (deliveries != null && deliveries.Count > 0)
            {
                var goodQty = Convert.ToInt32(deliveries.Sum(p => p.GoodQuantity));
                var brokenQty = Convert.ToInt32(deliveries.Sum(p => p.BrokenQuantity));

                return ((goodQty + brokenQty) > 0);
            }

            return true;
        }

        private bool IdShouldMatchListWHDeliveryId(ReceiveReturnsDTO model)
        {
            if (model.DeliveredItems != null && model.DeliveredItems.Count > 0)
            {
                return (model.DeliveredItems.Where(p => p.WHDeliveryId != model.Id).Count() == 0);
            }

            return true;
        }

    }
}

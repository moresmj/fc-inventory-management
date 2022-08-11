using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class AddShowroomDeliveryItemsValidator : AbstractValidator<AddShowroomDeliveryItems>
    {

        private readonly DataContext _context;

        public AddShowroomDeliveryItemsValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Quantity
            RuleFor(p => p.STOrderDetailId)
                    .NotEmpty();


            //  Validate Item
            RuleFor(p => p)
                    .NotEmpty()
                    .Must(ValidItem)
                        .WithMessage("Item is not valid");


            //  Validate Quantity
            RuleFor(p => p.Quantity)
                    .NotEmpty()
                    .GreaterThanOrEqualTo(0);

            RuleFor(p => p)
                    .Must(QuantityNotMoreThanApprovedQuantity)
                        .WithMessage("Quantity is not valid");
        }


        private bool ValidItem(AddShowroomDeliveryItems model)
        {
            var item = this._context.STOrderDetails.Where(x => x.Id == model.STOrderDetailId 
                                                               && x.STOrderId == model.STOrderId
                                                               && x.ItemId == model.ItemId).Count();
            if (item != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool QuantityNotMoreThanApprovedQuantity(AddShowroomDeliveryItems model)
        {
            var item = this._context.STOrderDetails.Where(x => x.Id == model.STOrderDetailId
                                                               && x.STOrderId == model.STOrderId
                                                               && x.ItemId == model.ItemId).FirstOrDefault();

            //  Get all requested quantity for delivery
            var requestedQuantity = Convert.ToInt32(this._context.STShowroomDeliveries.Where
                                                        (
                                                            x => x.STOrderDetailId == model.STOrderDetailId
                                                            && x.ItemId == model.ItemId
                                                            && (x.DeliveryStatus == DeliveryStatusEnum.Pending || x.DeliveryStatus == DeliveryStatusEnum.Waiting)
                                                        )
                                                        .Select(x => x.Quantity)
                                                        .Sum()
                                                    );

            //  Get all delivered quantity
            var deliveredQuantity = Convert.ToInt32(this._context.STShowroomDeliveries.Where
                                                        (
                                                            x => x.STOrderDetailId == model.STOrderDetailId
                                                            && x.ItemId == model.ItemId
                                                            && x.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                        )
                                                        .Sum(x => x.DeliveredQuantity)
                                                    ) + model.Quantity;



            if (item != null)
            {
                if ((requestedQuantity + deliveredQuantity) > item.ApprovedQuantity)
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

    }
}

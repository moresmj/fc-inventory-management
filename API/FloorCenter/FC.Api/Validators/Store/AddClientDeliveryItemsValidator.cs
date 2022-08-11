using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class AddClientDeliveryItemsValidator : AbstractValidator<AddClientDeliveryItems>
    {

        private readonly DataContext _context;

        public AddClientDeliveryItemsValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Quantity
            RuleFor(p => p.STOrderDetailId)
                    .NotEmpty()
                        .WithMessage("'Id' should not be empty.");


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


        private bool ValidItem(AddClientDeliveryItems model)
        {
            var item = this._context.STOrderDetails.Where(x => x.Id == model.STOrderDetailId && x.STOrderId == model.STOrderId && x.ItemId == model.ItemId).Count();
            if (item != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool QuantityNotMoreThanApprovedQuantity(AddClientDeliveryItems model)
        {
            var item = this._context.STOrderDetails.Where(x => x.Id == model.STOrderDetailId && x.STOrderId == model.STOrderId && x.ItemId == model.ItemId).FirstOrDefault();
            //remove for ticket #418
            ////  Get all requested quantity for delivery
            //var requestedQuantity = Convert.ToInt32(this._context.STClientDeliveries.Where
            //                                            (
            //                                                x => x.STOrderDetailId == model.STOrderDetailId
            //                                                && x.ItemId == model.ItemId
            //                                                && (x.DeliveryStatus == DeliveryStatusEnum.Pending || x.DeliveryStatus == DeliveryStatusEnum.Waiting)
            //                                            )
            //                                            .Select(x => x.Quantity)
            //                                            .Sum()
            //                                        );

            ////  Get all delivered quantity
            //var deliveredQuantity = Convert.ToInt32(this._context.STClientDeliveries.Where
            //                                            (
            //                                                x => x.STOrderDetailId == model.STOrderDetailId
            //                                                && x.ItemId == model.ItemId 
            //                                                && x.DeliveryStatus == DeliveryStatusEnum.Delivered
            //                                            )
            //                                            .Sum(x => x.DeliveredQuantity)
            //                                        ) + model.Quantity;

            //added for ticket #418
            var forDeliveryQty = Convert.ToInt32(this._context.STClientDeliveries.Where(

                 x => x.STOrderDetailId == model.STOrderDetailId
                 && x.ItemId == model.ItemId
                )
                .Sum(x => x.Quantity)) + model.Quantity;
                

            

            if (item != null)
            {
                //remove for ticket #418
                //if ((requestedQuantity + deliveredQuantity) > item.ApprovedQuantity)
                //added for ticket #418
                if (forDeliveryQty > item.ApprovedQuantity)
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

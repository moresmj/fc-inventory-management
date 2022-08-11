using FC.Api.DTOs.Store.Returns;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Linq;

namespace FC.Api.Validators.Warehouse
{
    public class AddPurchaseReturnDeliveryItemsValidator : AbstractValidator<AddPurchaseReturnDeliveryItems>
    {

        private readonly DataContext _context;

        public AddPurchaseReturnDeliveryItemsValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate STReturnDetailId
            RuleFor(p => p.STPurchaseReturnId)
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


        private bool ValidItem(AddPurchaseReturnDeliveryItems model)
        {
            var item = this._context.STPurchaseReturns.Where(x => x.Id == model.STPurchaseReturnId
                                                               && x.STReturnId == model.STReturnId
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

        private bool QuantityNotMoreThanApprovedQuantity(AddPurchaseReturnDeliveryItems model)
        {
            var item = this._context.STPurchaseReturns.Where(x => x.Id == model.STPurchaseReturnId
                                                               && x.STReturnId == model.STReturnId
                                                               && x.ItemId == model.ItemId).FirstOrDefault();

            //  Get all delivered
            var deliveredQty = Convert.ToInt32(this._context.WHDeliveryDetails.Where
                                                        (
                                                            x => x.STPurchaseReturnId == model.STPurchaseReturnId
                                                            && x.ItemId == model.ItemId
                                                            && x.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                        )
                                                        .Sum(p => p.Quantity)
                                                    );

            //  Get all requested for delivery
            var reqForDelivery = Convert.ToInt32(this._context.WHDeliveryDetails.Where
                                                        (
                                                            x => x.STPurchaseReturnId == model.STPurchaseReturnId
                                                            && x.ItemId == model.ItemId
                                                            && (x.DeliveryStatus == DeliveryStatusEnum.Pending || x.DeliveryStatus == DeliveryStatusEnum.Waiting)
                                                        )
                                                        .Sum(x => x.Quantity)
                                                    ) + model.Quantity;

            if (item != null)
            {
                if ((reqForDelivery + deliveredQty) > (item.GoodQuantity + item.BrokenQuantity))
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

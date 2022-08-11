using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Releasing;
using InventorySystemAPI.Models.Store.Sales;
using InventorySystemAPI.Models.Warehouse.Stock;
using System;
using System.Linq;

namespace InventorySystemAPI.Validators.Store.Releasing
{
    public class STReleaseDetailValidator : AbstractValidator<STReleaseDetail>
    {
        private readonly FloorCenterContext _context;

        private readonly DeliveryTypeEnum _deliveryType;

        public STReleaseDetailValidator(FloorCenterContext context, DeliveryTypeEnum deliveryType)
        {
            this._context = context;

            this._deliveryType = deliveryType;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate STSalesDetailId
            RuleFor(p => p.STSalesDetailId)
                .NotEmpty();

            RuleFor(p => p)
                .Must(ValidRecord)
                        .WithMessage("Record is not valid");


            //  Validate Quantity
            RuleFor(p => p.Quantity)
                .NotEmpty();

            RuleFor(p => p)
                .Must(QuantityNotMoreThanOnRecord)
                        .WithMessage("Quantity is more than the ordered quantity")
                        .WithName("Quantity");

        }


        private bool ValidRecord(STReleaseDetail model)
        {
            if (model.STSalesDetailId.HasValue)
            {
                //TODO: Get current user's store id
                var store_id = 1;

                var count = this._context.STSalesDetails
                            .Where(
                                        x => x.Id == model.STSalesDetailId
                                        && x.STSales.StoreId == store_id
                                        && x.STSales.DeliveryType == this._deliveryType
                                        && x.DeliveryStatus == DeliveryStatusEnum.Waiting
                                   ).Count();

                if (count < 1)
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


        private bool QuantityNotMoreThanOnRecord(STReleaseDetail model)
        {
            if (model.STSalesDetailId.HasValue && model.Quantity.Value > 0)
            {
                //TODO: Get current user's store id
                var store_id = 1;
                var count = Convert.ToInt32(this._context.STSalesDetails
                                            .Where(
                                                        x => x.Id == model.STSalesDetailId
                                                        && x.STSales.StoreId == store_id
                                                        && x.STSales.DeliveryType == this._deliveryType
                                                        && x.DeliveryStatus == DeliveryStatusEnum.Waiting
                                                   ).Sum(x => x.Quantity)
                                   );

                if (model.Quantity > count)
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

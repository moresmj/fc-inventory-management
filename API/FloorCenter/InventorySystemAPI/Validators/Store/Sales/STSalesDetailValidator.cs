using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Sales;
using System;
using System.Linq;

namespace InventorySystemAPI.Validators.Store.Sales
{
    public class STSalesDetailValidator : AbstractValidator<STSalesDetail>
    {

        private readonly FloorCenterContext _context;
        
        public STSalesDetailValidator(FloorCenterContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            ////  Validate ItemId
            RuleFor(p => p.ItemId)
                    .NotEmpty()
                    .Must(ValidItem)
                        .WithMessage("{PropertyName} is not valid");


            //  Validate Quantity
            RuleFor(p => p.Quantity)
                    .NotEmpty()
                    .GreaterThan(0);

            RuleFor(p => p)
                .Must(QuantityNotMoreThanAvailableOnHand)
                    .WithMessage("The selected quantity is more than the available quantity");


        }


        private bool ValidItem(int? itemId)
        {
            //TODO: Get current user's store id
            var onHand = Convert.ToInt32(
                                            this._context.STStocks
                                                .Where(
                                                            x => x.ItemId == itemId 
                                                            //&& x.STDeliveryDetail.STDelivery.StoreId == store_id_here
                                                       )
                                                .Sum(x => x.OnHand)
                                        );

            if (onHand < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        private bool QuantityNotMoreThanAvailableOnHand(STSalesDetail model)
        {
            if (model.Quantity.HasValue)
            {
                //TODO: Get current user's store id
                var onHand = Convert.ToInt32(
                                                this._context.STStocks
                                                    .Where(
                                                                x => x.ItemId == model.ItemId
                                                           //&& x.STDeliveryDetail.STDelivery.StoreId == store_id_here
                                                           )
                                                    .Sum(x => x.OnHand)
                                            );
                
                if (model.Quantity > onHand)
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

using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class AddSalesOrderDeliveryItemsValidator : AbstractValidator<AddSalesOrderDeliveryItems>
    {

        private readonly DataContext _context;

        public AddSalesOrderDeliveryItemsValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


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


        private bool ValidItem(AddSalesOrderDeliveryItems model)
        {
            var item = this._context.STSalesDetails.Where(x => x.Id == model.STSalesDetailId && x.STSalesId == model.STSalesId && x.ItemId == model.ItemId).Count();
            if (item != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool QuantityNotMoreThanApprovedQuantity(AddSalesOrderDeliveryItems model)
        {
            var item = this._context.STSalesDetails.Where(x => x.Id == model.STSalesDetailId && x.STSalesId == model.STSalesId && x.ItemId == model.ItemId).FirstOrDefault();

            //#422 added exceeding input quantity
            var forDeliveryQty = Convert.ToInt32(this._context.STClientDeliveries.Where
                                                (
                                                    x => x.STSalesDetailId == model.STSalesDetailId
                                                   && x.ItemId == model.ItemId
                                                )
                                                .Sum(x => x.Quantity)) + model.Quantity;



            if (item != null)
            {
                if (forDeliveryQty > item.Quantity)
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

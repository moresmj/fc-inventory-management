using FC.Api.DTOs.Store.Returns.ClientReturn;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Linq;

namespace FC.Api.Validators.Store.Returns
{
    public class AddClientReturnItemsValidator : AbstractValidator<AddClientReturnItems>
    {

        private readonly DataContext _context;

        public AddClientReturnItemsValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            //  Validate STSalesId
            RuleFor(p => p.STSalesId)
                    .NotEmpty();


            //  Validate ItemId
            RuleFor(p => p.ItemId)
                    .NotEmpty();


            //  Validate Record
            RuleFor(p => p)
                    .Must(ValidItem)
                        .When(p => p.STSalesId.HasValue && p.ItemId.HasValue)
                        .WithMessage("Record is not valid")
                        .WithName("Id & STSalesId");


            //  Validate Quantity
            RuleFor(p => p.Quantity)
                    .GreaterThanOrEqualTo(0);


            //  Validate ReturnReason
            RuleFor(p => p.ReturnReason)
                    .NotEmpty()
                        .When(p => p.Quantity > 0)
                    .IsInEnum();


            //  Validate Remarks
            RuleFor(p => p.Remarks)
                    .NotEmpty()
                        .When(p => p.ReturnReason == ReturnReasonEnum.Others);


            //  Validate Total Quantity
            RuleFor(p => p)
                    .Must(ReturnQuantityMustNotBeMoreThanPurchasedQuantity)
                        .When(p => p.STSalesId.HasValue && p.ItemId.HasValue)
                        .WithMessage("Return Qty should not be more than the purchased quantity")
                        .WithName("Return Qty");


        }

        private bool ValidItem(AddClientReturnItems model)
        {
            var count = this._context.STSalesDetails
                                    .Where(x => x.Id == model.Id
                                                && x.STSalesId == model.STSalesId
                                                && x.ItemId == model.ItemId
                                                && x.DeliveryStatus == DeliveryStatusEnum.Delivered)
                                    .Count();
            if (count == 0)
            {
                return false;
            }

            return true;
        }

        private bool ReturnQuantityMustNotBeMoreThanPurchasedQuantity(AddClientReturnItems model)
        {
            //  Get STSalesDetail record
            var objSTSalesDetail = _context.STSalesDetails
                                           .Where(p => p.Id == model.Id
                                                       && p.STSalesId == model.STSalesId
                                                       && p.ItemId == model.ItemId
                                                       && p.DeliveryStatus == DeliveryStatusEnum.Delivered)
                                           .FirstOrDefault();


            //  Get return records
            var totalReturnQty = Convert.ToInt32(
                                    _context.STClientReturns
                                            .Where(p => p.STSalesDetailId == model.Id
                                                        && p.ItemId == model.ItemId
                                                        && p.DeliveryStatus != DeliveryStatusEnum.NotDelivered)
                                            .Sum(p => p.Quantity));



            if(objSTSalesDetail != null)
            {
                if(objSTSalesDetail.Quantity >= (totalReturnQty + model.Quantity))
                {
                    return true;
                }
            }

            return false;
        }

    }
}

using FC.Api.DTOs.Warehouse.Receive_Items;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Linq;

namespace FC.Api.Validators.Warehouse.Receive_Items
{
    public class ReceiveReturnsItemsValidator : AbstractValidator<ReceiveReturnsItems>
    {

        private DataContext _context;

        public ReceiveReturnsItemsValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            //  Validate Id
            RuleFor(p => p.Id)
                    .NotEmpty();


            //  Validate WHDeliveryId
            RuleFor(p => p.WHDeliveryId)
                    .NotEmpty();


            //  Validate STPurchaseReturnId
            RuleFor(p => p.STPurchaseReturnId)
                    .NotEmpty();


            //  Validate ItemId
            RuleFor(p => p.ItemId)
                    .NotEmpty();


            //  Validate GoodQuantity
            RuleFor(p => p.GoodQuantity)
                    .GreaterThanOrEqualTo(0);


            //  Validate BrokenQuantity
            RuleFor(p => p.BrokenQuantity)
                    .GreaterThanOrEqualTo(0);


            //  Validate WHDeliveryDetail record
            RuleFor(p => p)
                    .Must(ValidRecord)
                        .WithMessage("Record is not valid")
                        .WithName("Id & ItemId");


            //Validate Total Good And Broken Quantity
            RuleFor(p => p)
                    .Must(GoodAndBrokenQuantityMustNotBeMoreThanDeliveredQuantity)
                        .WithMessage("Total Good and Broken Quantity to be received should not be more than the delivered quantity")
                        .WithName("GoodQuantity & BrokenQuantity");

            //RuleFor(p => p)
            //        .Must(GoodAndBrokenQuantityMustNotBeMoreThanRequestedToReturnQuantity)
            //            .WithMessage("Total Good and Broken Quantity to be received should not be more than the delivered quantity")
            //            .WithName("GoodQuantity & BrokenQuantity");

        }

        private bool ValidRecord(ReceiveReturnsItems model)
        {
            return _context.WHDeliveryDetails.Where(p => p.Id == model.Id && p.ItemId == model.ItemId).Count() == 1;
        }

        private bool GoodAndBrokenQuantityMustNotBeMoreThanDeliveredQuantity(ReceiveReturnsItems model)
        {
            if(model.Id.HasValue && model.WHDeliveryId.HasValue && model.STPurchaseReturnId.HasValue && model.ItemId.HasValue)
            {
                var whDeliveryDetail = _context.WHDeliveryDetails.Where(p => p.Id == model.Id
                                                                             && p.WHDeliveryId == model.WHDeliveryId
                                                                             && p.STPurchaseReturnId == model.STPurchaseReturnId
                                                                             && p.ItemId == model.ItemId)
                                                                 .FirstOrDefault();
                if(whDeliveryDetail != null)
                {
                    if((model.GoodQuantity + model.BrokenQuantity) > whDeliveryDetail.Quantity)
                    {
                        return false;
                    }
                }
            }
            
            
            return true;
        }


        private bool GoodAndBrokenQuantityMustNotBeMoreThanRequestedToReturnQuantity(ReceiveReturnsItems model)
        {
            if (model.STPurchaseReturnId.HasValue && model.ItemId.HasValue)
            {
                //  Get purchase return record
                var stPurchaseReturn = _context.STPurchaseReturns
                                               .Where(p => p.Id == model.STPurchaseReturnId
                                                           && p.ItemId == model.ItemId)
                                               .FirstOrDefault();
                if(stPurchaseReturn != null)
                {
                    //  Get total requested return qty
                    var total = Convert.ToInt32(stPurchaseReturn.GoodQuantity) + Convert.ToInt32(stPurchaseReturn.BrokenQuantity);

                    //  Get warehouse delivery detail records
                    var warehouseDeliveries = _context.WHDeliveryDetails
                                                      .Where(p => p.STPurchaseReturnId == stPurchaseReturn.Id
                                                                  && p.ItemId == stPurchaseReturn.ItemId)
                                                      .ToList();

                    if(warehouseDeliveries != null)
                    {
                        //  Get delivered records
                        var deliveredItems = warehouseDeliveries.Where(p => p.DeliveryStatus == DeliveryStatusEnum.Delivered
                                                                            && p.ReleaseStatus == ReleaseStatusEnum.Released);

                        var totalGoodDelivered = Convert.ToInt32(deliveredItems.Sum(p => p.ReceivedGoodQuantity));
                        var totalBrokenDelivered = Convert.ToInt32(deliveredItems.Sum(p => p.ReceivedBrokenQuantity));


                        //  Get total released not yet delivered items
                        var totalReleased = Convert.ToInt32(warehouseDeliveries.Where(p => p.DeliveryStatus != DeliveryStatusEnum.Delivered
                                                                           && p.ReleaseStatus == ReleaseStatusEnum.Released)
                                                               .Sum(p => p.Quantity));


                        if((totalGoodDelivered + totalBrokenDelivered + totalReleased) > total)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}

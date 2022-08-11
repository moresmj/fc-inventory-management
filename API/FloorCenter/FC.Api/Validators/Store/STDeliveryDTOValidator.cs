using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class STDeliveryDTOValidator : AbstractValidator<STDeliveryDTO>
    {

        private readonly DataContext _context;

        public STDeliveryDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Id
            RuleFor(p => p.Id)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid");


            //  Validate DRNumber
            RuleFor(p => p.DRNumber)
                .NotEmpty();


            //  Validate Delivery Date
            RuleFor(p => p.DeliveryDate)
                .NotEmpty();


            RuleFor(p => p.PlateNumber)
                 .NotEmpty()
                    .MaximumLength(10);


            // Validate DRNumber and Deliery Date
            RuleFor(p => p)
                .Must(DRNumberAndDeliveryDateNotYetRegistered)
                    .WithMessage("DR Number and Delivery Date already registered")
                    .WithName("DRNumber and DeliveryDate");


            //  Validat Showroom items for delivery
            RuleFor(p => p.ShowroomDeliveries)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .Must(ShouldHaveAtLeastOneItemToBeRequestedForDelivery)
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .SetCollectionValidator(new STShowroomDeliveryDTOValidator(context));

        }

        private bool ValidRecord(int id)
        {
            var obj = (from x in _context.STOrders.Where(x => x.Id == id 
                                                              && x.OrderType == OrderTypeEnum.ShowroomStockOrder 
                                                              && x.DeliveryType != DeliveryTypeEnum.Pickup
                                                              && x.RequestStatus == RequestStatusEnum.Approved)
                      .Include(y => y.OrderedItems)
                       select new STOrder
                       {
                           Id = x.Id,
                           StoreId = x.StoreId,
                           WarehouseId = x.WarehouseId,
                           PONumber = x.PONumber,
                           TransactionType = x.TransactionType,
                           Remarks = x.Remarks,
                           RequestStatus = x.RequestStatus,
                           PODate = x.PODate,
                           OrderedItems = x.OrderedItems.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList()
                       }).Where(y => y.OrderedItems.Count > 0).FirstOrDefault();

            if (obj == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool DRNumberAndDeliveryDateNotYetRegistered(STDeliveryDTO model)
        {
            if (!string.IsNullOrWhiteSpace(model.DRNumber) && model.DeliveryDate.HasValue)
            {
                var count = _context.STDeliveries.Where(p => p.DRNumber == model.DRNumber && p.DeliveryDate == model.DeliveryDate).Count();
                return (count == 0);
            }

            return true;
        }

        private bool ShouldHaveAtLeastOneItemToBeRequestedForDelivery(ICollection<STShowroomDeliveryDTO> showroomDeliveries)
        {
            if(showroomDeliveries != null && showroomDeliveries.Count > 0)
            {
                return (showroomDeliveries.Where(p => p.Quantity > 0).Count() > 0);
            }

            return true;
        }


    }
}

using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FC.Api.Validators.Store
{
    public class AddClientDeliveryDTOValidator : AbstractValidator<AddClientDeliveryDTO>
    {

        private readonly DataContext _context;

        public AddClientDeliveryDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Id
            RuleFor(p => p.Id)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid");


            //  Validate Delivery Date
            RuleFor(p => p.DeliveryDate)
                .NotEmpty();


            //  Validate SINumber
            RuleFor(p => p.SINumber)
                .NotEmpty()
                .MaximumLength(50);


            //  Validate SINumber
            RuleFor(p => p)
                .Must(isNotAccountsReceivable)
                .WithMessage("'ORNumber' should not be empty")
                .Must(rec => rec.ORNumber.ToString().Length < 51)
                .WithMessage("The length of 'OR Number' must be 50 characters or fewer")
                .When(isNotAccountsReceivable);
                


            //  Validate DRNumber
            RuleFor(p => p.DRNumber)
                .NotEmpty()
                .MaximumLength(50);


            //  Validate PreferredTime
            RuleFor(p => p.PreferredTime)
                .NotEmpty()
                .IsInEnum();


            //  Validate ClientName
            RuleFor(p => p.ClientName)
                .NotEmpty();


            //  Validate Address
            RuleFor(p => p)
                .Must(CheckAddress)
                    .WithMessage("'Address' should not be empty");


            //  Validate Contact Number
            RuleFor(p => p)
              .Must(checkType)
              .WithMessage("'Contact' number should not be empty");


            // Validate DRNumber and Deliery Date
            //RuleFor(p => p)
            //    .Must(DRNumberAndDeliveryDateNotYetRegistered)
            //        .WithMessage("DR Number and Delivery Date already registered")
            //        .WithName("DRNumber and DeliveryDate");


            //  Validate Client items for delivery
            RuleFor(p => p.ClientDeliveries)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .Must(ShouldHaveAtLeastOneItemToBeRequestedForDelivery)
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .SetCollectionValidator(new AddClientDeliveryItemsValidator(context));

            RuleFor(p => p)
                .Must(IdShouldMatchListSTOrderId)
                    .WithMessage("Invalid item(s) found on the list.")
                    .WithName("Id & ShowroomDeliveries.STOrderId");

        }

        private bool ValidRecord(int id)
        {
            var obj = (from x in _context.STOrders.Where(x => x.Id == id
                                                              &&
                                                              (
                                                                    x.OrderType == OrderTypeEnum.ClientOrder
                                                                    || x.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                              )
                                                              //&& x.DeliveryType == DeliveryTypeEnum.Delivery
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

        private bool DRNumberAndDeliveryDateNotYetRegistered(AddClientDeliveryDTO model)
        {
            if (!string.IsNullOrWhiteSpace(model.DRNumber) && model.DeliveryDate.HasValue)
            {
                var obj = (from x in _context.STOrders.Where(x => x.Id == model.Id
                                                              &&
                                                              (
                                                                    x.OrderType == OrderTypeEnum.ClientOrder
                                                                    || x.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                              )
                                          
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
                               OrderType = x.OrderType,
                               DeliveryType = x.DeliveryType,
                               OrderToStoreId=x.OrderToStoreId,
                               OrderedItems = x.OrderedItems.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList()
                           }).Where(y => y.OrderedItems.Count > 0).FirstOrDefault();

                if(obj != null)
                {
                    if (obj.DeliveryType == DeliveryTypeEnum.Pickup && obj.OrderType == OrderTypeEnum.ClientOrder)
                    {
                        return true;
                    }
                }


                var count = 0;
                if (obj != null && obj.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder)
                {
                    count = _context.STDeliveries.Where(p => p.DRNumber == model.DRNumber && p.DeliveryDate == model.DeliveryDate && p.DeliveryFromStoreId == obj.OrderToStoreId).Count();
                }
                else
                {
                    count = _context.STDeliveries.Where(p => p.DRNumber == model.DRNumber && p.DeliveryDate == model.DeliveryDate).Count();
                }
               
                return (count == 0);
            }

            return true;
        }

        private bool ShouldHaveAtLeastOneItemToBeRequestedForDelivery(ICollection<AddClientDeliveryItems> deliveries)
        {
            if (deliveries != null && deliveries.Count > 0)
            {
                return (deliveries.Where(p => p.Quantity > 0).Count() > 0);
            }

            return true;
        }

        private bool IdShouldMatchListSTOrderId(AddClientDeliveryDTO model)
        {
            if (model.ClientDeliveries != null && model.ClientDeliveries.Count > 0)
            {
                return (model.ClientDeliveries.Where(p => p.STOrderId != model.Id).Count() == 0);
            }

            return true;
        }
        private bool checkType(AddClientDeliveryDTO model)
        {
            var obj = (from x in _context.STOrders.Where(x => x.Id == model.Id
                                                              &&
                                                              (
                                                                    x.OrderType == OrderTypeEnum.ClientOrder
                                                                    || x.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                              )
                                                              //&& x.DeliveryType == DeliveryTypeEnum.Delivery
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
                           OrderType = x.OrderType,
                           DeliveryType = x.DeliveryType,
                           OrderedItems = x.OrderedItems.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList()
                       }).Where(y => y.OrderedItems.Count > 0).FirstOrDefault();

            if (obj.OrderType == OrderTypeEnum.ClientOrder && obj.DeliveryType == DeliveryTypeEnum.Pickup)
            {
                return true;
            }
            else
            {
              
                    if (string.IsNullOrEmpty(model.ContactNumber))
                    {
                        return false;
                    }

                    return Regex.IsMatch(model.ContactNumber, @"\s*(?:\+?(\d{1,3}))?([-. (]*(\d{3})[-. )]*)?((\d{3})[-. ]*(\d{2,4})(?:[-.x ]*(\d+))?)\s*");
            }
        }

        private bool CheckAddress(AddClientDeliveryDTO model)
        {

            var obj = (from x in _context.STOrders.Where(x => x.Id == model.Id
                                                              &&
                                                              (
                                                                    x.OrderType == OrderTypeEnum.ClientOrder
                                                                    || x.OrderType == OrderTypeEnum.InterbranchOrIntercompanyOrder
                                                              )
                                                              //&& x.DeliveryType == DeliveryTypeEnum.Delivery
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
                           OrderType = x.OrderType,
                           DeliveryType = x.DeliveryType,
                           OrderedItems = x.OrderedItems.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList()
                       }).Where(y => y.OrderedItems.Count > 0).FirstOrDefault();

            if(obj.OrderType != OrderTypeEnum.ClientOrder && obj.DeliveryType != DeliveryTypeEnum.Pickup)
            {
                if (string.IsNullOrEmpty(model.Address1))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

      

            
        }


        private bool isNotAccountsReceivable(AddClientDeliveryDTO model)
        {
            if (model.PaymentMode != PaymentModeEnum.AccountsReceivable)
            {
                
                if(string.IsNullOrEmpty(model.ORNumber))
                {
                    return false;
                }
                else
                {
                    return true;
                }
                
            }
            else
            {
                return true;
            }

          
        }

    }
}

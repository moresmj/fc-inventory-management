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
    public class AddSalesOrderDeliveryDTOValidator : AbstractValidator<AddSalesOrderDeliveryDTO>
    {

        private readonly DataContext _context;

        public AddSalesOrderDeliveryDTOValidator(DataContext context)
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
            RuleFor(p => p.ORNumber)
                .NotEmpty()
                .MaximumLength(50);


            //  Validate DRNumber
            RuleFor(p => p.DRNumber)
                .NotEmpty()
                .When(p => p.DeliveryType == DeliveryTypeEnum.Delivery)
                .MaximumLength(50);


            //  Validate PreferredTime
            RuleFor(p => p.PreferredTime)
                .NotEmpty()
                .IsInEnum();


            //  Validate ClientName
            RuleFor(p => p.ClientName)
                .NotEmpty();


            //  Validate Address
            RuleFor(p => p.Address1)
                .NotEmpty()
                .When(IsRequired)
                    .WithMessage("'Address' should not be empty");


            //  Validate Contact Number
            RuleFor(p => p.ContactNumber)
                .NotEmpty()
                .Matches("[0-9]")
                .When(IsRequired);



            // Validate DRNumber and Deliery Date
            //RuleFor(p => p)
            //    .Must(DRNumberAndDeliveryDateNotYetRegistered)
            //        .WithMessage("Delivery Date already registered")
            //        .WithName("DeliveryDate");


            //  Validate Client items for delivery
            RuleFor(p => p.ClientDeliveries)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .Must(ShouldHaveAtLeastOneItemToBeRequestedForDelivery)
                    .WithMessage("Please add at least one item to be requested for delivery.")
                .SetCollectionValidator(new AddSalesOrderDeliveryItemsValidator(context));

            RuleFor(p => p)
                .Must(IdShouldMatchListSTSalesId)
                    .WithMessage("Invalid item(s) found on the list.")
                    .WithName("Id & ClientDeliveries.STSalesId");

        }

        private bool ValidRecord(int id)
        {
            var obj = (from x in _context.STSales.Where(x => x.Id == id
                                                              && x.SalesType == SalesTypeEnum.SalesOrder)
                                          .Include(y => y.SoldItems)
                       select new STSales
                       {
                           SoldItems = x.SoldItems.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Pending).ToList()
                       }).Where(y => y.SoldItems.Count > 0).FirstOrDefault();

            if (obj == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool DRNumberAndDeliveryDateNotYetRegistered(AddSalesOrderDeliveryDTO model)
        {
            if (!string.IsNullOrWhiteSpace(model.DRNumber) && model.DeliveryDate.HasValue)
            {
                var count = _context.STDeliveries.Where(p => p.DRNumber == model.DRNumber && p.DeliveryDate == model.DeliveryDate).Count();
                return (count == 0);
            }

            return true;
        }

        private bool ShouldHaveAtLeastOneItemToBeRequestedForDelivery(ICollection<AddSalesOrderDeliveryItems> deliveries)
        {
            if (deliveries != null && deliveries.Count > 0)
            {
                return (deliveries.Where(p => p.Quantity > 0).Count() > 0);
            }

            return true;
        }

        private bool IdShouldMatchListSTSalesId(AddSalesOrderDeliveryDTO model)
        {
            if (model.ClientDeliveries != null && model.ClientDeliveries.Count > 0)
            {
                return (model.ClientDeliveries.Where(p => p.STSalesId != model.Id).Count() == 0);
            }

            return true;
        }

        private bool IsRequired(AddSalesOrderDeliveryDTO model)
        {
            var deliveryType = _context.STSales.AsNoTracking().Where(p => p.Id == model.Id).Select(p => p.DeliveryType).FirstOrDefault();
            return deliveryType == DeliveryTypeEnum.Delivery;

        }

    }
}

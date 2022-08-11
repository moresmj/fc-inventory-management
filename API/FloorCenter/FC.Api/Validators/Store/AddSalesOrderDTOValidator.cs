using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FC.Api.Validators.Store
{
    public class AddSalesOrderDTOValidator : AbstractValidator<AddSalesOrderDTO>
    {

        private readonly DataContext _context;

        public AddSalesOrderDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            //  Validate SINumber
            RuleFor(p => p.SINumber)
                    .NotEmpty()
                    .MaximumLength(50);

            
            //  Validate ORNumber
            RuleFor(p => p.ORNumber)
                    .NotEmpty()
                    .MaximumLength(50);


            //  Validate DRNumber
            RuleFor(p => p.DRNumber)
                    .NotEmpty()
                    .When(p => p.DeliveryType == DeliveryTypeEnum.Delivery)
                    .MaximumLength(50);


            //  Validate DeliveryType
            RuleFor(p => p.DeliveryType)
                    .NotEmpty()
                    .Must(PickupAndDeliveryOnly)
                        .WithMessage("'{PropertyName}' is not valid");



            //  Validate SalesDate
            RuleFor(p => p.SalesDate)
                    .NotEmpty();


            //  Validate SalesAgent
            RuleFor(p => p.SalesAgent)
                    .NotEmpty();


            //  Validate ClientName
            RuleFor(p => p.ClientName)
                    .NotEmpty();


            //  Validate Address
            RuleFor(p => p.Address1)
                .NotEmpty()
                    .When(p => p.DeliveryType == DeliveryTypeEnum.Delivery)
                    .WithMessage("'Address' should not be empty");


            //  Validate Contact Number
            RuleFor(p => p.ContactNumber)
                .NotEmpty()
                    .When(p => p.DeliveryType == DeliveryTypeEnum.Delivery)
                .Must(IsValidContact)
                    .WithMessage("'{PropertyName}' is not in the correct format.");

            RuleFor(p => p)
                .Must(AllItemsAreRegisteredInStore)
                    .WithMessage("Found invalid item(s) on the list")
                    .WithName("StoreId & Sold Items");


            //  Validate SoldItems
            RuleFor(p => p.SoldItems)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be sold.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be sold.")
                .SetCollectionValidator(new AddSalesOrderItemsValidator(context));
        }

        private bool PickupAndDeliveryOnly(DeliveryTypeEnum? deliveryType)
        {
            if (deliveryType.HasValue)
            {
                if (deliveryType == DeliveryTypeEnum.ShowroomPickup)
                {
                    return false;
                }
            }

            return true;
        }

        private bool AllItemsAreRegisteredInStore(AddSalesOrderDTO model)
        {
            if (model.StoreId.HasValue && (model.SoldItems != null && model.SoldItems.Count() > 0))
            {
                bool foundInvalid = false;

                foreach (var item in model.SoldItems)
                {
                    var count = _context.STStocks
                                        .Where(p => p.StoreId == model.StoreId
                                                    && p.ItemId == item.ItemId)
                                        .Count();

                    if (count > 0)
                    {
                        foundInvalid = true;
                        break;
                    }
                }

                return foundInvalid;
            }

            return true;
        }

        private bool IsValidContact(string contactNumber)
        {
            if (string.IsNullOrEmpty(contactNumber))
            {
                return true;
            }
            return Regex.IsMatch(contactNumber, @"^[-()0-9]+$");
        }

    }
}

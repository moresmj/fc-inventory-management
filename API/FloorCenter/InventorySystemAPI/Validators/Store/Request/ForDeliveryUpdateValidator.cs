using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Request;
using InventorySystemAPI.Models.Warehouse.Stock;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace InventorySystemAPI.Validators.Store.Request
{
    public class ForDeliveryUpdateValidator : AbstractValidator<ForDeliveryUpdate>
    {

        private readonly FloorCenterContext _context;

        public ForDeliveryUpdateValidator(FloorCenterContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.Id)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid");


            //  Validate ReceivedBy
            RuleFor(p => p.ReceivedBy)
                    .NotEmpty()
                    .Must(ValidUser)
                        .WithMessage("{PropertyName} is not valid");


            RuleFor(p => p.ItemsToBeDelivered).SetCollectionValidator(new ForDeliveryUpdateItemsValidator(context));

        }

        private bool ValidRecord(int id)
        {
            //TODO: Get logged in user store id
            var store_id = 1;

            var obj = (from x in _context.STDeliveries.Where(x => x.Id == id && x.StoreId == store_id)
                      .Include(y => y.STInventory)
                      .Include(y => y.ItemsToBeDelivered)
                       select new ForDeliveryUpdate
                       {
                           Id = x.Id,
                           StoreId = x.StoreId,
                           DeliveryDate = x.DeliveryDate,
                           ItemsToBeDelivered = x.ItemsToBeDelivered.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList()
                       }).Where(y => y.ItemsToBeDelivered.Count > 0).Count();

            if (obj < 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool ValidUser(int? checkedBy)
        {
            //TO DO: Get warehouse's users list only
            var user = this._context.Users.Where(x => x.Id == checkedBy).Count();

            if (user != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}

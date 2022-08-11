using FluentValidation;
using InventorySystemAPI.Context;
using InventorySystemAPI.Models.Store.Inventory;
using InventorySystemAPI.Models.Warehouse.Stock;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace InventorySystemAPI.Validators.Store.Delivery
{
    public class STDeliveryValidator : AbstractValidator<Models.Store.Request.STDelivery>
    {

        private readonly FloorCenterContext _context;

        public STDeliveryValidator(FloorCenterContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.Id)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid");

            RuleFor(p => p.DeliveryDate)
                .NotEmpty();

            RuleFor(p => p.ItemsToBeDelivered).SetCollectionValidator(new STDeliveryDetailValidator(context));

        }

        private bool ValidRecord(int id)
        {
            //TODO: Get logged in user store id
            var store_id = 1;

            var obj = (from x in _context.STInventories.Where(x => x.Id == id && x.StoreId == store_id && x.RequestStatus == RequestStatusEnum.Approved)
                      .Include(y => y.RequestedItems)
                       select new STInventory
                       {
                           Id = x.Id,
                           StoreId = x.StoreId,
                           WarehouseId = x.WarehouseId,
                           PONumber = x.PONumber,
                           TransactionType = x.TransactionType,
                           Remarks = x.Remarks,
                           RequestStatus = x.RequestStatus,
                           PODate = x.PODate,
                           RequestedItems = x.RequestedItems.Where(z => z.DeliveryStatus == DeliveryStatusEnum.Waiting).ToList()
                       }).Where(y => y.RequestedItems.Count > 0).FirstOrDefault();

            if (obj == null)
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

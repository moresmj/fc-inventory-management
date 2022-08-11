using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Api.Services.Stores;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FC.Api.Validators.Store
{
    public class SaveReceiveItemValidator : AbstractValidator<SaveReceiveItem>
    {

        private readonly DataContext _context;

        public SaveReceiveItemValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            //  Validate Id
            RuleFor(p => p.Id)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid");

            RuleFor(p => p)
                .Must(IsDRNumberRequired)
                    .WithMessage("DR Number is required")
                    .WithName("DR Number")
                 .Must(rec => rec.DRNumber.ToString().Length < 50)
                 .WithMessage("The length of 'DR Number' must be 50 characters or fewer")
                 .When(IsDRNumberRequired);

            RuleFor(p => p)
                .Must(IsDRDateRequired)
                    .WithMessage("DR Date is required")
                    .WithName("DR Date");

            RuleFor(p => p.ShowroomDeliveries)
                .NotEmpty()
                    .WithMessage("Please add at least one item to be received.")
                .Must(list => list.Count > 0)
                    .WithMessage("Please add at least one item to be received.")
                .SetCollectionValidator(new SaveReceiveItemDeliveriesValidator(context));


            RuleFor(p => p)
                .Must(IdShouldMatchListSTOrderId)
                    .WithMessage("Invalid item(s) found on the list.")
                    .WithName("Id & ShowroomDeliveries.STOrderId");

        }

        private bool ValidRecord(int id)
        {
            var obj = new STOrderService(_context).GetReceivingItemById(id);
            if(obj != null)
            {
                return true;
            }

            return false;
        }

        private bool IdShouldMatchListSTOrderId(SaveReceiveItem model)
        {
            if (model.ShowroomDeliveries != null && model.ShowroomDeliveries.Count > 0)
            {
                return (model.ShowroomDeliveries.Where(p => p.STDeliveryId != model.Id).Count() == 0);
            }

            return true;
        }

        private bool IsDRNumberRequired(SaveReceiveItem model)
        {
            var orderId = _context.STDeliveries.AsNoTracking().Where(p => p.Id == model.Id).Select(p => p.STOrderId).FirstOrDefault();
            var order = _context.STOrders.AsNoTracking().Include(p => p.Warehouse).Where(p => p.Id == orderId).FirstOrDefault();


            if (order.Warehouse == null)
            {
                return true;
            }
            else if(order.Warehouse.Vendor)
            {
                return (string.IsNullOrEmpty(model.DRNumber)) ? false : true;
            }
            return true;
        }

        private bool IsDRDateRequired(SaveReceiveItem model)
        {
            var orderId = _context.STDeliveries.AsNoTracking().Where(p => p.Id == model.Id).Select(p => p.STOrderId).FirstOrDefault();
            var order = _context.STOrders.AsNoTracking().Include(p => p.Warehouse).Where(p => p.Id == orderId).FirstOrDefault();


            if (order.Warehouse == null)
            {
                return true;
            }
            else if(order.Warehouse.Vendor)
            {
                return (model.DeliveryDate.HasValue) ? true : false;
            }
            return true;
        }

    }
}

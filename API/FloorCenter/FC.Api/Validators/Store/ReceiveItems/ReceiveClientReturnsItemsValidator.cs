using FC.Api.DTOs.Store.ReceiveItems;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.Store.ReceiveItems
{
    public class ReceiveClientReturnsItemsValidator : AbstractValidator<ReceiveClientReturnsItems>
    {
        private DataContext _context;

        public ReceiveClientReturnsItemsValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            //  Validate STReturnId
            RuleFor(p => p.STReturnId)
                    .NotEmpty();


            //  Validate ItemId
            RuleFor(p => p.ItemId)
                    .NotEmpty();


            //  Validate Record
            RuleFor(p => p)
                    .Must(ValidItem)
                        .When(p => p.STReturnId.HasValue && p.ItemId.HasValue && p.StoreId.HasValue)
                        .WithMessage("Record is not valid")
                        .WithName("Id & STReturnId");
        }

        private bool ValidItem(ReceiveClientReturnsItems model)
        {
            var objSTReturn = _context.STReturns
                                      .Where(p => p.Id == model.STReturnId
                                                  && p.StoreId == model.StoreId)
                                      .FirstOrDefault();
            if(objSTReturn == null)
            {
                return false;
            }

            var count = 0;
            if (objSTReturn.ClientReturnType == ClientReturnTypeEnum.StoreReturn)
            {
                count = this._context.STClientReturns
                                     .Where(x => x.Id == model.Id
                                                 && x.STReturnId == model.STReturnId
                                                 && x.ItemId == model.ItemId
                                                 && x.DeliveryStatus == DeliveryStatusEnum.Pending)
                                     .Count();
            }
            else
            {
                count = this._context.STClientReturns
                     .Where(x => x.Id == model.Id
                                 && x.STReturnId == model.STReturnId
                                 && x.ItemId == model.ItemId
                                 && x.DeliveryStatus == DeliveryStatusEnum.Waiting)
                     .Count();

            }

            if (count == 0)
            {
                return false;
            }

            return true;
        }
    }
}

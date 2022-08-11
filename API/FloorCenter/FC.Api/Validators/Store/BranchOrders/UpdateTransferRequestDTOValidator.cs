using FC.Api.DTOs.Store.BranchOrders;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FC.Core.Domain.Stores;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Store.BranchOrders
{
    public class UpdateTransferRequestDTOValidator : AbstractValidator<UpdateTransferRequestDTO>
    {
        private readonly DataContext _context;

        public UpdateTransferRequestDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p)
                .Must(ValidRecord)
                    .WithMessage("Record is not valid")
                    .WithName("Id");


            //  Validate ORNumber (TOR No.)
            RuleFor(p => p)
                .Must(ORNotEmptyIfInterbranch)
                    .WithMessage("'TOR No' should not be empty")
                    .WithName("TOR No")
                .Must(rec => rec.ORNumber.ToString().Length < 51)
                    .WithMessage("The length of 'OR Number' must be 50 characters or fewer")
                    .When(IsInterBranch);


            //  Validate SINumber
            RuleFor(p => p)
                .Must(SINotEmptyIfIntercompany)
                    .WithMessage("'SI Number' should not be empty")
                    .WithName("SINumber")
                .Must(rec => rec.SINumber.ToString().Length < 51)
                    .WithMessage("The length of 'SI Number' must be 50 characters or fewer")
                    .When(IsInterCompany);

            //  Validate DRNumber = WHDRNumber
            RuleFor(p => p)
                .Must(WHDRNumberNotEmptyIfIntercompany)
                    .WithMessage("'WHDR' should not be empty")
                    .WithName("WHDR")
                .Must(rec => rec.WHDRNumber.ToString().Length < 51)
                    .WithMessage("The length of 'WHDR Number' must be 50 characters or fewer")
                    .When(IsInterCompany);


        }

        private bool ValidRecord(UpdateTransferRequestDTO model)
        {
            if (model.Id.HasValue)
            {
                var record = _context.STOrders
                               .Where(p => p.Id == model.Id && p.RequestStatus == RequestStatusEnum.Approved)
                               .FirstOrDefault();

                return record.Id == model.Id;
            }

            return true;
        }


        private STOrder GetOrderRecord(UpdateTransferRequestDTO model)
        {
            if (model.Id.HasValue)
            {
                return _context.STOrders
                           .Where(p => p.Id == model.Id)
                           .FirstOrDefault();
            }

            return null;
        }

        private bool ORNotEmptyIfInterbranch(UpdateTransferRequestDTO model)
        {
           
            if (IsInterBranch(model))
            {
            return !string.IsNullOrWhiteSpace(model.ORNumber);
            }

            return true;
        }


        private bool SINotEmptyIfIntercompany(UpdateTransferRequestDTO model)
        {
           
           if(IsInterCompany(model))
           {
              return !string.IsNullOrWhiteSpace(model.SINumber);
           }
             
            return true;
        }

        private bool WHDRNumberNotEmptyIfIntercompany(UpdateTransferRequestDTO model)
        {
    
             if(IsInterCompany(model))
             {
                 return !string.IsNullOrWhiteSpace(model.WHDRNumber);
             }
  

            return true;
        }


        private bool IsInterCompany(UpdateTransferRequestDTO model)
        {
            var order = GetOrderRecord(model);
            if (order != null)
            {
                var storeRequestFrom = _context.Stores.Where(p => p.Id == order.StoreId).FirstOrDefault();
                var storeRequestTo = _context.Stores.Where(p => p.Id == order.OrderToStoreId).FirstOrDefault();

                if (storeRequestFrom != null && storeRequestTo != null)
                {
                    if (storeRequestFrom.CompanyId != storeRequestTo.CompanyId)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                
                }
            }
            return true;

        }

        private bool IsInterBranch(UpdateTransferRequestDTO model)
        {
            var order = GetOrderRecord(model);
            if (order != null)
            {
                var storeRequestFrom = _context.Stores.Where(p => p.Id == order.StoreId).FirstOrDefault();
                var storeRequestTo = _context.Stores.Where(p => p.Id == order.OrderToStoreId).FirstOrDefault();

                if (storeRequestFrom != null && storeRequestTo != null)
                {
                    if (storeRequestFrom.CompanyId == storeRequestTo.CompanyId)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            return true;

        }




    }

}

using FC.Api.DTOs.Store;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FC.Api.Validators
{
    public class UpdateWHDRNumberDTOValidator : AbstractValidator<UpdateWHDRNumberDTO>
    {
        private readonly DataContext _context;

        public UpdateWHDRNumberDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.Id)
                .NotEmpty()
                .Must(ValidRecord)
                    .WithMessage("Record is not valid");

            RuleFor(p => p.WHDRNumber)
                .NotEmpty()
                .When(p => p.RequestStatus != RequestStatusEnum.Cancelled)
                .MaximumLength(50)
                .Must(IsAlphaNumeric)
                    .WithMessage("Please enter only Alphanumeric characters on {PropertyName}")
                .Must(NotYetRegistered)
                    .WithMessage("{PropertyName} '{PropertyValue}' is already registered");


        }

        /// <summary>
        /// This is used to check if record is still valid
        /// RequestStatus should be Approved and OrderType should be ShowroomStockOrder or ClientOrder
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>False if not valid, otherwise True</returns>
        private bool ValidRecord(int id)
        {
            var count = this._context.STOrders.Where(x => x.Id == id
                                                          && (x.OrderType == OrderTypeEnum.ShowroomStockOrder || x.OrderType == OrderTypeEnum.ClientOrder)
                                                          && x.RequestStatus == RequestStatusEnum.Approved).Count();
            if (count != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsAlphaNumeric(string WHDRNumber)
        {
            Regex regex = new Regex("^[a-zA-Z0-9]*$");
            if (!regex.IsMatch(WHDRNumber))
            {
                return false;
            }

            return true;
        }

        private bool NotYetRegistered(string WHDRNumber)
        {
            var count = _context.STOrders.Where(p => p.WHDRNumber.ToLower() == WHDRNumber.ToLower()).Count();
            if (count != 0)
            {
                return false;
            }

            return true;
        }

        private bool IsRequired(UpdateWHDRNumberDTO model)
        {
            if(model.RequestStatus == RequestStatusEnum.Cancelled)
            {
                return true;
            }
            else
            {
                if(string.IsNullOrWhiteSpace(model.WHDRNumber))
                {
                    return false;
                }

                return true;
            }
        }

        private bool CheckLength(UpdateWHDRNumberDTO model)
        {
            if(model.RequestStatus != RequestStatusEnum.Cancelled && model.WHDRNumber.Length > 50)
            {
                return false;
            }

            return true;
        }
    }
}

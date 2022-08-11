using FC.Api.DTOs.Warehouse;
using FC.Api.DTOs.Warehouse.ModifyTonality;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Warehouse.ModifyTonality
{
    public class WHModifyItemTonalityDTOValidator : AbstractValidator<WHModifyItemTonalityDTO>
    {
        private readonly DataContext _context;

        public WHModifyItemTonalityDTOValidator(DataContext context)
        {

            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.STOrderId)
                .NotEmpty()
                .Must(ValidRecord)
                    .WithMessage("Order has existing modification request");

            RuleFor(p => p)
                .Must(CheckItems)
                    .WithMessage("Add atleast one item to be modified")
                .Must(checkDuplicateItem)
                    .WithMessage("Tonality cannot be used twice at the same time");


            RuleFor(p => p.ModifyItemTonalityDetails)
                .SetCollectionValidator(new WHModifyItemTonalityDetailsDTOValidator(context));
        }


        private bool ValidRecord(int? id)
        {
            var obj = _context.WHModifyItemTonality.Where(p => p.STOrderId == id && p.RequestStatus == RequestStatusEnum.Pending).Count();

            if (obj > 0)
            {
                return false;
            }

            return true;
        }


        private bool CheckItems(WHModifyItemTonalityDTO model)
        {
            var obj = model.ModifyItemTonalityDetails.Where(p => p.ItemId != null).Count();

            if(obj == 0)
            {
                return false;
            }

            return true;

        }

        private bool checkDuplicateItem(WHModifyItemTonalityDTO model)
        {
            var obj = model.ModifyItemTonalityDetails.Where(p => p.ItemId.HasValue).Select(p => p.ItemId);

            var modifiedItems = model.ModifyItemTonalityDetails.Where(p => p.ItemId == null);

            //Will check if tonality already in the list is selected
            var duplicateTonality = modifiedItems.Where(p => obj.Contains(p.OldItemId));


            if(obj.Count() != obj.Distinct().Count() || duplicateTonality.Count() > 0)
            {
                return false;
            }
            return true;
        }
    }
}

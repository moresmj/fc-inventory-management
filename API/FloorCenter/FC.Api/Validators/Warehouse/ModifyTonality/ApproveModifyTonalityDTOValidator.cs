using FC.Api.DTOs.Warehouse.ModifyTonality;
using FC.Api.Helpers;
using FC.Core.Domain.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Warehouse.ModifyTonality
{
    public class ApproveModifyTonalityDTOValidator : AbstractValidator<ApproveModifyTonalityDTO>
    {

        private readonly DataContext _context;

        public ApproveModifyTonalityDTOValidator(DataContext context)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;


            RuleFor(p => p)
                .Must(ValidRecord)
                .WithMessage("Record is not Valid");


            RuleFor(p => p)
                .Must(CheckIfNoDelReleased)
                .WithMessage("Invalid Record cannot be modified");

          

        }


        private bool ValidRecord(ApproveModifyTonalityDTO model)
        {
            if(model.Id.HasValue)
            {
                var record = _context.WHModifyItemTonality.Where(p => p.Id == model.Id && p.RequestStatus == RequestStatusEnum.Pending).FirstOrDefault();

                return record.Id == model.Id;
            }

            return true;
        }

        private bool CheckIfNoDelReleased(ApproveModifyTonalityDTO model)
        {
            if(model.STOrderId.HasValue)
            {
                var record = _context.STOrders
                                            .Where(p => p.Id == model.STOrderId)
                                            .Include(p => p.Deliveries)
                                                .ThenInclude(p => p.ShowroomDeliveries)
                                              .Include(p => p.Deliveries)
                                                .ThenInclude(p => p.ClientDeliveries);

                var test = record.ToList();

               var releasedRec = record.SelectMany(p => p.Deliveries.Where(
                                                         x => (x.ClientDeliveries.Any(c => c.ReleaseStatus == ReleaseStatusEnum.Released))
                                                               ||
                                                              (x.ShowroomDeliveries.Any(s => s.ReleaseStatus == ReleaseStatusEnum.Released))

                                                     )).Select(z => z.STOrderId).ToList();

                return releasedRec.Count() == 0;



            }

            return true;
        }
    }
}

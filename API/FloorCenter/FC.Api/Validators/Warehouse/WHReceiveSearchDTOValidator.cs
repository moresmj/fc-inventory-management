using FC.Api.DTOs.Warehouse;
using FC.Api.Helpers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Warehouse
{
    public class WHReceiveSearchDTOValidator : AbstractValidator<WHReceiveSearchDTO>
    {

        private readonly DataContext _context;


        public WHReceiveSearchDTOValidator(DataContext context)
        {

            this._context = context;


            CascadeMode = CascadeMode.StopOnFirstFailure;


            //Validate PODATE From to
            RuleFor(x => x)
                    .Must(CheckPODate)
                    .WithMessage("Invalid Date : PO DATE From is greater than PO Date To")
                    .WithName("PO DATE");

            //Validate DRDATE From to
            RuleFor(x => x)
                    .Must(CheckDRDate)
                    .WithMessage("Invalid Date : DR DATE From is greater than DR Date To")
                    .WithName("DR DATE");
        }

        private bool CheckPODate(WHReceiveSearchDTO model)
        {
        

               return dateRangeChecker(model.PODateFrom, model.PODateTo);
      
           

           
        }


        private bool CheckDRDate(WHReceiveSearchDTO model)
        {
           

                return dateRangeChecker(model.DRDateFrom, model.DRDateTo);
           
        }

        private bool dateRangeChecker(DateTime? dFrom, DateTime? dTo)
        {
            if (dFrom.HasValue && dTo.HasValue)
            {


                if (dFrom > dTo)
                {

                    return false;
                }
                else
                {
                    return true;
                }
            }

            return true;
        }

    }
}

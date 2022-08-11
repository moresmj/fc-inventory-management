using FC.Api.DTOs.Store.Returns;
using FC.Api.Helpers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Store.Returns
{
    public class SearchReturnsValidator : AbstractValidator<SearchReturns>
    {

        private readonly DataContext _context;

        private readonly DateTime? minDate;

        public SearchReturnsValidator(DataContext context)
        {

            this._context = context;
            this.minDate = new DateTime(1800, 01, 01);


            CascadeMode = CascadeMode.StopOnFirstFailure;


            RuleFor(x => x.RequestDateFrom)
                    .GreaterThan(x => this.minDate)
                    .WithMessage("Request Date From must be greater than '1/1/1800'");



            RuleFor(x => x.RequestDateTo)
                    .GreaterThan(x => this.minDate)
                        .WithMessage("Request Date To must be greater than '1/1/1800'");
                       

            //Validate DRDATE From to
            RuleFor(x => x)
                    .Must(CheckRequestDate)
                    .WithMessage("Invalid Date : Request DATE From is greater than Request Date To")
                    .WithName("Request DATE");
        }



        private bool CheckRequestDate(SearchReturns model)
        {


            return dateRangeChecker(model.RequestDateFrom, model.RequestDateTo);

        }

        private bool dateRangeChecker(DateTime? dFrom, DateTime? dTo)
        {
            if (dFrom.HasValue && dTo.HasValue)
            {
                    return !(dFrom > dTo);
            }

            return true;
        }

    }
}

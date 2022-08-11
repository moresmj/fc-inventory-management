using FC.Api.DTOs.Company;
using FC.Api.Helpers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.Company
{
    public class CompanyDTOValidator : AbstractValidator<CompanyDTO>
    {

        private readonly DataContext _context;

        public CompanyDTOValidator(DataContext context, bool runValidations = true)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            if (runValidations)
            {
                //removed for ticket #433
                //  Validate Store Name
                //RuleFor(p => p)
                //        .NotEmpty()
                //        .Must(NameStillAvailable)
                //            .WithMessage("Company Name is already registered");

                RuleFor(p => p.Code)
                    .NotEmpty()
                    .MinimumLength(2)
                    .MaximumLength(50);

                RuleFor(p => p.Name)
                    .NotEmpty()
                    .MinimumLength(2)
                    .MaximumLength(255);
                    

                RuleFor(p => p)
                    .NotEmpty()
                    .Must(CodeStillAvailable)
                            .WithMessage("Company Code is already registered");
            }

        }

        /// <summary>
        /// This function is used to validate if selected Company Name is already registered.
        /// </summary>
        /// <param name="name">Company Name to be checked if already registered.</param>
        /// <returns>False if already registered, otherwise True.</returns>
        private bool NameStillAvailable(CompanyDTO model)
        {
            if (!string.IsNullOrEmpty(model.Name))
            {
                var count = _context.Companies.Where(x => x.Name.ToLower() == model.Name.ToLower() && x.Id != model.Id).Count();

                if (count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// This function is used to validate if selected Company Name is already registered.
        /// </summary>
        /// <param name="name">Company Code to be checked if already registered.</param>
        /// <returns>False if already registered, otherwise True.</returns>
        private bool CodeStillAvailable(CompanyDTO model)
        {
            if (!string.IsNullOrEmpty(model.Name))
            {
                var count = _context.Companies.Where(x => x.Code.ToLower() == model.Code.ToLower() && x.Id != model.Id).Count();

                if (count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// This function is used to validate if selected Company ID is registered.
        /// </summary>
        /// <param name="id">ID to be checked if registered.</param>
        /// <returns>False if not valid means not registered, otherwise True</returns>
        public bool IdValid(int? id)
        {
            if (id.HasValue)
            {
                var count = this._context.Companies.Where(x => x.Id == id).Count();
                if (count != 1)
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

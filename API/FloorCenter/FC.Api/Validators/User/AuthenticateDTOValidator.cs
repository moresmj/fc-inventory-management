using FC.Api.DTOs.User;
using FC.Api.Helpers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.User
{
    public class AuthenticateDTOValidator : AbstractValidator<AuthenticateDTO>
    {

        private readonly DataContext _context;


        public AuthenticateDTOValidator(DataContext context, bool runDefaultValidations = true)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            if (runDefaultValidations == true)
            {
                RuleFor(p => p.Password)
                    .NotEmpty();

                RuleFor(p => p.UserName)
                    .NotEmpty();

                RuleFor(p => p)
                    .Must(UserIsActive)
                    .WithMessage("User account has been deactivated");

                

            }
        }

        private bool UserIsActive(AuthenticateDTO userDetails)
        {
            if (!string.IsNullOrWhiteSpace(userDetails.UserName) && (!string.IsNullOrWhiteSpace(userDetails.Password)))
            {
                var user = this._context.Users.Where(x => x.UserName.ToLower() == userDetails.UserName.ToLower()).SingleOrDefault();

                if (user != null && user.IsActive != null && user.IsActive == true)
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
    }
}

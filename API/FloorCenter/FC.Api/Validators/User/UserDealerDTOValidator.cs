using FC.Api.DTOs.User;
using FC.Api.Helpers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Api.Validators.User
{
    public class UserDealerDTOValidator : AbstractValidator<UserDealerDTO>
    {
        private readonly DataContext _context;

        public UserDealerDTOValidator(DataContext context, bool runDefaultValidations = true)
        {
            this._context = context;

            CascadeMode = CascadeMode.StopOnFirstFailure;

            if (runDefaultValidations == true)
            {
                //  Validate Id
                RuleFor(p => p.Id)
                    .Must(IdValid)
                        .When(p => p.Id != 0)
                        .WithMessage("Record is not valid");

                //  Validate Full Name
                RuleFor(p => p.FullName)
                    .NotEmpty();


                //  Validate User Name
                RuleFor(p => p.UserName)
                    .NotEmpty()
                        .When(r => r.Id == 0)
                    .Must(UserNameStillAvailable)
                        .When(r => r.Id == 0)
                        .WithMessage("User Name '{PropertyValue}' is already taken");


                //  Validate Password
                RuleFor(p => p.Password)
                    .NotEmpty()
                    .MinimumLength(8);


                //  Validate Email Address
                RuleFor(p => p.EmailAddress)
                    .NotEmpty()
                    .EmailAddress();

                //RuleFor(p => p)
                //    .Must(CheckDuplicateEmail)
                //       .WithMessage("Email address is already taken");

                RuleFor(p => p.StoresHandled)
                    .NotEmpty()
                    .Must(SelectedStoresIsValid)
                    .WithMessage("Selected store(s) do no exist");
            }

        }

        private bool UserNameStillAvailable(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var user = this._context.Users.Where(x => x.UserName.ToLower() == userName.ToLower()).SingleOrDefault();

                if (user == null)
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
        /// This function is used to validate if selected User ID is registered.
        /// </summary>
        /// <param name="id">ID to be checked if registered.</param>
        /// <returns>False if not valid means not registered, otherwise True</returns>
        public bool IdValid(int? id)
        {
            if (id.HasValue)
            {
                var count = this._context.Users.Where(x => x.Id == id).Count();

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

        private bool IdValid(int id)
        {
            var count = this._context.Users.Where(x => x.Id == id).Count();

            if (count != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public bool IsStoreValid(int? id)
        {
            if (id.HasValue)
            {
                var count = this._context.Stores.Where(x => x.Id == id).Count();
                return (count != 1) ? false : true;
            }
            return true;
        }

        public bool SelectedStoresIsValid(IList<int> storesHandled)
        {
            bool isNotValidStore = false;
            for (int i = 0; i < storesHandled.Count(); i++)
            {
                // Returns error message if store do not exist
                isNotValidStore = (_context.Stores.Count(x => x.Id == storesHandled[i]) == 0);
                if(isNotValidStore)
                {
                    // record has store that do not exist.
                    return false;
                }
            }
            return true;
        }

        public bool CheckDuplicateEmail(UserDealerDTO model)
        {
            if(!string.IsNullOrEmpty(model.EmailAddress))
            {
                var user = model.Id != 0 ? _context.Users.Where(p => p.EmailAddress.ToLower() == model.EmailAddress.ToLower() && p.Id == model.Id).FirstOrDefault() 
                                         : _context.Users.Where(p => p.EmailAddress.ToLower() == model.EmailAddress.ToLower()).FirstOrDefault();

                if(user != null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

using FC.Api.DTOs.User;
using FC.Api.Helpers;
using FC.Api.Validators.Warehouse;
using FC.Core.Domain.Users;
using FluentValidation;
using System.Linq;

namespace FC.Api.Validators.User
{
    public class UserDTOValidator : AbstractValidator<UserDTO>
    {

        private readonly DataContext _context;

        public UserDTOValidator(DataContext context, bool runDefaultValidations = true)
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
                    .EmailAddress();


                //  Validate Assignment
                RuleFor(p => p.Assignment)
                    .NotEmpty()
                    .IsInEnum()
                        .WithMessage("{PropertyName} is not valid");


                //  Validate Warehouse ID
                RuleFor(p => p.WarehouseId)
                    .NotEmpty()
                    .Must(new WarehouseDTOValidator(context, false).IdValid)
                        .When(r => r.Assignment == AssignmentEnum.Warehouse)
                        .WithMessage("{PropertyName} is not valid");


                ////  Validate Store ID
                //RuleFor(p => p.StoreId)
                //    .NotEmpty()
                //    .Must(new StoreDTOValidator(context, false).IdValid)
                //        .When(r => r.Assignment == AssignmentEnum.Store)
                //        .WithMessage("{PropertyName} is not valid");


                //  Validate User Type
                RuleFor(p => p.UserType)
                    .NotEmpty()
                    .IsInEnum()
                        .WithMessage("{PropertyName} is not valid");
            }

        }


        /// <summary>
        /// This function is used to validate if selected User Name is already registered.
        /// </summary>
        /// <param name="userName">User Name to be checked if already registered.</param>
        /// <returns>False if already registered, otherwise True</returns>
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


    }
}

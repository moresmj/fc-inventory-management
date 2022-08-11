using FluentValidation;

namespace InventorySystemAPI.Validators.User
{
    public class UserSearchValidator : AbstractValidator<Models.User.UserSearch>
    {

        public UserSearchValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;


            //  Validate Email Address
            RuleFor(p => p.EmailAddress)
                .EmailAddress();


            //  Validate Assignment
            RuleFor(p => p.Assignment)
                .IsInEnum()
                    .WithMessage("{PropertyName} is not valid");


            //  Validate User Type
            RuleFor(p => p.UserType)
                .IsInEnum()
                    .WithMessage("{PropertyName} is not valid");
        }

    }
}

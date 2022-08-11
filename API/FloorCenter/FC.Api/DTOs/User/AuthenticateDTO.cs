using FC.Api.Validators.User;
using FluentValidation.Attributes;

namespace FC.Api.DTOs.User
{
    [Validator(typeof(AuthenticateDTOValidator))]
    public class AuthenticateDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

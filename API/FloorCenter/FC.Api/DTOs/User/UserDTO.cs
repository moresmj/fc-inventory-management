using FC.Api.Helpers;
using FC.Api.Validators.User;
using FC.Core.Domain.Users;
using FluentValidation.Attributes;
using System;

namespace FC.Api.DTOs.User
{
    [Validator(typeof(UserDTOValidator))]
    public class UserDTO
    {

        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }

        public UserTypeEnum? UserType { get; set; }

        public string UserTypeStr
        {
            get
            {
                if (this.UserType.HasValue)
                {
                    return EnumExtensions.SplitName(Enum.GetName(typeof(UserTypeEnum), this.UserType));
                }

                return null;
            }
        }

        public AssignmentEnum? Assignment { get; set; }

        public string AssignmentStr
        {
            get
            {
                if (this.Assignment.HasValue)
                {
                    return EnumExtensions.SplitName(Enum.GetName(typeof(AssignmentEnum), this.Assignment));
                }

                return null;
            }
        }

        public int? WarehouseId { get; set; }
        public int? StoreId { get; set; }

        public DateTime? LastLogin { get; set; }

        public bool? isActive { get; set; }

    }
}

using FluentValidation.Attributes;
using InventorySystemAPI.Validators.User;

namespace InventorySystemAPI.Models.User
{

    [Validator(typeof(UserSearchValidator))]
    public class UserSearch 
    {

        public int? Id { get; set; }

        public string FullName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string EmailAddress { get; set; }

        public string Address { get; set; }

        public string ContactNumber { get; set; }

        public UserTypeEnum? UserType { get; set; }

        public AssignmentEnum? Assignment { get; set; }

        public int? WarehouseId { get; set; }

        public int? StoreId { get; set; }


    }
}

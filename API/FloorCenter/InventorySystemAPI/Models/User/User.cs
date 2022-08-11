using FluentValidation.Attributes;
using InventorySystemAPI.Validators.User;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventorySystemAPI.Models.User
{
    [Validator(typeof(UserValidator))]
    public class User : BaseEntity
    {

        [Column(Order = 0)]
        public override int Id { get; set; }

        [Column(Order = 1)]
        public string FullName { get; set; }

        [Column(Order = 2)]
        public string UserName { get; set; }

        [Column(Order = 3)]
        public string Password { get; set; }

        [Column(Order = 4)]
        public string EmailAddress { get; set; }

        [Column(Order = 5)]
        public string Address { get; set; }

        [Column(Order = 6)]
        public string ContactNumber { get; set; }

        [Column(Order = 7)]
        public UserTypeEnum? UserType { get; set; }

        public string UserTypeStr
        {
            get
            {
                if(this.UserType.HasValue)
                {
                    return Enum.GetName(typeof(UserTypeEnum), this.UserType);
                }

                return null;
            }
        }

        [Column(Order = 8)]
        public AssignmentEnum? Assignment { get; set; }

        public string AssignmentStr
        {
            get
            {
                if (this.Assignment.HasValue)
                {
                    return Enum.GetName(typeof(AssignmentEnum), this.Assignment);
                }

                return null;
            }
        }

        [Column(Order = 9)]
        public int? WarehouseId { get; set; }

        [Column(Order = 10)]
        public int? StoreId { get; set; }

        /// <summary>
        /// This method must be called before calling context .Add() or .Update()
        /// </summary>
        internal void EncryptPassword()
        {
            
        }

        //public string MenuSettings { get; set; }
    }
}
